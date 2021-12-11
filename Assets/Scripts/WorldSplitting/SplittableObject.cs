
using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    [RequireComponent(typeof(ObjectColor))]
    public class SplittableObject : MonoBehaviour
    {
        [SerializeField] protected bool isPersistentColor;
        [SerializeField] protected bool defaultInactive;

        protected ObjectColor objectColor;
        protected ObjectColor dimension;
        protected Collider col;
        protected Dictionary<Dimension.Color, SplittableObject> siblings;
        protected bool _IsMerged;

        public bool IsMerged {
            get => _IsMerged;
            set => _IsMerged = value;
        }
        public bool DefaultInactive {
            get => defaultInactive;
        }
        public ObjectColor Dim {
            get => dimension;
            set => dimension = value;
        }
        public ObjectColor ObjectColor {
            get => objectColor;
        }
        public Dimension.Color Color {
            get => objectColor.Color;
            set {
                objectColor.Color = value;
                if (objectColor.Color == Dimension.Color.NONE)
                {
                    World.Instance.DeleteObject(this);
                }
            }
        }
        public Dictionary<Dimension.Color, SplittableObject> Siblings {
            get => siblings;
        }
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

        // Split the object into 3 dimension. Called by World.
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

        protected void SplitPersistent()
        {
            List<Dimension.Color> splittedColor;
            if (this.Color == Dimension.Color.BLACK)
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
                    World.Instance.DeleteObject(Siblings[sc]);
                    Siblings[sc] = null;
                }
            }
            World.Instance.DeleteObject(this);
        }

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
            MergeCollidedObjects(ref mergedColor, curSiblings);

            if (mergedColor == Dimension.Color.BLACK)
                MergeToBlack(curSiblings);
            else
                MergeToNewParent(mergedColor, curSiblings);
        }

        protected void MergeCollidedObjects(ref Dimension.Color mergedColor, List<SplittableObject> curSiblings)
        {
            Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 0.7f, transform.rotation);
            foreach (Collider c in colliders)
            {
                if (c == null || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;
                var so = c.gameObject.GetComponent<SplittableObject>();
                if (so == null || so.IsMerged)
                {
                    continue;
                }
                else if (so.Color == Dimension.Color.BLACK)
                {
                    mergedColor = Dimension.Color.BLACK;
                }
                else if (c.gameObject.name == gameObject.name && Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition))
                {
                    mergedColor = Dimension.AddColor(mergedColor, so.Color);
                    so.IsMerged = true;
                    curSiblings.Add(so);
                }
                else
                {
                    mergedColor = Dimension.Color.BLACK;
                    so.Merge(this);
                }
            }
        }

        protected void MergeToBlack(List<SplittableObject> curSiblings)
        {
            World.Instance.MoveObjectToDimension(this, Dimension.Color.WHITE);
            this.Color = Dimension.Color.BLACK;
            isPersistentColor = true;
            World.Instance.MoveToProcessed(this);
            for (int i = 0 ; i < curSiblings.Count; i++)
            {
                World.Instance.DeleteObject(curSiblings[i]);
            }
        }

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

        public bool IsInCorrectDim()
        {
            return this.Color == Dim.Color;
        }

    }


}