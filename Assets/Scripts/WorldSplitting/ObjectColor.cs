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
                    _propBlock.SetColor("_Color", Dimension.MaterialColor[color]);
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

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            if (usingMaterial)
            {
                _propBlock = new MaterialPropertyBlock();
                _renderer = GetComponent<Renderer>();
                _renderer.GetPropertyBlock(_propBlock);
            }
        }

        public void Init()
        {
            if (usingMaterial)
                Color = color;
        }
    }

}