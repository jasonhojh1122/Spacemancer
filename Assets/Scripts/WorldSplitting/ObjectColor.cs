using System;
using UnityEngine;

namespace Core
{
    public class ObjectColor : MonoBehaviour
    {

        [SerializeField] Dimension.Color color;

        SplittableObject so;
        Renderer _renderer;
        MaterialPropertyBlock _propBlock;
        Collider col;

        public Dimension.Color Color
        {
            get => color;
            set {
                color = value;

                if (color == Dimension.Color.NONE)
                {
                    _propBlock.SetFloat("_Error", 0);
                    _propBlock.SetFloat("_Dissolve", 1);
                }
                else
                {
                    _propBlock.SetColor("_Color", Dimension.MaterialColor[color]);
                    _propBlock.SetFloat("_Dissolve", 0);
                    if (color != so.Dim.GetColor())
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
            _propBlock = new MaterialPropertyBlock();
            so = GetComponent<SplittableObject>();
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(_propBlock);
            col = GetComponent<Collider>();
        }

        public void Init()
        {
            Color = color;
        }
    }

}