
using UnityEngine;
using System.Collections.Generic;

public class SplitableObject : MonoBehaviour {

    [SerializeField] public ObjectColor objectColor;
    [SerializeField] public Dictionary<Dimension.Color, SplitableObject> splitted;
    [SerializeField] World world;
    Collider col;

    bool _IsMerged;
    public bool IsMerged {
        get => _IsMerged;
        set => _IsMerged = value;
    }

    void Awake() {
        col = GetComponent<Collider>();
        objectColor = GetComponent<ObjectColor>();
        world = FindObjectOfType<World>();
        splitted = new Dictionary<Dimension.Color, SplitableObject>();
        foreach (Dimension.Color bc in Dimension.BaseColor)
            splitted.Add(bc, null);
        IsMerged = false;
    }

    public void Split(HashSet<SplitableObject> mergedObjects, HashSet<SplitableObject> splittedObjects,
                        HashSet<SplitableObject> blackOutObjects, HashSet<SplitableObject> toBeDestroyedObjects) {
        if (objectColor.GetColor() == Dimension.Color.BLACK)
            SplitBlack(mergedObjects, blackOutObjects);
        else
            SplitColor(mergedObjects, splittedObjects, toBeDestroyedObjects);
    }

    void SplitBlack(HashSet<SplitableObject> mergedObjects, HashSet<SplitableObject> blackOutObjects) {
        foreach (Dimension.Color sc in Dimension.BaseColor) {
            var s = splitted[sc];
            if (s == null) {
                var so = InstantiateToDimension(sc);
                splitted[sc] = so;
                blackOutObjects.Add(so);
            }
            else {
                s.transform.localPosition = transform.localPosition;
            }
        }
        mergedObjects.Remove(this);
        blackOutObjects.Add(this);
    }

    void SplitColor(HashSet<SplitableObject> mergedObjects,
            HashSet<SplitableObject> splittedObjects, HashSet<SplitableObject> toBeDestroyedObjects) {

        var splittedColor = Dimension.SplitColor(objectColor.GetColor());
        var missingColor = Dimension.MissingColor(objectColor.GetColor());
        foreach (Dimension.Color sc in splittedColor) {
            var s = splitted[sc];
            if (s == null) {
                var so = InstantiateToDimension(sc);
                splittedObjects.Add(so);
            }
            else {
                s.transform.localPosition = transform.localPosition;
                s.IsMerged = false;
                mergedObjects.Remove(s);
                splittedObjects.Add(s);
            }
        }
        foreach (Dimension.Color sc in missingColor) {
            var s = splitted[sc];
            if (s != null) {
                mergedObjects.Remove(s);
                toBeDestroyedObjects.Add(s);
            }
        }
        mergedObjects.Remove(this);
        toBeDestroyedObjects.Add(this);
    }

    public void Merge(SplitableObject parent, HashSet<SplitableObject> mergedObjects, HashSet<SplitableObject> splittedObjects,
            HashSet<SplitableObject> blackOutObjects, HashSet<SplitableObject> toBeDestroyedObjects) {

        IsMerged = true;

        Dimension.Color mergedColor = objectColor.GetColor();

        if (parent != null && parent.objectColor.GetColor() == Dimension.Color.BLACK) {
            mergedColor = Dimension.Color.BLACK;
        }

        List<SplitableObject> siblings = new List<SplitableObject>();
        Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 0.8f, transform.rotation);

        foreach (Collider c in colliders) {
            if (c == null || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;

            var so = c.gameObject.GetComponent<SplitableObject>();
            if (so.IsMerged)
                continue;
            else if (so.objectColor.GetColor() == Dimension.Color.BLACK) {
                mergedColor = Dimension.Color.BLACK;
            }
            else if (c.gameObject.name == gameObject.name && Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition)) {
                mergedColor = Dimension.AddColor(mergedColor, so.objectColor.GetColor());
                so.IsMerged = true;
                siblings.Add(so);
            }
            else {
                mergedColor = Dimension.Color.BLACK;
                so.Merge(this, mergedObjects, splittedObjects, blackOutObjects, toBeDestroyedObjects);
            }
        }

        if (mergedColor == Dimension.Color.RED || mergedColor == Dimension.Color.GREEN || mergedColor == Dimension.Color.BLUE) {
            toBeDestroyedObjects.Add(this);
            splittedObjects.Remove(this);
        }
        else if (mergedColor == Dimension.Color.BLACK) {
            MergeToBlack(siblings, mergedObjects, splittedObjects, blackOutObjects);
        }
        else {
            MergeToNewParent(mergedColor, siblings, mergedObjects, splittedObjects);
        }
    }

    private void MergeToBlack(List<SplitableObject> siblings, HashSet<SplitableObject> mergedObjects,
            HashSet<SplitableObject> splittedObjects, HashSet<SplitableObject> blackOutObjects) {

        var parent = InstantiateAsParent();
        parent.objectColor.SetColor(Dimension.Color.BLACK);

        siblings.Add(this);

        for (int i = 0 ; i < siblings.Count; i++) {
            siblings[i].objectColor.SetColor(Dimension.Color.BLACK);
            siblings[i].IsMerged = false;
            blackOutObjects.Add(siblings[i]);
            splittedObjects.Remove(siblings[i]);
            parent.splitted[Dimension.BaseColor[i]] = siblings[i];
        }

        if (siblings.Count == Dimension.BaseColor.Count)
            blackOutObjects.Add(parent);
        else
            mergedObjects.Add(parent);
    }

    private void MergeToNewParent(Dimension.Color mergedColor, List<SplitableObject> siblings,
                    HashSet<SplitableObject> mergedObjects, HashSet<SplitableObject> splittedObjects) {

        var parent = InstantiateAsParent();
        parent.objectColor.SetColor(mergedColor);
        mergedObjects.Add(parent);

        siblings.Add(this);

        var splittedColor = Dimension.SplitColor(mergedColor);

        for (int i = 0; i < siblings.Count; i++) {
            siblings[i].MoveToDimension(splittedColor[i]);
            mergedObjects.Add(siblings[i]);
            splittedObjects.Remove(siblings[i]);
            parent.splitted[splittedColor[i]] = siblings[i];
        }
    }

    public SplitableObject InstantiateToDimension(Dimension.Color color) {
        var so = Instantiate<SplitableObject>(this);
        so.gameObject.name = gameObject.name;
        so.objectColor.SetColor(color);
        so.transform.SetParent(world.GetDimension(color).transform);
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
        transform.SetParent(world.GetDimension(color).transform);
        transform.localPosition = localPos;
        transform.localRotation = localRot;
    }

}