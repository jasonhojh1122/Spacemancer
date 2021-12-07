using System;
using UnityEngine;

namespace Core
{
    public class ObjectColor : MonoBehaviour
    {

        [SerializeField] Dimension.Color color;
        [SerializeField] bool usingMaterial = true;
        SplittableObject so;
        Renderer _renderer;
        MaterialPropertyBlock _propBlock;

        static float selectMaxSpreadDist = 15.0f;
        static float selectSpreadSpeed = 10.0f;
        float selectSpreadDist = 0.0f;
        Dimension.Color selectColor;
        bool selected;

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
                    _propBlock.SetColor("_MainColor", Dimension.MaterialColor[color]);
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
            set => selectColor = value;
        }

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            if (usingMaterial)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetVector("_ContactPos", Vector3.positiveInfinity);
                _renderer.SetPropertyBlock(_propBlock);
            }
            selected = false;
        }

        private void Update()
        {
            if (selected)
            {
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetColor("_SelectColor", Dimension.MaterialColor[selectColor]);
                _renderer.SetPropertyBlock(_propBlock);
            }
        }

        public void Init()
        {
            if (usingMaterial)
                Color = color;
        }

        public void SkillSelect(Vector3 contactPos)
        {
            selected = true;
            StopAllCoroutines();
            StartCoroutine(SkillSelectAnim(contactPos));
        }

        public void SkillUnselect(Vector3 contactPos)
        {
            selected = false;
            StopAllCoroutines();
            StartCoroutine(SkillUnselectAnim(contactPos));
        }

        System.Collections.IEnumerator SkillSelectAnim(Vector3 contactPos)
        {
            while (selectSpreadDist < selectMaxSpreadDist)
            {
                selectSpreadDist += selectSpreadSpeed * Time.deltaTime;
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_SelectSpreadDistance", selectSpreadDist);
                _propBlock.SetVector("_ContactPos", contactPos);
                _renderer.SetPropertyBlock(_propBlock);
                yield return null;
            }
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_SelectSpreadDistance", selectMaxSpreadDist);
            _renderer.SetPropertyBlock(_propBlock);
        }

        System.Collections.IEnumerator SkillUnselectAnim(Vector3 contactPos)
        {
            while (selectSpreadDist > 0)
            {
                selectSpreadDist -= selectSpreadSpeed * Time.deltaTime;
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_SelectSpreadDistance", selectSpreadDist);
                _propBlock.SetVector("_ContactPos", contactPos);
                _renderer.SetPropertyBlock(_propBlock);
                yield return null;
            }
            _renderer.GetPropertyBlock(_propBlock);
            _propBlock.SetFloat("_SelectSpreadDistance", 0.0f);
            _renderer.SetPropertyBlock(_propBlock);
        }
    }

}