
using UnityEngine;
using System.Collections.Generic;

namespace Core
{
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
        protected ObjectColor dimension;
        protected Collider col;
        protected Dictionary<Dimension.Color, SplittableObject> siblings;
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
        }

        /// <summary>
        /// The dimension which the object is located in.
        /// </summary>
        public ObjectColor Dim {
            get => dimension;
            set => dimension = value;
        }

        /// <summary>
        /// The <c>ObjectColor</c> of the object.
        /// </summary>
        public ObjectColor ObjectColor {
            get => objectColor;
        }

        /// <summary>
        /// The color of the object.
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
        /// A <c>Dictionary</c> mapping of color and the sibling objects in different dimension.
        /// </summary>
        public Dictionary<Dimension.Color, SplittableObject> Siblings {
            get => siblings;
        }

        /// <summary>
        /// If the object's color is persistent.
        /// </summary>
        public bool IsPersistentColor {
            get => isPersistentColor;
        }

        void Awake()
        {
            col = GetComponent<Collider>();
            objectColor = GetComponent<ObjectColor>();
            siblings = new Dictionary<Dimension.Color, SplittableObject>();
            foreach (Dimension.Color bc in Dimension.BaseColor)
                Siblings.Add(bc, null);
            IsMerged = false;
        }

        protected void OnDisable()
        {
            // World.Instance.RemoveFromSet(this);
        }

        /// <summary>
        /// Splits the object into 3 dimensions based on the Color.
        /// </summary>
        public virtual void Split()
        {
            if (Dim.Color != Dimension.Color.WHITE)
            {
                World.Instance.MoveToProcessed(this);
                return;
            }
            else if (isPersistentColor)
                SplitPersistent();
            else
                SplitColor();
        }

        /// <summary>
        /// Splits the object with persistent color.
        /// </summary>
        protected void SplitPersistent()
        {
            List<Dimension.Color> splittedColor;
            if (this.Color == Dimension.Color.BLACK || this.Color == Dimension.Color.NONE)
                splittedColor = Dimension.BaseColor;
            else
                splittedColor = Dimension.SplitColor(this.Color);
            foreach (Dimension.Color sc in splittedColor)
            {
                if (Siblings[sc] == null)
                {
                    var so = World.Instance.InstantiateNewObjectToDimension(this, sc);
                    so.Color = this.Color;
                    Siblings[sc] = so;
                }
                else
                {
                    Siblings[sc].transform.localPosition = transform.localPosition;
                }
                World.Instance.RemoveFromSet(Siblings[sc]);
            }
            World.Instance.RemoveFromSet(this);
        }

        /// <summary>
        /// Splits the object with normal color.
        /// </summary>
        protected void SplitColor()
        {
            var splittedColor = Dimension.SplitColor(Color);
            var missingColor = Dimension.MissingColor(Color);
            foreach (Dimension.Color sc in splittedColor)
            {
                if (Siblings[sc] == null)
                {
                    Siblings[sc] = World.Instance.InstantiateNewObjectToDimension(this, sc);
                    Siblings[sc].Color = sc;
                }
                else
                {
                    Siblings[sc].transform.localPosition = transform.localPosition;
                    Siblings[sc].IsMerged = false;
                }
                World.Instance.MoveToProcessed(Siblings[sc]);
            }

            foreach (Dimension.Color sc in missingColor)
            {
                if (Siblings[sc] != null)
                {
                    World.Instance.DeactivateObject(Siblings[sc]);
                    Siblings[sc] = null;
                }
            }
            World.Instance.DeactivateObject(this);
        }

        /// <summary>
        /// Merges object into white dimension.
        /// </summary>
        /// <param name="parent"> The parent object in recursive merging. </param>
        public virtual void Merge(SplittableObject parent) {
            if (isPersistentColor)
            {
                World.Instance.MoveToProcessed(this);
                return;
            }

            IsMerged = true;

            Dimension.Color mergedColor = this.Color;
            if (parent != null && parent.Color == Dimension.Color.BLACK)
                mergedColor = Dimension.Color.BLACK;
            List<SplittableObject> curSiblings = new List<SplittableObject>();
            ProcessCollidedObjects(ref mergedColor, curSiblings);

            if (mergedColor == Dimension.Color.BLACK)
                MergeToBlack(curSiblings);
            else
                MergeToNewParent(mergedColor, curSiblings);
        }

        /// <summary>
        /// It recursively calculates the merged color and siblings with collided objects.
        /// </summary>
        /// <param name="mergedColor"> The final merged color. </param>
        /// <param name="curSiblings"> A <c>List</c> of collided siblings. </param>
        protected void ProcessCollidedObjects(ref Dimension.Color mergedColor, List<SplittableObject> curSiblings)
        {
            Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents - Util.Fuzzy.amountVec3, transform.rotation);
            // Debug.DrawLine(col.bounds.center+col.bounds.extents, col.bounds.center-col.bounds.extents, UnityEngine.Color.red, 1000.0f);
            foreach (Collider c in colliders)
            {
                if (c == null || !c.gameObject.activeSelf || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;
                var so = c.gameObject.GetComponent<SplittableObject>();
                if (so == null || so.IsMerged)
                {
                    continue;
                }
                else if (so.Color == Dimension.Color.BLACK)
                {
                    mergedColor = Dimension.Color.BLACK;
                }
                else if (c.gameObject.name == gameObject.name && Util.Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition))
                {
                    mergedColor = Dimension.AddColor(mergedColor, so.Color);
                    so.IsMerged = true;
                    curSiblings.Add(so);
                }
                else if (IsPenetrated(so))
                {
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
                // Debug.Log(gameObject.name + " penetrated with " + s.gameObject.name + " dir " + dir + " dist " + dist);
                if (dist <= Util.Fuzzy.amount)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Merges the siblings into a black object in white dimension.
        /// </summary>
        /// <param name="curSiblings"> A <c>List</c> of collided siblings. </param>
        protected void MergeToBlack(List<SplittableObject> curSiblings)
        {
            World.Instance.MoveObjectToDimension(this, Dimension.Color.WHITE);
            this.Color = Dimension.Color.BLACK;
            isPersistentColor = true;
            World.Instance.MoveToProcessed(this);
            for (int i = 0 ; i < curSiblings.Count; i++)
            {
                World.Instance.DeactivateObject(curSiblings[i]);
            }
        }

        /// <summary>
        /// Merges the siblings into a new object in white dimension.
        /// </summary>
        /// <param name="mergedColor"> The color of the new object. </param>
        /// <param name="curSiblings"> A <c>List</c> of collided siblings. </param>
        protected void MergeToNewParent(Dimension.Color mergedColor, List<SplittableObject> curSiblings)
        {
            var parent = World.Instance.InstantiateNewObjectToDimension(this, Dimension.Color.WHITE);
            parent.Color = mergedColor;
            World.Instance.MoveToProcessed(parent);

            curSiblings.Add(this);

            var splittedColor = Dimension.SplitColor(mergedColor);
            var missingColor = Dimension.MissingColor(mergedColor);

            for (int i = 0; i < curSiblings.Count; i++)
            {
                World.Instance.MoveObjectToDimension(curSiblings[i], splittedColor[i]);
                curSiblings[i].Color = splittedColor[i];
                parent.Siblings[splittedColor[i]] = curSiblings[i];
                World.Instance.MoveToProcessed(curSiblings[i]);
            }
            for (int i = 0; i < missingColor.Count; i++)
            {
                parent.Siblings[missingColor[i]] = null;
            }
        }

        /// <summary>
        /// Reports if the object's color is the same as the dimension which it is located.
        /// </summary>
        /// <returns> A boolean represent if the object is in the correct dimension. </returns>
        public bool IsInCorrectDim()
        {
            return this.Color == Dim.Color;
        }

    }


}