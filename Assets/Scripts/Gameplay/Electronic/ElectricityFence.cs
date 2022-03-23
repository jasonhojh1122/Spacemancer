using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Electronic
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class ElectricityFence : ElectronicObject
    {
        [SerializeField] ParticleSystem ps;
        [SerializeField] List<LineRenderer> lineRenderers;
        [SerializeField] float lineWidth;
        [SerializeField] float explosionLength = 1.2f;

        Splittable.SplittableObject so;

        bool IsOn
        {
            get => isOn;
            set
            {
                isOn = value;
                ToggleLineRenderer(isOn);
                if (isOn)
                    UpdateLineRendererColor();
            }
        }

        private void Awake()
        {
            InitLineRenderer();
            so = GetComponent<Splittable.SplittableObject>();
            Core.World.Instance.OnTransitionEnd.AddListener(OnDimensionChange);
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
        }

        private void Start()
        {
            UpdateState();
        }

        void UpdateState()
        {
            if (so.IsInCorrectDim())
            {
                IsOn = true;
            }
            else
            {
                IsOn = false;
            }
        }

        void InitLineRenderer()
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                lr.positionCount = 2;
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3( Mathf.Abs(lr.transform.localPosition.x) * 2.0f, 0.0f, 0.0f));
            }
        }

        void ToggleLineRenderer(bool state)
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                lr.enabled = state;
            }
        }

        void UpdateLineRendererColor()
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                lr.GetPropertyBlock(block);
                block.SetColor("_LaserColor", Core.Dimension.MaterialColor[so.Color]);
                lr.SetPropertyBlock(block);
            }
        }

        public void OnZoneEnter(Collider other)
        {
            if (IsOn && other.gameObject.tag == "Player")
            {
                StartCoroutine(Explode());
            }
        }

        IEnumerator Explode()
        {
            ps.Play();
            Input.InputManager.Instance.pause = true;
            yield return new WaitForSeconds(explosionLength);
            SceneLoader.Instance.Reload();
        }

        public override void TurnOn()
        {
            if (!IsOn)
            {
                IsOn = true;
            }
        }

        public override void TurnOff()
        {
            if (IsOn)
            {
                IsOn = false;
            }
        }

        public override void Toggle()
        {
            if (!so.IsInCorrectDim()) return;
            if (IsOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
        }

        public override void OnColorChange()
        {
            UpdateState();
        }

        public override void OnDimensionChange()
        {
            UpdateState();
        }
    }

}
