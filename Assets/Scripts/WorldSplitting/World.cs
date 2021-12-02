using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Set = System.Collections.Generic.HashSet<Core.SplittableObject>;

namespace Core
{
    [RequireComponent(typeof(DimensionTransition))]
    public class World : MonoBehaviour
    {

        [SerializeField] List<ObjectColor> dimensions;
        [SerializeField] Transform inactivePoolRoot;
        [SerializeField] List<Dimension.ColorSetting> colorSettings;

        DimensionTransition dimensionTransition;
        PlayerInteraction playerInteraction;
        Dictionary<Dimension.Color, ObjectColor> dimensionMap;
        SplittableObjectPool objectPool;
        Set processedObjects, unprocessedObjects;
        bool splitted;

        public Dictionary<Dimension.Color, ObjectColor> Dims {
            get => dimensionMap;
        }
        public bool Splitted {
            get => splitted;
        }
        public ObjectColor ActiveDimension {
            get {
                if (splitted) return Dims[dimensionTransition.ActiveDimensionColor];
                else return Dims[Dimension.Color.WHITE];
            }
        }
        public bool Transitting {
            get => dimensionTransition.Transitting;
        }
        public SplittableObjectPool ObjectPool {
            get => objectPool;
        }

        void Awake()
        {
            playerInteraction = FindObjectOfType<PlayerInteraction>();
            dimensionTransition = GetComponent<DimensionTransition>();
            dimensionMap = new Dictionary<Dimension.Color, ObjectColor>();
            foreach (ObjectColor d in dimensions)
            {
                dimensionMap.Add(d.Color, d);
            }

            processedObjects = new Set();
            unprocessedObjects = new Set();

            objectPool = new SplittableObjectPool();

            splitted = false;

            if(Dimension.MaterialColor.Count == 0)
            {
                foreach (Dimension.ColorSetting setting in colorSettings)
                {
                    Dimension.MaterialColor.Add(setting.colorTag, setting.color32);
                }
            }
           
            Debug.Log("World Awake");
        }

        private void Start()
        {
            SplittableObject[] so = FindObjectsOfType<SplittableObject>();
            foreach (SplittableObject s in so)
            {
                s.Dim = dimensionMap[Dimension.Color.WHITE];
                s.ObjectColor.Init();
                if (s.DefaultInactive)
                {
                    DeleteObject(s);
                }
                else
                {
                    unprocessedObjects.Add(s);
                    objectPool.SetActive(s);
                }
            }
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

        public void MoveToProcessed(SplittableObject so)
        {
            unprocessedObjects.Remove(so);
            if (!processedObjects.Add(so))
            {
                Debug.Log("Existed " + so.gameObject.name + " " + so.Dim.Color.ToString());
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
            newSo.Dim = Dims[color];
            MoveTransformToNewParent(newSo.transform, newSo.Dim.transform, so.transform.localPosition, so.transform.localRotation);
            return newSo;
        }

        public void MoveObjectToDimension(SplittableObject so, Dimension.Color color)
        {
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            so.Dim = Dims[color];
            MoveTransformToNewParent(so.transform, so.Dim.transform, localPos, localRot);
        }

        public void MoveObjectToDimension(GameObject go, Dimension.Color color)
        {
            Vector3 localPos = go.transform.localPosition;
            Quaternion localRot = go.transform.localRotation;
            MoveTransformToNewParent(go.transform, Dims[color].transform, localPos, localRot);
        }

        public void DeleteObject(SplittableObject so)
        {
            RemoveFromSet(so);
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            MoveTransformToNewParent(so.transform, inactivePoolRoot, localPos, localRot);
            objectPool.SetInactive(so);
        }

        public void ActivateObject(SplittableObject so, Dimension.Color dim)
        {
            objectPool.SetActive(so);
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            MoveTransformToNewParent(so.transform, Dims[dim].transform, localPos, localRot);
            unprocessedObjects.Add(so);
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

        public void RotateDimensions(int dir)
        {
            if (!Splitted || dimensionTransition.Transitting) return;
            playerInteraction.OnDimensionChange();
            StartCoroutine(dimensionTransition.RotateTransition(dir));
        }


        public void Toggle()
        {
            if (dimensionTransition.Transitting) return;
            playerInteraction.OnDimensionChange();
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

        public void Log()
        {
            Debug.Log("");
            Debug.Log("-----processed Object-----");
            foreach (SplittableObject so in processedObjects) {
                if (so.gameObject.name != "Box") continue;
                Debug.Log(so.gameObject.name + ", " + so.ObjectColor.Color.ToString() + ", ID: " + so.transform.GetInstanceID());
            }
            Debug.Log("");
            Debug.Log("-----unprocessedObjects Object-----");
            foreach (SplittableObject so in unprocessedObjects) {
                if (so.gameObject.name != "Box") continue;
                Debug.Log(so.gameObject.name + ", " + so.ObjectColor.Color.ToString() + ", ID: " + so.transform.GetInstanceID());
            }
        }

    }

}