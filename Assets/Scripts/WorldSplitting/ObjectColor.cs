using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class ObjectColor : MonoBehaviour
    {

        [SerializeField] protected Dimension.Color color = Dimension.Color.WHITE;
        [SerializeField] protected bool usingMaterial = true;
        [SerializeField] ObjectColor root;
        [SerializeField] List<ObjectColor> group;

        /// <summary>
        /// Called after the withdrawing animation is finished.
        /// </summary>
        [HideInInspector] public UnityEvent OnWithdrew;

        /// <summary>
        /// Called after the inserting animation is finished.
        /// </summary>
        [HideInInspector] public UnityEvent OnInserted;

        /// <summary>
        /// Called when the color is changed.
        /// </summary>
        [HideInInspector] public UnityEvent OnColorChanged;

        Animator animator;
        Renderer _renderer;
        protected MaterialPropertyBlock _propBlock;
        protected Vector3 contactPoint;
        protected Dimension.Color dimensionColor;
        protected Dimension.Color selectColor;
        protected Dimension.Color secondColor;
        bool isRoot;
        bool dirty;

        /// <summary>
        /// The color of the object.
        /// Set the color of the object will immediately updates the material.
        /// </summary>
        public Dimension.Color Color
        {
            get => color;
            set
            {
                if (isRoot)
                    foreach(var oc in group)
                        oc.SetColor(value);
                else
                    root.Color = value;
            }
        }

        public void SetColor(Dimension.Color value)
        {
            color = value;
            dirty = true;
            OnColorChanged.Invoke();
        }

        /// <summary>
        /// The color of the laser selection.
        /// </summary>
        public virtual Dimension.Color SelectColor
        {
            get => selectColor;
            set
            {
                if (isRoot)
                    foreach(var oc in group)
                        oc.SetSelectColor(value);
                else
                    root.SelectColor = value;
            }
        }

        public void SetSelectColor(Dimension.Color value)
        {
            selectColor = value;
            dirty = true;
        }

        /// <summary>
        /// The secondary color at insertion and withdrawal of color.
        /// </summary>
        public virtual Dimension.Color SecondColor
        {
            get => secondColor;
            set
            {
                if (isRoot)
                    foreach(var oc in group)
                        oc.SetSecondColor(value);
                else
                    root.SecondColor = value;
            }
        }

        public void SetSecondColor(Dimension.Color value)
        {
            secondColor = value;
            dirty = true;
        }

        public Dimension.Color DimensionColor {
            get => dimensionColor;
            set
            {
                if (isRoot)
                    foreach(var oc in group)
                        oc.SetDimensionColor(value);
                else
                    root.DimensionColor = value;
            }
        }

        public void SetDimensionColor(Dimension.Color value)
        {
            Debug.Log(gameObject.name + " " + transform.GetInstanceID() + " set dim " + value.ToString());
            dimensionColor = value;
            dirty = true;
        }

        public Vector3 ContactPoint {
            get => contactPoint;
            set
            {
                if (isRoot)
                    foreach(var oc in group)
                        oc.SetContactPoint(value);
                else
                    root.ContactPoint = value;
            }
        }

        public void SetContactPoint(Vector3 value)
        {
            dirty = true;
            contactPoint = value;
        }

        public bool IsRoot {
            get => isRoot;
        }

        public ObjectColor Root {
            get => root;
        }

        public Animator Animator {
            get => animator;
        }

        protected void Awake()
        {
            isRoot = (root == null);
            if (isRoot)
            {
                if (group == null)
                    group = new List<ObjectColor>();
                group.Add(this);
            }
            if (usingMaterial)
            {
                animator = GetComponent<Animator>();
                _renderer = GetComponent<Renderer>();
                _propBlock = new MaterialPropertyBlock();
            }
            OnColorChanged = new UnityEvent();
            dirty = false;
        }

        private void Update()
        {
            if (dirty && usingMaterial)
            {
                dirty = false;
                UpdateProperties();
            }
        }

        void UpdateProperties()
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_MainColor", (float)color);
            if (color != DimensionColor)
                _propBlock.SetFloat("_Error", 1);
            else
                _propBlock.SetFloat("_Error", 0);
            _propBlock.SetFloat("_SelectColor", (float)SelectColor);
            _propBlock.SetFloat("_SecondColor", (float)SecondColor);
            _propBlock.SetVector("_ContactPos", contactPoint);
            _renderer.SetPropertyBlock(_propBlock);
        }

        /// <summary>
        /// Shows the selected hinting.
        /// </summary>
        public void Select()
        {
            if (isRoot)
                foreach(var oc in group)
                    oc.Animator.SetTrigger("Select");
            else
                root.Select();
        }

        /// <summary>
        /// Hides the selected hinting.
        /// </summary>
        /// <param name="contactPoint"> The laser contact point. </param>
        public void Unselect()
        {
            if (isRoot)
                foreach(var oc in group)
                    oc.Animator.SetTrigger("Unselect");
            else
                root.Unselect();
        }

        /// <summary>
        /// Shows the inserted secondary color.
        /// </summary>
        public void Insert()
        {
            if (isRoot)
                foreach(var oc in group)
                    oc.Animator.SetTrigger("Insert");
            else
                root.Insert();
        }

        /// <summary>
        /// Shows the new color after withdrawal.
        /// </summary>
        public void Withdraw()
        {
            if (isRoot)
                foreach(var oc in group)
                {
                    if (Color == Dimension.Color.NONE)
                        oc.Animator.SetTrigger("Dissolve");
                    else
                        oc.Animator.SetTrigger("Withdraw");
                }
            else
                root.Withdraw();
        }

        public void OnWithdrawAnimFinished()
        {
            OnWithdrew.Invoke();
        }

        public void OnInsertAnimFinished()
        {
            OnInserted.Invoke();
        }

    }
}