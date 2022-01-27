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
        [SerializeField] Transform inactiveRoot;
        [SerializeField] List<Dimension.ColorSetting> colorSettings;
        [SerializeField] Dimension.Color startDimensionColor = Dimension.Color.WHITE;
        [HideInInspector] public UnityEvent BeforeMerge = new UnityEvent();
        [HideInInspector] public UnityEvent BeforeSplit = new UnityEvent();
        DimensionTransition dimensionTransition;
        Dictionary<Dimension.Color, int> dimId;
        SplittableObjectPool objectPool;
        Set processedObjects, unprocessedObjects;
        bool splitted;
        int activeDimId;

        /// <summary>
        /// The list of dimensions' root.
        /// </summary>
        /// <value></value>
        public List<ObjectColor> Dimensions {
            get => dimensions;
        }

        public Dictionary<Dimension.Color, int> DimId {
            get => dimId;
        }

        /// <summary>
        /// The currently active dimension.
        /// </summary>
        public ObjectColor ActiveDimension {
            get => dimensions[activeDimId];
        }

        public int ActiveDimId {
            get => activeDimId;
        }

        /// <summary>
        /// If the world is splitted.
        /// </summary>
        public bool Splitted {
            get => splitted;
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

        /// <summary>
        /// The dimension color at start time.
        /// </summary>
        /// <value></value>
        public Dimension.Color StartDimensionColor {
            get => startDimensionColor;
        }

        void Awake()
        {
            if (_instance != null)
                Debug.LogError("Multiple instance of World created");

            _instance = this;
            dimensionTransition = GetComponent<DimensionTransition>();

            dimId = new Dictionary<Dimension.Color, int>();
            foreach (Dimension.Color c in Dimension.AllColor)
                dimId.Add(c, -1);
            dimId[Dimension.Color.WHITE] = 0;
            activeDimId = 0;

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
                s.OnInitialized.Invoke();
                var localPos = s.transform.position;
                var localRot = s.transform.rotation;
                MoveTransformToNewParent(s.transform, dimensions[dimId[Dimension.Color.WHITE]].transform, localPos, localRot);
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
            if (startDimensionColor != Dimension.Color.WHITE)
                DefaultSplit();
        }

        private void DefaultSplit()
        {
            dimensionTransition.DefaultSplit();
            dimensions[0].Color = startDimensionColor;
            dimId[startDimensionColor] = 0;
            dimId[Dimension.Color.WHITE] = -1;

            var missingColors = Dimension.MissingColor(startDimensionColor);
            int i = 1;
            for (; i < missingColors.Count + 1; i++)
            {
                dimensions[i].Color = missingColors[i-1];
                dimId[missingColors[i-1]] = i;
            }
            for (; i < dimensions.Count; i++)
                dimensions[i].Color = Dimension.Color.NONE;
            SplitObjects();
            splitted = true;
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
                Util.Debug.Log(gameObject, so.gameObject.name + " " + so.Dim.Color.ToString() + " existed in processed objects");
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
        /// Instantiates a <c>SplittableObject</c> into a specified dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be instantiated. </param>
        /// <param name="color"> The color of the target dimension to instantiate into. </param>
        /// <returns> The instantiated <c>SplittableObject</c>. </returns>
        public SplittableObject InstantiateNewObjectToDimension(SplittableObject so, Dimension.Color color)
        {
            var newSo = objectPool.Instantiate(so.name);
            if (newSo == null)
            {
                newSo = GameObject.Instantiate<SplittableObject>(so);
                MoveToProcessed(newSo);
                objectPool.SetActive(newSo);
            }
            newSo.gameObject.name = so.name;
            newSo.Dim = dimensions[dimId[color]];
            MoveTransformToNewParent(newSo.transform, newSo.Dim.transform, so.transform.localPosition, so.transform.localRotation);
            return newSo;
        }

        /// <summary>
        /// Instantiates a <c>SplittableObject</c> object into a specified dimension.
        /// </summary>
        /// <param name="soName"> The name of the <c>SplittableObject</c> to be instantiated. </param>
        /// <param name="color"> The color of the target dimension to instantiate into. </param>
        /// <returns> The instantiated <c>SplittableObject</c>. </returns>
        public SplittableObject InstantiateNewObjectToDimension(string soName, Dimension.Color color)
        {
            var newSo = objectPool.Instantiate(soName);
            newSo.gameObject.name = soName;
            newSo.Dim = dimensions[dimId[color]];
            MoveTransformToNewParent(newSo.transform, newSo.Dim.transform, newSo.transform.localPosition, newSo.transform.localRotation);
            return newSo;
        }

        /// <summary>
        /// Moves the given <c>SplittableObject</c> to the active dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be moved. </param>
        public void MoveObjectToActiveDimension(SplittableObject so)
        {
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            so.Dim = dimensions[activeDimId];
            MoveTransformToNewParent(so.transform, so.Dim.transform, localPos, localRot);
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
            so.Dim = dimensions[dimId[color]];
            MoveTransformToNewParent(so.transform, so.Dim.transform, localPos, localRot);
        }

        /// <summary>
        /// Moves the given <c>SplittableObject</c> to a specified dimension.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be moved. </param>
        /// <param name="id"> The id of the target dimension to move into. </param>
        public void MoveObjectToDimension(SplittableObject so, int id)
        {
            Vector3 localPos = so.transform.localPosition;
            Quaternion localRot = so.transform.localRotation;
            so.Dim = dimensions[id];
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
            MoveTransformToNewParent(go.transform, dimensions[dimId[color]].transform, localPos, localRot);
        }

        /// <summary>
        /// Moves the given <c>GameObject</c> to a specified dimension.
        /// </summary>
        /// <param name="go"> The <c>GameObject</c> to be moved. </param>
        /// <param name="id"> The id of the target diemension to move into. </param>
        public void MoveObjectToDimension(GameObject go, int id)
        {
            Vector3 localPos = go.transform.localPosition;
            Quaternion localRot = go.transform.localRotation;
            MoveTransformToNewParent(go.transform, dimensions[id].transform, localPos, localRot);
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
            MoveTransformToNewParent(so.transform, inactiveRoot, localPos, localRot);
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
            MoveTransformToNewParent(so.transform, dimensions[dimId[color]].transform, localPos, localRot);
            unprocessedObjects.Add(so);
            so.Dim = dimensions[dimId[color]];
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

        public void Toggle()
        {
            if (splitted)
            {
                splitted = false;
                BeforeMerge.Invoke();
                dimensions[0].Color = Dimension.Color.NONE;
                dimensions[1].Color = Dimension.Color.WHITE;
                dimensions[2].Color = Dimension.Color.NONE;
                dimId[Dimension.Color.WHITE] = 1;
                dimId[Dimension.Color.RED] = -1;
                dimId[Dimension.Color.BLUE] = -1;
                dimId[Dimension.Color.GREEN] = -1;
                activeDimId = 1;
                StartCoroutine(dimensionTransition.MergeTransition());
            }
            else
            {
                splitted = true;
                BeforeSplit.Invoke();
                dimensions[0].Color = Dimension.Color.RED;
                dimensions[1].Color = Dimension.Color.BLUE;
                dimensions[2].Color = Dimension.Color.GREEN;
                dimId[Dimension.Color.RED] = 0;
                dimId[Dimension.Color.BLUE] = 1;
                dimId[Dimension.Color.GREEN] = 2;
                dimId[Dimension.Color.WHITE] = -1;
                activeDimId = 1;
                StartCoroutine(dimensionTransition.SplitTransition());

            }
        }

        /// <summary>
        /// Gets the dimension based on the color.
        /// </summary>
        /// <param name="color"> The dimension color. </param>
        /// <returns> If the dimension is active, the the <c>ObjectColor</c> of the dimension is returned,
        /// otherwise null is returned. </returns>
        public ObjectColor GetDim(Dimension.Color color)
        {
            if (dimId[color] > 0 && dimId[color] < dimensions.Count)
                return dimensions[dimId[color]];
            else
                return null;
        }

    }

}