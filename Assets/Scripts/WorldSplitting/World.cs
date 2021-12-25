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
        static World _instance;
        public static World Instance {
            get => _instance;
        }
        [SerializeField] List<ObjectColor> dimensions;
        [SerializeField] List<Dimension.ColorSetting> colorSettings;

        DimensionTransition dimensionTransition;
        PlayerInteraction playerInteraction;
        Dictionary<Dimension.Color, ObjectColor> dimensionMap;
        SplittableObjectPool objectPool;
        Set processedObjects, unprocessedObjects;
        bool splitted;

        /// <summary>
        /// A <c>Dictionary</c> mapping of color and dimensions' <c>ObjectColor</c>.
        /// </summary>
        public Dictionary<Dimension.Color, ObjectColor> Dims {
            get => dimensionMap;
        }

        /// <summary>
        /// If the world is splitted.
        /// </summary>
        public bool Splitted {
            get => splitted;
        }

        /// <summary>
        /// The currently active dimension.
        /// </summary>
        public ObjectColor ActiveDimension {
            get {
                if (splitted) return Dims[dimensionTransition.ActiveDimensionColor];
                else return Dims[Dimension.Color.WHITE];
            }
        }

        /// <summary>
        /// If the world is splitting, merging or rotating.
        /// </summary>
        /// <value></value>
        public bool Transitting {
            get => dimensionTransition.Transitting;
        }

        /// <summary>
        /// The <c>SplittableObjectPool</c> which stores all the <c>SplittableObject</c> in
        /// the scene.
        /// </summary>
        public SplittableObjectPool ObjectPool {
            get => objectPool;
        }

        void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("Multiple instance of World created");
            }
            _instance = this;
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
        }

        private void Start()
        {
            SplittableObject[] so = FindObjectsOfType<SplittableObject>();
            foreach (SplittableObject s in so)
            {
                s.Dim = Dims[Dimension.Color.WHITE];
                s.ObjectColor.Init();
                var localPos = Dims[Dimension.Color.WHITE].transform.InverseTransformPoint(s.transform.position);
                var localRot = Quaternion.Inverse(Dims[Dimension.Color.WHITE].transform.rotation) * s.transform.rotation;
                MoveTransformToNewParent(s.transform, Dims[Dimension.Color.WHITE].transform, localPos, localRot);
                if (s.DefaultInactive)
                {
                    DeactivateObject(s);
                }
                else
                {
                    unprocessedObjects.Add(s);
                    objectPool.SetActive(s);
                }
            }
        }

        /// <summary>
        /// Splits the objects into different dimension based on their color.
        /// </summary>
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

        /// <summary>
        /// Merges the objects in different dimension into white dimension.
        /// </summary>
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

        /// <summary>
        /// Moves the given <c>SplittableObject</c> from unprocessed set into processed set.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be moved. </param>
        public void MoveToProcessed(SplittableObject so)
        {
            unprocessedObjects.Remove(so);
            if (!processedObjects.Add(so))
            {
                Debug.Log("Existed " + so.gameObject.name + " " + so.Dim.Color.ToString());
            }
        }

        /// <summary>
        /// Removes the given <c>SplittableObject</c> from processed and unprocessed sets.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be removed. </param>
        public void RemoveFromSet(SplittableObject so)
        {
            unprocessedObjects.Remove(so);
            processedObjects.Remove(so);
        }

        /// <summary>
        /// Adds the given <c>SplittableObject</c> into unprocessed set.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be added. </param>
        public void AddToUnprocessed(SplittableObject so)
        {
            unprocessedObjects.Add(so);
        }

        /// <summary>
        /// Instantiates a cloned object into a specified dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be cloned. </param>
        /// <param name="color"> The color of the target dimension to instantiate into. </param>
        /// <returns> The cloned <c>SplittableObject</c>. </returns>
        public SplittableObject InstantiateNewObjectToDimension(SplittableObject so, Dimension.Color color)
        {
            var newSo = objectPool.Instantiate(so.name);
            newSo.gameObject.name = so.name;
            newSo.Dim = Dims[color];
            MoveTransformToNewParent(newSo.transform, newSo.Dim.transform, so.transform.localPosition, so.transform.localRotation);
            return newSo;
        }

        /// <summary>
        /// Moves the given <c>SplittableObject</c> to a specified dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be moved. </param>
        /// <param name="color"> The color of the target dimension to move into. </param>
        public void MoveObjectToDimension(SplittableObject so, Dimension.Color color)
        {
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            so.Dim = Dims[color];
            MoveTransformToNewParent(so.transform, so.Dim.transform, localPos, localRot);
        }

        /// <summary>
        /// Moves the given <c>GameObject</c> to a specified dimension.
        /// </summary>
        /// <param name="go"> The <c>GameObject</c> to be moved. </param>
        /// <param name="color"> The color of the target diemension to move into. </param>
        public void MoveObjectToDimension(GameObject go, Dimension.Color color)
        {
            Vector3 localPos = go.transform.localPosition;
            Quaternion localRot = go.transform.localRotation;
            MoveTransformToNewParent(go.transform, Dims[color].transform, localPos, localRot);
        }

        /// <summary>
        /// Deactivates the given <c>SplittableObject</c> from the scene.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be deactivated. </param>
        public void DeactivateObject(SplittableObject so)
        {
            RemoveFromSet(so);
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            MoveTransformToNewParent(so.transform, Dims[Dimension.Color.NONE].transform, localPos, localRot);
            objectPool.SetInactive(so);
        }

        /// <summary>
        /// Activates the given <c>SplittableObject</c> into specified dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be activated. </param>
        /// <param name="color"> The color the target dimension to activate into. </param>
        public void ActivateObject(SplittableObject so, Dimension.Color color)
        {
            objectPool.SetActive(so);
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            MoveTransformToNewParent(so.transform, Dims[color].transform, localPos, localRot);
            unprocessedObjects.Add(so);
            so.Dim = Dims[color];
            so.Color = so.Color;
        }

        /// <summary>
        /// Sets the child's parent with specified local position and local rotation.
        /// </summary>
        /// <param name="child"> The <c>Transform</c> of the child. </param>
        /// <param name="parent"> The <c>Transform</c> of the parent</param>
        /// <param name="localPos"> The local position. </param>
        /// <param name="localRot"> The local rotation. </param>
        void MoveTransformToNewParent(Transform child, Transform parent, Vector3 localPos, Quaternion localRot)
        {
            child.SetParent(parent);
            child.localPosition = localPos;
            child.localRotation = localRot;
        }

        /// <summary>
        /// Swaps the unprocessed set and the processed set.
        /// </summary>
        void SwapSet()
        {
            Set tmp = unprocessedObjects;
            unprocessedObjects = processedObjects;
            processedObjects = tmp;
        }

        /// <summary>
        /// Starts the split process.
        /// </summary>
        void SplitDimensions()
        {
            StartCoroutine(dimensionTransition.SplitTransition());
        }

        /// <summary>
        /// Starts the merge process.
        /// </summary>
        void MergeDimensions()
        {
            StartCoroutine(dimensionTransition.MergeTransition());
        }

        /// <summary>
        /// Rotates the dimensions if the world is splitted.
        /// </summary>
        /// <param name="dir">The direction to rotate. Greater than zero for right rotation.
        /// Less than zero for left rotation. </param>
        public void RotateDimensions(int dir)
        {
            if (!Splitted || dimensionTransition.Transitting) return;
            playerInteraction.OnDimensionChange();
            StartCoroutine(dimensionTransition.RotateTransition(dir));
        }

        /// <summary>
        /// Toggles world's splitting and merging.
        /// </summary>
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
                Debug.Log(so.gameObject.name + ", " + so.Color.ToString() + ", ID: " + so.transform.GetInstanceID());
            }
            Debug.Log("");
            Debug.Log("-----unprocessedObjects Object-----");
            foreach (SplittableObject so in unprocessedObjects) {
                if (so.gameObject.name != "Box") continue;
                Debug.Log(so.gameObject.name + ", " + so.Color.ToString() + ", ID: " + so.transform.GetInstanceID());
            }
        }

    }

}