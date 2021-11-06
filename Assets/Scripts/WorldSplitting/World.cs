using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Set = System.Collections.Generic.HashSet<SplitableObject>;

public class World : MonoBehaviour
{

    [SerializeField] List<Dimension> dimensions;
    [SerializeField] float radius = 30.0f;
    [SerializeField] float transitionDuration = 1.0f;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] DimensionTransition dimensionTransition;

    Dictionary<Dimension.Color, Dimension> dimensionMap;
    Set processedObjects;
    Set unprocessedObjects;
    Set toBeDestroyedObjects;
    Set blackOutObjects;
    bool splitted;

    public float Radius {
        get => radius;
    }
    public float TransitionDur {
        get => transitionDuration;
    }
    public float FadeDur {
        get => fadeDuration;
    }
    public Dictionary<Dimension.Color, Dimension> Dims {
        get => dimensionMap;
    }
    public bool Splitted {
        get => splitted;
    }

    void Awake() {
        dimensionMap = new Dictionary<Dimension.Color, Dimension>();
        foreach (Dimension d in dimensions)
            dimensionMap.Add(d.GetColor(), d);

        processedObjects = new Set();
        unprocessedObjects = new Set();
        blackOutObjects = new Set();
        toBeDestroyedObjects = new Set();

        splitted = false;
        Debug.Log("World Awkae");
    }

    private void OnEnable() {

    }

    private void Start() {
        SplitableObject[] so = FindObjectsOfType<SplitableObject>();
        foreach (SplitableObject s in so) {
            s.Dim = dimensionMap[Dimension.Color.WHITE];
            unprocessedObjects.Add(s);
        }
    }

    public void SplitObjects() {
        while (unprocessedObjects.Count > 0) {
            var so = unprocessedObjects.FirstOrDefault();
            if (so == null) break;
            so.Split();
        }
        SwapSet();
    }

    public void MergeObjects() {
        while (unprocessedObjects.Count > 0) {
            var so = unprocessedObjects.FirstOrDefault();
            if (so == null) break;
            so.Merge(null);
        }
        SwapSet();
    }

    private void SwapSet() {
        Set tmp = unprocessedObjects;
        unprocessedObjects = processedObjects;
        processedObjects = tmp;
    }

    public void DestoryObjects() {
        while (toBeDestroyedObjects.Count > 0) {
            var so = toBeDestroyedObjects.FirstOrDefault();
            toBeDestroyedObjects.Remove(so);
            Destroy(so.gameObject);
        }
    }

    void SplitDimensions() {
        StartCoroutine(dimensionTransition.SplitTransition());
    }

    void MergeDimensions() {
        StartCoroutine(dimensionTransition.MergeTransition());
    }

    public void RotateDimensions(int dir) {
        if (!Splitted || dimensionTransition.Transitting) return;
        StartCoroutine(dimensionTransition.RotationTransition(dir));
    }

    public void Toggle() {
        if (dimensionTransition.Transitting) return;
        if (splitted) {
            splitted = false;
            MergeDimensions();
        }
        else {
            splitted = true;
            SplitDimensions();
        }
    }

    public Dimension GetDimension(Dimension.Color color) {
        return dimensionMap[color];
    }

    public void MoveToProcessed(SplitableObject so) {
        unprocessedObjects.Remove(so);
        if (!processedObjects.Add(so)) {
            Debug.Log("Existed " + so.gameObject.name + " " + so.Dim.GetColor().ToString());
        }
    }

    public void MoveToBeDestoryed(SplitableObject so) {
        unprocessedObjects.Remove(so);
        toBeDestroyedObjects.Add(so);
    }

    public void MoveToBlackOut(SplitableObject so) {
        unprocessedObjects.Remove(so);
        blackOutObjects.Add(so);
    }

    public void RemoveFromSet(SplitableObject so) {
        unprocessedObjects.Remove(so);
        processedObjects.Remove(so);
        blackOutObjects.Remove(so);
    }

    public void AddToUnprocessed(SplitableObject so) {
        unprocessedObjects.Add(so);
    }

    void Log() {
        Debug.Log("");
        Debug.Log("-----processed Object-----");
        foreach (SplitableObject so in processedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
        Debug.Log("");
        Debug.Log("-----unprocessedObjects Object-----");
        foreach (SplitableObject so in unprocessedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
        Debug.Log("");
        Debug.Log("-----BlackOut Object-----");
        foreach (SplitableObject so in blackOutObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
        Debug.Log("");
        Debug.Log("-----To be destoryed Object-----");
        foreach (SplitableObject so in toBeDestroyedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
        Debug.Log("");
    }

}
