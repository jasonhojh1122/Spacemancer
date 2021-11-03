using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{

    [SerializeField] List<Dimension> dimensions;
    [SerializeField] float radius = 30.0f;
    [SerializeField] float transitionDuration = 1.0f;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] DimensionTransition dimensionTransition;

    public float Radius {
        get => radius;
    }

    public float TransitionDur {
        get => transitionDuration;
    }
    public float FadeDur {
        get => fadeDuration;
    }

    Dictionary<Dimension.Color, Dimension> dimensionMap;
    public Dictionary<Dimension.Color, Dimension> Dims {
        get => dimensionMap;
    }

    HashSet<SplitableObject> mergedObjects;
    HashSet<SplitableObject> splittedObjects;
    HashSet<SplitableObject> toBeDestroyedObjects;
    HashSet<SplitableObject> blackOutObjects;

    bool splitted;
    public bool Splitted {
        get => splitted;
    }

    void Awake() {
        dimensionMap = new Dictionary<Dimension.Color, Dimension>();
        foreach (Dimension d in dimensions)
            dimensionMap.Add(d.GetColor(), d);

        mergedObjects = new HashSet<SplitableObject>();
        splittedObjects = new HashSet<SplitableObject>();
        toBeDestroyedObjects = new HashSet<SplitableObject>();
        blackOutObjects = new HashSet<SplitableObject>();

        SplitableObject[] so = FindObjectsOfType<SplitableObject>();
        foreach (SplitableObject s in so) {
            s.Dim = dimensionMap[Dimension.Color.WHITE];
            mergedObjects.Add(s);
        }

        splitted = false;
    }

    public void SplitObjects() {
        while (mergedObjects.Count > 0) {
            var so = mergedObjects.FirstOrDefault();
            if (so == null) break;
            so.Split(mergedObjects, splittedObjects, blackOutObjects, toBeDestroyedObjects);
        }
    }

    public void MergeObjects() {
        while (splittedObjects.Count > 0) {
            var so = splittedObjects.FirstOrDefault();
            if (so == null) break;
            if (so.ObjectColor == Dimension.Color.BLACK) {
                so.IsMerged = false;
                blackOutObjects.Add(so);
            }
            else {
                so.Merge(null, mergedObjects, splittedObjects, blackOutObjects, toBeDestroyedObjects);
            }
        }
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

    void Log() {
        Debug.Log("");
        Debug.Log("-----Merged Object-----");
        foreach (SplitableObject so in mergedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.gameObject.GetInstanceID());
        }
        Debug.Log("");
        Debug.Log("-----Splitted Object-----");
        foreach (SplitableObject so in splittedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.gameObject.GetInstanceID());
        }
        Debug.Log("");
        Debug.Log("-----BlackOut Object-----");
        foreach (SplitableObject so in blackOutObjects) {
            Debug.Log(so.gameObject.name + ", " + so.gameObject.GetInstanceID());
        }
        Debug.Log("");
        Debug.Log("-----To be destoryed Object-----");
        foreach (SplitableObject so in toBeDestroyedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.gameObject.GetInstanceID());
        }
        Debug.Log("");
    }

}
