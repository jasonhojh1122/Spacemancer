using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public class ObjectColor : MonoBehaviour
    {

        [SerializeField] Dimension.Color color;
        [SerializeField] bool usingMaterial = true;

        public UnityEvent OnWithdrew;
        public UnityEvent OnInserted;

        SplittableObject so;
        Renderer _renderer;
        MaterialPropertyBlock _propBlock;

        Dimension.Color selectColor;
        Dimension.Color secondColor;
        static float MixDur = 0.5f;
        static float defaultMixDistMax = 15f;
        float mixDistMax;
        float selectDistStart;
        float selectDistEnd;
        float secondDistStart;
        float secondDistEnd;
        bool selected;

        /// <summary>
        /// The color of the object.
        /// Set the color of the object will immediately updates the material.
        /// </summary>
        public Dimension.Color Color
        {
            get => color;
            set {
                color = value;
                if (!usingMaterial) return;
                if (color == Dimension.Color.NONE)
                {
                    _propBlock.SetFloat("_Error", 0);
                    _propBlock.SetFloat("_Dissolve", 1);
                }
                else
                {
                    _propBlock.SetFloat("_MainColor", (float)color);
                    _propBlock.SetFloat("_Dissolve", 0);
                    if (color != so.Dim.color)
                    {
                        _propBlock.SetFloat("_Error", 1);
                    }
                    else
                    {
                        _propBlock.SetFloat("_Error", 0);
                    }
                }
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

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
            if (usingMaterial)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                Reset();
            }
            var col = GetComponent<Collider>();
            if (col != null)
            {
                mixDistMax = Mathf.Max(Mathf.Max(col.bounds.size.x, col.bounds.size.y), col.bounds.size.z) * 1.5f;
            }
            else
            {
                mixDistMax = defaultMixDistMax;
            }
            selected = false;
        }

        public void Select(Vector3 contactPos)
        {
            selected = true;
            StopAllCoroutines();
            StartCoroutine(SelectAnim(contactPos));
            Debug.Log("select");
        }

        public void Unselect(Vector3 contactPos)
        {
            selected = false;
            StopAllCoroutines();
            StartCoroutine(UnselectAnim(contactPos));
            Debug.Log("unselect");
        }

        public void Insert(Vector3 contactPos)
        {
            selected = true;
            StopAllCoroutines();
            StartCoroutine(InsertAnim(contactPos));
            Debug.Log("insert");
        }

        public void Withdraw(Vector3 contactPos)
        {
            selected = false;
            StopAllCoroutines();
            StartCoroutine(WithdrawAnim(contactPos));
            Debug.Log("withdraw");
        }

        System.Collections.IEnumerator SelectAnim(Vector3 contactPos)
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector("_ContactPos", contactPos);
            _propBlock.SetFloat("_SelectDistStart", 0.0f);
            _renderer.SetPropertyBlock(_propBlock);
            float t = Mathf.Lerp(0.0f, MixDur, Mathf.InverseLerp(0.0f, mixDistMax, selectDistEnd));
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                selectDistEnd = Mathf.Lerp(startSelect, mixDistMax, t/MixDur);
                SetMaterialDist();
                t += Time.deltaTime;
                yield return null;
            }
        }

        System.Collections.IEnumerator UnselectAnim(Vector3 contactPos)
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector("_ContactPos", contactPos);
            _propBlock.SetFloat("_SelectDistStart", 0.0f);
            _renderer.SetPropertyBlock(_propBlock);

            float t = Mathf.Lerp(0.0f, MixDur, Mathf.InverseLerp(mixDistMax, 0.0f, selectDistEnd));
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                selectDistEnd = Mathf.Lerp(startSelect, 0.0f, t/MixDur);
                SetMaterialDist();
                t += Time.deltaTime;
                yield return null;
            }
        }

        System.Collections.IEnumerator InsertAnim(Vector3 contactPos)
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector("_ContactPos", contactPos);
            _propBlock.SetFloat("_SelectDistStart", 0.0f);
            _propBlock.SetFloat("_SecondDistStart", 0.0f);
            _renderer.SetPropertyBlock(_propBlock);

            float t = 0.0f;
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                float p = t/MixDur;
                secondDistEnd = Mathf.Lerp(0.0f, mixDistMax, p);
                selectDistStart = Mathf.Lerp(0.0f, mixDistMax, p);
                selectDistEnd = Mathf.Lerp(startSelect, mixDistMax, p);
                SetMaterialDist();
                t += Time.deltaTime;
                yield return null;
            }
            OnInserted.Invoke();
        }

        System.Collections.IEnumerator WithdrawAnim(Vector3 contactPos)
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetVector("_ContactPos", contactPos);
            _propBlock.SetFloat("_SelectDistStart", 0.0f);
            _propBlock.SetFloat("_SecondDistEnd", mixDistMax);
            _renderer.SetPropertyBlock(_propBlock);

            float t = 0.0f;
            float startSelect = selectDistEnd;
            while (t < MixDur)
            {
                float p = t/MixDur;
                secondDistEnd = Mathf.Lerp(mixDistMax, 0.0f, p);
                selectDistEnd = Mathf.Lerp(startSelect, 0.0f, p);
                SetMaterialDist();
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
            SetMaterialDist();
        }

        void SetMaterialDist()
        {
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_SelectDistStart", selectDistStart);
            _propBlock.SetFloat("_SelectDistEnd", selectDistEnd);
            _propBlock.SetFloat("_SecondDistStart", secondDistStart);
            _propBlock.SetFloat("_SecondDistEnd", secondDistEnd);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

}