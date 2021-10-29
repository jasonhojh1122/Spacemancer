using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{

    [SerializeField] List<Dimension> dimensions;
    [SerializeField] float moveSpeed;
    [SerializeField] float trunSpeed;

    public UnityEvent OnMergeTransitionStart;
    public UnityEvent OnMergeTransitionEnd;
    public UnityEvent OnSplitTransitionStart;
    public UnityEvent OnSplitTransitionEnd;

    Dictionary<Dimension.Color, Dimension> dimensionMap;

    HashSet<SplitableObject> mergedObjects;
    HashSet<SplitableObject> splittedObjects;
    HashSet<SplitableObject> toBeDestroyedObjects;
    HashSet<SplitableObject> blackOutObjects;

    bool splitted;


    void Start() {
        dimensionMap = new Dictionary<Dimension.Color, Dimension>();
        foreach (Dimension d in dimensions) {
            dimensionMap.Add(d.GetColor(), d);
        }

        mergedObjects = new HashSet<SplitableObject>();
        splittedObjects = new HashSet<SplitableObject>();
        toBeDestroyedObjects = new HashSet<SplitableObject>();
        blackOutObjects = new HashSet<SplitableObject>();

        SplitableObject[] so = FindObjectsOfType<SplitableObject>();
        foreach (SplitableObject s in so) {
            mergedObjects.Add(s);
        }

        OnMergeTransitionEnd.AddListener(MergeObjects);
        splitted = false;
    }

    public void SplitObjects() {
        while (mergedObjects.Count > 0) {
            var so = mergedObjects.FirstOrDefault();
            if (so == null) break;
            so.Split(mergedObjects, splittedObjects, blackOutObjects, toBeDestroyedObjects);
        }
        StartCoroutine(DestoryObjects());
        Log();
    }

    public void MergeObjects() {
        while (splittedObjects.Count > 0) {
            var so = splittedObjects.FirstOrDefault();
            if (so == null) break;
            if (so.objectColor.GetColor() == Dimension.Color.BLACK) {
                so.IsMerged = false;
                blackOutObjects.Add(so);
            }
            else {
                so.Merge(null, mergedObjects, splittedObjects, blackOutObjects, toBeDestroyedObjects);
            }
        }
        StartCoroutine(DestoryObjects());
        Log();
    }

    public IEnumerator DestoryObjects() {
        while (toBeDestroyedObjects.Count > 0) {
            var so = toBeDestroyedObjects.FirstOrDefault();
            toBeDestroyedObjects.Remove(so);
            Destroy(so.gameObject);
        }
        yield return null;
    }

    public void SplitDimensions() {
        SplitObjects();
        StartCoroutine(SplitTransition());
    }

    public void MergeDimensions() {
        StartCoroutine(MergeTransition());
    }


    IEnumerator SplitTransition() {
        OnSplitTransitionStart.Invoke();
        while (true) {
            int c = 0;
            foreach (Dimension.Color color in Dimension.BaseColor) {
                bool r = MoveDimension(dimensionMap[color].transform, dimensionMap[color].targetPosition,
                            dimensionMap[color].targetRotation);
                if (r) c++;
            }
            if (c == Dimension.BaseColor.Count) break;
            else yield return null;
        }
        Physics.SyncTransforms();
        OnSplitTransitionEnd.Invoke();
    }

    IEnumerator MergeTransition() {
        OnMergeTransitionStart.Invoke();
        while (true) {
            int c = 0;
            foreach (Dimension.Color color in Dimension.BaseColor) {
                bool r = MoveDimension(dimensionMap[color].transform, dimensionMap[Dimension.Color.WHITE].targetPosition,
                            dimensionMap[Dimension.Color.WHITE].targetRotation);
                if (r) c++;
            }
            if (c == Dimension.BaseColor.Count) break;
            else yield return null;
        }
        Physics.SyncTransforms();
        OnMergeTransitionEnd.Invoke();
    }

    bool MoveDimension(Transform t, Vector3 tarPos, Quaternion tarRot) {
        t.position = Vector3.MoveTowards(t.position, tarPos, moveSpeed * Time.deltaTime);
        t.rotation = Quaternion.RotateTowards(t.rotation, tarRot, trunSpeed * Time.deltaTime);
        return (Fuzzy.CloseVector3(t.position, tarPos) && Fuzzy.CloseQuaternion(t.rotation, tarRot));
    }

    public void Toggle() {
        if (splitted) {
            MergeDimensions();
            splitted = false;
        }
        else {
            SplitDimensions();
            splitted = true;
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
