
using UnityEngine;
using System.Collections.Generic;


namespace Core
{

    [RequireComponent(typeof(ObjectColor))]
    public class SplittableObject : MonoBehaviour
    {

        [SerializeField] protected bool persistentColor;
        [SerializeField] protected bool defaultInactive;

        protected ObjectColor objectColor;
        protected World world;
        protected Collider col;
        protected ObjectColor dimension;
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
        public Dictionary<Dimension.Color, SplittableObject> Siblings {
            get => siblings;
        }

        void Awake()
        {
            col = GetComponent<Collider>();
            objectColor = GetComponent<ObjectColor>();
            world = FindObjectOfType<World>();
            siblings = new Dictionary<Dimension.Color, SplittableObject>();
            foreach (Dimension.Color bc in Dimension.BaseColor)
                Siblings.Add(bc, null);
            IsMerged = false;
        }

        protected void OnDisable()
        {
            // world.RemoveFromSet(this);
        }

        public virtual void Split()
        {
            if (Dim.Color != Dimension.Color.WHITE)
            {
                world.MoveToProcessed(this);
                return;
            }
            else if (persistentColor)
                SplitPersistent();
            else
                SplitColor();
        }

        protected void SplitPersistent()
        {
            foreach (Dimension.Color sc in Dimension.BaseColor)
            {
                if (Siblings[sc] == null)
                {
                    var so = world.InstantiateNewObjectToDimension(this, sc);
                    so.ObjectColor.Color = ObjectColor.Color;
                    Siblings[sc] = so;
                    world.RemoveFromSet(so);
                }
                else
                {
                    Siblings[sc].transform.localPosition = transform.localPosition;
                }
            }
            world.RemoveFromSet(this);
        }

        protected void SplitColor()
        {
            var splittedColor = Dimension.SplitColor(ObjectColor.Color);
            var missingColor = Dimension.MissingColor(ObjectColor.Color);
            foreach (Dimension.Color sc in splittedColor)
            {
                if (Siblings[sc] == null)
                {
                    Siblings[sc] = world.InstantiateNewObjectToDimension(this, sc);
                    Siblings[sc].ObjectColor.Color = sc;
                }
                else
                {
                    Siblings[sc].transform.localPosition = transform.localPosition;
                    Siblings[sc].IsMerged = false;
                }
                world.MoveToProcessed(Siblings[sc]);
            }

            foreach (Dimension.Color sc in missingColor)
            {
                if (Siblings[sc] != null)
                {
                    world.DeleteObject(Siblings[sc]);
                    Siblings[sc] = null;
                }
            }
            world.DeleteObject(this);
        }

        public virtual void Merge(SplittableObject parent) {
            if (persistentColor)
            {
                world.MoveToProcessed(this);
                return;
            }

            IsMerged = true;

            Dimension.Color mergedColor = ObjectColor.Color;

            if (parent != null && parent.ObjectColor.Color == Dimension.Color.BLACK)
                mergedColor = Dimension.Color.BLACK;

            List<SplittableObject> curSiblings = new List<SplittableObject>();
            Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 0.7f, transform.rotation);

            foreach (Collider c in colliders)
            {
                if (c == null || c.gameObject.GetInstanceID() == col.gameObject.GetInstanceID()) continue;

                var so = c.gameObject.GetComponent<SplittableObject>();

                if (so == null || so.IsMerged)
                {
                    continue;
                }
                else if (so.ObjectColor.Color == Dimension.Color.BLACK)
                {
                    mergedColor = Dimension.Color.BLACK;
                }
                else if (c.gameObject.name == gameObject.name && Fuzzy.CloseVector3(c.transform.localPosition, transform.localPosition))
                {
                    mergedColor = Dimension.AddColor(mergedColor, so.ObjectColor.Color);
                    so.IsMerged = true;
                    curSiblings.Add(so);
                }
                else
                {
                    mergedColor = Dimension.Color.BLACK;
                    so.Merge(this);
                }
            }
            if (mergedColor == Dimension.Color.BLACK)
                MergeToBlack(curSiblings);
            else
                MergeToNewParent(mergedColor, curSiblings);
        }

        protected void MergeToBlack(List<SplittableObject> curSiblings)
        {
            world.MoveObjectToDimension(this, Dimension.Color.WHITE);
            this.ObjectColor.Color = Dimension.Color.BLACK;
            persistentColor = true;
            world.MoveToProcessed(this);

            for (int i = 0 ; i < curSiblings.Count; i++)
            {
                world.DeleteObject(curSiblings[i]);
            }
        }

        protected void MergeToNewParent(Dimension.Color mergedColor, List<SplittableObject> curSiblings)
        {
            var parent = world.InstantiateNewObjectToDimension(this, Dimension.Color.WHITE);
            parent.ObjectColor.Color = mergedColor;
            world.MoveToProcessed(parent);

            curSiblings.Add(this);

            var splittedColor = Dimension.SplitColor(mergedColor);
            var missingColor = Dimension.MissingColor(mergedColor);

            for (int i = 0; i < curSiblings.Count; i++)
            {
                world.MoveObjectToDimension(curSiblings[i], splittedColor[i]);
                curSiblings[i].ObjectColor.Color = splittedColor[i];
                parent.Siblings[splittedColor[i]] = curSiblings[i];
                world.MoveToProcessed(curSiblings[i]);
            }
            for (int i = 0; i < missingColor.Count; i++)
            {
                parent.Siblings[missingColor[i]] = null;
            }

            /* if (gameObject.name == "Box")
            {
                Debug.Log(gameObject.name + " " + parent.transform.GetInstanceID() + " siblings: ");
                foreach (Dimension.Color color in Dimension.BaseColor)
                {
                    if (parent.Siblings[color] != null)
                    {
                        Debug.Log(parent.siblings[color].transform.GetInstanceID());
                    }
                }
            } */
        }

        public bool IsInCorrectDim()
        {
            return ObjectColor.Color == Dim.Color;
        }

    }


}