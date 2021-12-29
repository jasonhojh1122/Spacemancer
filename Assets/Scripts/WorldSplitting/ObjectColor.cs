using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class ObjectColor : MonoBehaviour
    {

        [SerializeField] Dimension.Color color = Dimension.Color.WHITE;
        [SerializeField] bool usingMaterial = true;

        [HideInInspector] public UnityEvent OnWithdrew;
        [HideInInspector] public UnityEvent OnInserted;
        [HideInInspector] public UnityEvent OnColorChanged;

        SplittableObject so;
        Renderer _renderer;
        MaterialPropertyBlock _propBlock;
        Vector3 contactPos;
        Dimension.Color selectColor;
        Dimension.Color secondColor;
        static float MixDur = 0.5f;
        static float defaultMixDistMax = 20.0f;
        float mixDistMax;
        float selectDistStart;
        float selectDistEnd;
        float secondDistStart;
        float secondDistEnd;
        float dissolveDistStart;
        float dissolveDistEnd;

        /// <summary>
        /// The color of the object.
        /// Set the color of the object will immediately updates the material.
        /// </summary>
        public Dimension.Color Color
        {
            get => color;
            set {
                color = value;
                OnColorChanged.Invoke();
                if (!usingMaterial) return;
                if (color != Dimension.Color.NONE)
                {
                    _renderer.GetPropertyBlock(_propBlock);
                    _propBlock.SetFloat("_MainColor", (float)color);
                    if (color != so.Dim.color)
                        _propBlock.SetFloat("_Error", 1);
                    else
                        _propBlock.SetFloat("_Error", 0);
                    _renderer.SetPropertyBlock(_propBlock);
                }
            }
        }

        /// <summary>
        /// The color of the laser selection.
        /// </summary>
        public Dimension.Color SelectColor
        {
            get => selectColor;
            set {
                selectColor = value;
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_SelectColor", (float)selectColor);
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        /// <summary>
        /// The secondary color at insertion and withdrawal of color.
        /// </summary>
        public Dimension.Color SecondColor
        {
            get => secondColor;
            set {
                secondColor = value;
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_SecondColor", (float)SecondColor);
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            var col = GetComponent<Collider>();
            if (col != null)
            {
                mixDistMax = Mathf.Max(Mathf.Max(col.bounds.size.x, col.bounds.size.y), col.bounds.size.z) * 2.0f;
            }
            else
            {
                mixDistMax = defaultMixDistMax;
            }
            if (usingMaterial)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                Reset();
            }
        }

        private void OnEnable() {
            if (usingMaterial)
                Reset();
        }

        /// <summary>
        /// Shows the selected hinting.
        /// </summary>
        /// <param name="contactPos"> The laser contact point. </param>
        public void Select(Vector3 contactPos)
        {
            StopAllCoroutines();
            this.contactPos = contactPos;
            StartCoroutine(SelectAnim());
        }

        /// <summary>
        /// Hides the selected hinting.
        /// </summary>
        /// <param name="contactPos"> The laser contact point. </param>
        public void Unselect(Vector3 contactPos)
        {
            StopAllCoroutines();
            this.contactPos = contactPos;
            StartCoroutine(UnselectAnim());
        }

        /// <summary>
        /// Shows the inserted secondary color.
        /// </summary>
        /// <param name="contactPos"> The laser contact point. </param>
        public void Insert(Vector3 contactPos)
        {
            StopAllCoroutines();
            this.contactPos = contactPos;
            StartCoroutine(InsertAnim());
        }

        /// <summary>
        /// Shows the new color after withdrawal.
        /// </summary>
        /// <param name="contactPos"> The laser contact point. </param>
        public void Withdraw(Vector3 contactPos)
        {
            StopAllCoroutines();
            this.contactPos = contactPos;
            if (Color == Dimension.Color.NONE)
                StartCoroutine(WithdrawAnimDissolve());
            else
                StartCoroutine(WithdrawAnim());
        }

        System.Collections.IEnumerator SelectAnim()
        {
            selectDistStart = 0.0f;
            UpdateMaterialProp();
            float t = Mathf.Lerp(0.0f, MixDur, Mathf.InverseLerp(0.0f, mixDistMax, selectDistEnd));
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                selectDistEnd = Mathf.Lerp(startSelect, mixDistMax, t/MixDur);
                UpdateMaterialProp();
                t += Time.deltaTime;
                yield return null;
            }
        }

        System.Collections.IEnumerator UnselectAnim()
        {
            selectDistStart = 0.0f;
            UpdateMaterialProp();
            float t = Mathf.Lerp(0.0f, MixDur, Mathf.InverseLerp(mixDistMax, 0.0f, selectDistEnd));
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                selectDistEnd = Mathf.Lerp(startSelect, 0.0f, t/MixDur);
                UpdateMaterialProp();
                t += Time.deltaTime;
                yield return null;
            }
        }

        System.Collections.IEnumerator InsertAnim()
        {
            selectDistStart = 0.0f;
            secondDistStart = 0.0f;
            UpdateMaterialProp();
            float t = 0.0f;
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                float p = t/MixDur;
                secondDistEnd = Mathf.Lerp(0.0f, mixDistMax, p);
                selectDistStart = Mathf.Lerp(0.0f, mixDistMax, p);
                selectDistEnd = Mathf.Lerp(startSelect, mixDistMax, p);
                UpdateMaterialProp();
                t += Time.deltaTime;
                yield return null;
            }
            OnInserted.Invoke();
        }

        System.Collections.IEnumerator WithdrawAnim()
        {
            selectDistStart = 0.0f;
            secondDistEnd = mixDistMax;
            UpdateMaterialProp();
            float t = 0.0f;
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                float p = t/MixDur;
                secondDistEnd = Mathf.Lerp(mixDistMax, 0.0f, p);
                selectDistEnd = Mathf.Lerp(startSelect, 0.0f, p);
                UpdateMaterialProp();
                t += Time.deltaTime;
                yield return null;
            }
            OnWithdrew.Invoke();
        }

        System.Collections.IEnumerator WithdrawAnimDissolve()
        {
            selectDistStart = 0.0f;
            dissolveDistEnd = 100.0f;
            secondDistEnd = mixDistMax;
            UpdateMaterialProp();
            float t = 0.0f;
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                float p = t/MixDur;
                secondDistEnd = Mathf.Lerp(mixDistMax, 0.0f, p);
                selectDistEnd = Mathf.Lerp(startSelect, 0.0f, p);
                dissolveDistStart = Mathf.Lerp(mixDistMax, 0.0f, p);
                UpdateMaterialProp();
                t += Time.deltaTime;
                yield return null;
            }
            OnWithdrew.Invoke();
        }

        public void Init()
        {
            Color = color;
        }

        public void Reset()
        {
            selectDistStart = 0.0f;
            selectDistEnd = 0.0f;
            secondDistStart = 0.0f;
            secondDistEnd = 0.0f;
            dissolveDistStart = mixDistMax;
            dissolveDistEnd = -1.0f;
            contactPos = gameObject.transform.position;
            UpdateMaterialProp();
        }

        void UpdateMaterialProp()
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_SelectDistStart", selectDistStart);
            _propBlock.SetFloat("_SelectDistEnd", selectDistEnd);
            _propBlock.SetFloat("_SecondDistStart", secondDistStart);
            _propBlock.SetFloat("_SecondDistEnd", secondDistEnd);
            _propBlock.SetFloat("_DissolveDistStart", dissolveDistStart);
            _propBlock.SetFloat("_DissolveDistEnd", dissolveDistEnd);
            _propBlock.SetVector("_ContactPos", contactPos);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

}