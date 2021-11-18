using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Set = System.Collections.Generic.HashSet<Core.SplittableObject>;

namespace Core {
public class World : MonoBehaviour
{

    [SerializeField] List<Dimension> dimensions;
    [SerializeField] Transform inactivePoolRoot;
    [SerializeField] float radius = 30.0f;
    [SerializeField] float transitionDuration = 1.0f;
    [SerializeField] float fadeDuration = 0.2f;
    [SerializeField] DimensionTransition dimensionTransition;

    Dictionary<Dimension.Color, Dimension> dimensionMap;
    SplittableObjectPool objectPool;
    Set processedObjects, unprocessedObjects;
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
    public Dimension ActiveDimension {
        get {
            if (splitted) return GetDimension(dimensionTransition.ActiveDimensionColor);
            else return GetDimension(Dimension.Color.WHITE);
        }
    }
    public bool Transitting {
        get => dimensionTransition.Transitting;
    }

    void Awake()
    {
        dimensionMap = new Dictionary<Dimension.Color, Dimension>();
        foreach (Dimension d in dimensions)
            dimensionMap.Add(d.GetColor(), d);

        processedObjects = new Set();
        unprocessedObjects = new Set();

        objectPool = new SplittableObjectPool();

        splitted = false;
        Debug.Log("World Awake");
    }

    private void Start()
    {
        SplittableObject[] so = FindObjectsOfType<SplittableObject>();
        foreach (SplittableObject s in so)
        {
            s.Dim = dimensionMap[Dimension.Color.WHITE];
            s.ObjectColor.Init();
            unprocessedObjects.Add(s);
            objectPool.SetActive(s);
        }
        dimensionTransition.Init();
    }

    public void SplitObjects()
    {
        while (unprocessedObjects.Count > 0)
        {
            var so = unprocessedObjects.FirstOrDefault();
            if (so == null) break;
            so.Split();
        }
        SwapSet();
    }

    public void MergeObjects()
    {
        while (unprocessedObjects.Count > 0)
        {
            var so = unprocessedObjects.FirstOrDefault();
            if (so == null) break;
            so.Merge(null);
        }
        SwapSet();
    }

    public void RotateDimensions(int dir)
    {
        if (!Splitted || dimensionTransition.Transitting) return;
        StartCoroutine(dimensionTransition.RotateTransition(dir));
    }

    public Dimension GetDimension(Dimension.Color color)
    {
        return dimensionMap[color];
    }

    public void MoveToProcessed(SplittableObject so)
    {
        unprocessedObjects.Remove(so);
        if (!processedObjects.Add(so))
        {
            Debug.Log("Existed " + so.gameObject.name + " " + so.Dim.GetColor().ToString());
        }
    }

    public void RemoveFromSet(SplittableObject so)
    {
        unprocessedObjects.Remove(so);
        processedObjects.Remove(so);
    }

    public void AddToUnprocessed(SplittableObject so)
    {
        unprocessedObjects.Add(so);
    }

    public SplittableObject InstantiateNewObjectToDimension(SplittableObject so, Dimension.Color color)
    {
        var newSo = objectPool.Instantiate(so.name);
        newSo.gameObject.name = so.name;
        newSo.Dim = GetDimension(color);
        MoveTransformToNewParent(newSo.transform, newSo.Dim.transform, so.transform.localPosition, so.transform.localRotation);
        return newSo;
    }

    public void MoveObjectToDimension(SplittableObject so, Dimension.Color color)
    {
        Vector3 localPos = so.transform.localPosition;
        Quaternion localRot = so.transform.localRotation;
        so.Dim = GetDimension(color);
        MoveTransformToNewParent(so.transform, so.Dim.transform, localPos, localRot);
    }

    public void MoveObjectToDimension(GameObject go, Dimension.Color color)
    {
        Vector3 localPos = go.transform.localPosition;
        Quaternion localRot = go.transform.localRotation;
        MoveTransformToNewParent(go.transform, GetDimension(color).transform, localPos, localRot);
    }

    public void DeleteObject(SplittableObject so)
    {
        RemoveFromSet(so);
        Vector3 localPos = so.transform.localPosition;
        Quaternion localRot = so.transform.localRotation;
        MoveTransformToNewParent(so.transform, inactivePoolRoot, localPos, localRot);
        objectPool.SetInactive(so);
    }

    void MoveTransformToNewParent(Transform child, Transform parent, Vector3 localPos, Quaternion localRot)
    {
        child.SetParent(parent);
        child.localPosition = localPos;
        child.localRotation = localRot;
    }

    void SwapSet()
    {
        Set tmp = unprocessedObjects;
        unprocessedObjects = processedObjects;
        processedObjects = tmp;
    }

    void SplitDimensions()
    {
        StartCoroutine(dimensionTransition.SplitTransition());
    }

    void MergeDimensions()
    {
        StartCoroutine(dimensionTransition.MergeTransition());
    }

    public void Toggle()
    {
        if (dimensionTransition.Transitting) return;
        if (splitted)
        {
            splitted = false;
            MergeDimensions();
        }
        else
        {
            splitted = true;
            SplitDimensions();
        }
    }

    void Log()
    {
        Debug.Log("");
        Debug.Log("-----processed Object-----");
        foreach (SplittableObject so in processedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
        Debug.Log("");
        Debug.Log("-----unprocessedObjects Object-----");
        foreach (SplittableObject so in unprocessedObjects) {
            Debug.Log(so.gameObject.name + ", " + so.Dim.GetColor().ToString());
        }
    }

}

}