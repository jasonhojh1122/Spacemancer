
using UnityEngine;
using System.Collections.Generic;

using Set = System.Collections.Generic.HashSet<SplitableObject>;

public class SplitableObject : MonoBehaviour {

    [SerializeField] Dimension.Color color;
    [SerializeField] bool staticObject;
    [SerializeField] World world;
    Collider col;
    Dimension dimension;
    Renderer _renderer;
    MaterialPropertyBlock _propBlock;
    Dictionary<Dimension.Color, SplitableObject> splitted;

    bool _IsMerged;
    public bool IsMerged {
        get => _IsMerged;
        set => _IsMerged = value;
    }
    public Dimension Dim {
        get => dimension;
        set => dimension = value;
    }
    public Dimension.Color ObjectColor {
        get => color;
        set {
            color = value;
            _propBlock.SetColor("_Color", Dimension.MaterialColor[color]);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }
    public Dictionary<Dimension.Color, SplitableObject> Splitted {
        get => splitted;
    }

    void Awake() {
        col = GetComponent<Collider>();
        if (world == null)
            world = FindObjectOfType<World>();
        _renderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
        splitted = new Dictionary<Dimension.Color, SplitableObject>();
        foreach (Dimension.Color bc in Dimension.BaseColor)
            splitted.Add(bc, null);
        IsMerged = false;
        ObjectColor = color;
    }

    private void Start() {
        // world.AddToUnprocessed(this);
    }

    private void OnDisable() {
        world.RemoveFromSet(this);
    }

    public void Split() {
        if (ObjectColor == Dimension.Color.BLACK)
            SplitBlack();
        else if (staticObject && Dim.GetColor() != Dimension.Color.WHITE)
            world.MoveToProcessed(this);
        else
            SplitColor();
    }

    void SplitBlack() {
        foreach (Dimension.Color sc in Dimension.BaseColor) {
            var s = Splitted[sc];
            if (s == null) {
                var so = InstantiateToDimension(sc);
                Splitted[sc] = so;
                world.MoveToBlackOut(so);
            }
            else {
                s.transform.localPosition = transform.localPosition;
            }
        }
        world.MoveToBlackOut(this);
    }

    void SplitColor() {

        var splittedColor = Dimension.SplitColor(ObjectColor);
        var missingColor = Dimension.MissingColor(ObjectColor);
        foreach (Dimension.Color sc in splittedColor) {
            if (Splitted[sc] == null) {
                Splitted[sc] = InstantiateToDimension(sc);
            }
            else {
                Splitted[sc].transform.localPosition = transform.localPosition;
                Splitted[sc].Dim = world.GetDimension(sc);
                Splitted[sc].IsMerged = false;
            }
            world.MoveToProcessed(Splitted[sc]);
        }

        foreach (Dimension.Color sc in missingColor)
            if (Splitted[sc] != null)
                world.MoveToBeDestoryed(Splitted[sc]);

        if (staticObject)
            world.MoveToProcessed(this);
        else
            world.MoveToBeDestoryed(this);
    }

    public void Merge(SplitableObject parent) {

        if (staticObject && dimension.GetColor() != Dimension.Color.WHITE) {
            world.MoveToProcessed(this);
            return;
        }

        IsMerged = true;

        Dimension.Color mergedColor = (staticObject) ? Dimension.Color.NONE : ObjectColor;

        if (parent != null && parent.ObjectColor == Dimension.Color.BLACK)
            mergedColor = Dimension.Color.BLACK;

        List<SplitableObject> siblings = new List<SplitableObject>();
        Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 0.8f, transform.rotation);

        foreach (Collider c in colliders) {
            if (c == null || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;

            var so = c.gameObject.GetComponent<SplitableObject>();
            if (so == null || so.IsMerged)
                continue;
            else if (so.ObjectColor == Dimension.Color.BLACK)
                mergedColor = Dimension.Color.BLACK;
            else if (c.gameObject.name == gameObject.name && Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition)) {
                mergedColor = Dimension.AddColor(mergedColor, so.ObjectColor);
                so.IsMerged = true;
                siblings.Add(so);
            }
            else {
                mergedColor = Dimension.Color.BLACK;
                so.Merge(this);
            }
        }
        if (mergedColor == Dimension.Color.BLACK)
            MergeToBlack(siblings);
        else if (staticObject)
            MergeToStaticObject(mergedColor);
        else
            MergeToNewParent(mergedColor, siblings);
    }

    private void MergeToBlack(List<SplitableObject> siblings) {

        var parent = InstantiateAsParent();
        parent.ObjectColor = Dimension.Color.BLACK;

        siblings.Add(this);

        for (int i = 0 ; i < siblings.Count; i++) {
            siblings[i].ObjectColor = Dimension.Color.BLACK;
            siblings[i].IsMerged = false;
            parent.Splitted[Dimension.BaseColor[i]] = siblings[i];
            world.MoveToBlackOut(siblings[i]);
        }

        if (siblings.Count == Dimension.BaseColor.Count)
            world.MoveToBlackOut(parent);
        else
            world.MoveToProcessed(parent);
    }

    private void MergeToNewParent(Dimension.Color mergedColor, List<SplitableObject> siblings) {

        var parent = InstantiateAsParent();
        parent.ObjectColor = mergedColor;
        world.MoveToProcessed(parent);

        siblings.Add(this);

        var splittedColor = Dimension.SplitColor(mergedColor);

        for (int i = 0; i < siblings.Count; i++) {
            siblings[i].MoveToDimension(splittedColor[i]);
            siblings[i].ObjectColor = splittedColor[i];
            parent.Splitted[splittedColor[i]] = siblings[i];
            world.MoveToProcessed(siblings[i]);
        }
    }

    private void MergeToStaticObject(Dimension.Color mergedColor) {
        ObjectColor = mergedColor;
        world.MoveToProcessed(this);
    }

    public SplitableObject InstantiateToDimension(Dimension.Color color) {
        var so = Instantiate<SplitableObject>(this);
        so.gameObject.name = gameObject.name;
        so.ObjectColor = color;
        so.Dim = world.GetDimension(color);
        so.transform.SetParent(so.Dim.transform);
        so.transform.localPosition = transform.localPosition;
        so.transform.localRotation = transform.localRotation;
        return so;
    }

    public SplitableObject InstantiateAsParent() {
        return InstantiateToDimension(Dimension.Color.WHITE);
    }

    public void MoveToDimension(Dimension.Color color) {
        Vector3 localPos = transform.localPosition;
        Quaternion localRot = transform.localRotation;
        Dim = world.GetDimension(color);
        transform.SetParent(Dim.transform);
        transform.localPosition = localPos;
        transform.localRotation = localRot;
    }

    public bool IsInCorrectDim() {
        return ObjectColor == Dim.GetColor();
    }

}
