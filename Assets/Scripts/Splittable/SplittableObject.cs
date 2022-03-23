
using UnityEngine;
using System.Collections.Generic;

using Core;

namespace Splittable
{
    // TODO: persistent color?
    /// <summary>
    /// Base class of splittable object.
    /// </summary>
    [RequireComponent(typeof(ObjectColor))]
    public class SplittableObject : MonoBehaviour
    {
        [SerializeField] protected bool isPersistentColor;
        [SerializeField] protected bool defaultInactive;
        [SerializeField] protected bool isConvex = true;

        protected ObjectColor objectColor;
        protected Dimension dimension;
        protected Collider col;
        protected bool _IsMerged;

        /// <summary>
        /// If the object had merged in the merging process.
        /// </summary>
        public bool IsMerged {
            get => _IsMerged;
            set => _IsMerged = value;
        }

        /// <summary>
        /// If the object is default inactive.
        /// </summary>
        public bool DefaultInactive {
            get => defaultInactive;
            set => defaultInactive = value;
        }

        /// <summary>
        /// The dimension which the object is located in.
        /// </summary>
        public Dimension Dim {
            get => dimension;
            set {
                dimension = value;
                objectColor.DimensionColor = dimension.color;
            }
        }

        /// <summary>
        /// The <c>ObjectColor</c> of the object.
        /// </summary>
        public ObjectColor ObjectColor {
            get => objectColor;
        }

        /// <summary>
        /// The <c>Dimension.Color</c> of the <c>SplittableObject</c>.
        /// </summary>
        public Dimension.Color Color {
            get => objectColor.Color;
            set {
                objectColor.Color = value;
                if (objectColor.Color == Dimension.Color.NONE)
                {
                    World.Instance.DeactivateObject(this);
                }
            }
        }

        /// <summary>
        /// If the object's color is persistent.
        /// </summary>
        public bool IsPersistentColor {
            get => isPersistentColor;
        }

        /// <summary>
        /// Invoked after <c>SplittableObject</c> is initialized by <c>World</c>.
        /// </summary>
        /// <returns></returns>
        [HideInInspector] public UnityEngine.Events.UnityEvent OnInitialized = new UnityEngine.Events.UnityEvent();

        protected void Awake()
        {
            col = GetComponent<Collider>();
            objectColor = GetComponent<ObjectColor>();
            IsMerged = false;
            Dim = World.Instance.ActiveDimension;
            objectColor.Color = objectColor.Color;
            // objectColor.InitColor();
        }

        /// <summary>
        /// Splits the object into 3 dimensions based on the Color.
        /// </summary>
        public virtual void Split()
        {
            foreach (var dim in World.Instance.Dimensions)
            {
                if (dim.color == Dimension.Color.NONE)
                    continue;
                var splittedColor = objectColor.Color & dim.color;
                if (splittedColor != Dimension.Color.NONE)
                {
                    var splitted = World.Instance.InstantiateNewObjectToDimension(this, dim.color);
                    splitted.Color = splittedColor;
                    splitted.IsMerged = false;
                    World.Instance.MoveToProcessed(splitted);
                }
            }
            this.IsMerged = false;
            World.Instance.DeactivateObject(this);
        }

        /// <summary>
        /// Merges object into white dimension.
        /// </summary>
        /// <param name="parent"> The parent object in recursive merging. </param>
        public virtual void Merge(SplittableObject parent) {

            IsMerged = true;

            Dimension.Color mergedColor = this.Color;
            if (parent != null && parent.Color == Dimension.Color.BLACK)
                mergedColor = Dimension.Color.BLACK;
            var siblings = new List<SplittableObject>();
            var others = new List<SplittableObject>();
            ProcessCollidedObjects(ref mergedColor, siblings, others);
            if (mergedColor == Dimension.Color.BLACK)
            {
                var error = World.Instance.InstantiateNewObjectToDimension("ErrorSpace", Dimension.Color.WHITE);
                error.transform.localPosition = Util.Coordinate.Lattice(transform.localPosition);
                foreach (var so in siblings)
                {
                    so.Merge(this);
                    World.Instance.DeactivateObject(so);
                }
                World.Instance.DeactivateObject(this);
            }
            else
            {
                World.Instance.MoveObjectToDimension(this, Dimension.Color.WHITE);
                World.Instance.MoveToProcessed(this);
                Color = mergedColor;
                foreach (var so in siblings)
                    World.Instance.DeactivateObject(so);
            }

        }

        /// <summary>
        /// It recursively calculates the merged color and siblings with collided objects.
        /// </summary>
        /// <param name="mergedColor"> The final merged color. </param>
        /// <param name="siblings"> A <c>List</c> of collided siblings. </param>
        /// <param name="others"> A <c>List</c> of collided objects that're not siblings. </param>
        protected void ProcessCollidedObjects(ref Dimension.Color mergedColor,
                        List<SplittableObject> siblings, List<SplittableObject> others)
        {
            Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents - Util.Fuzzy.amountVec3, Quaternion.identity);
            Util.Debug.DrawBox(col.bounds.center, Quaternion.identity, col.bounds.extents*2, UnityEngine.Color.blue, 50.0f);
            foreach (Collider c in colliders)
            {
                if (c == null || !c.gameObject.activeSelf || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;
                var so = c.gameObject.GetComponent<SplittableObject>();
                if (so == null || so.IsMerged || so.gameObject.name == "ErrorSpace")
                {
                    continue;
                }
                if (!IsPenetrated(so) && (so.isConvex || isConvex))
                {
                    continue;
                }
                if (c.gameObject.name == gameObject.name)
                {
                    if (so.Color == Dimension.Color.BLACK ||
                        !Util.Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition))
                    {
                        siblings.Add(so);
                        mergedColor = Dimension.Color.BLACK;
                        Color = Dimension.Color.BLACK;
                    }
                    else
                    {
                        siblings.Add(so);
                        mergedColor = Dimension.AddColor(mergedColor, so.Color);
                        so.IsMerged = true;
                    }
                }
                else
                {
                    others.Add(so);
                    mergedColor = Dimension.Color.BLACK;
                    Color = Dimension.Color.BLACK;
                    so.Merge(this);
                }
            }
        }

        /// <summary>
        /// Checks if the given splittableObject is overlapped with current one.
        /// </summary>
        /// <param name="s">The splittableObject to be checked with.</param>
        /// <returns>True if they are overlapped.</returns>
        protected bool IsPenetrated(SplittableObject s)
        {
            if (!this.isConvex && !s.isConvex)
                return false;
            float dist;
            Vector3 dir;
            if (Physics.ComputePenetration(s.col, s.transform.position, s.transform.rotation,
                    col, transform.position, transform.rotation, out dir, out dist))
            {
                Util.Debug.Log(gameObject, " penetrated with " + s.gameObject.name + " " + s.transform.GetInstanceID() + " dir " + dir + " dist " + dist);
                Debug.DrawLine(col.transform.position, col.transform.position+dir * dist, UnityEngine.Color.green, 50);
                if (dist <= Util.Fuzzy.amount)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reports if the object's color is the same as the dimension which it is located.
        /// </summary>
        /// <returns> A boolean represent if the object is in the correct dimension. </returns>
        public bool IsInCorrectDim()
        {
            if (Dim == null) return false;
            return this.Color == Dim.color;
        }

    }


}