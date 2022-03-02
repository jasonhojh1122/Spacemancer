using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Electronic
{
    [RequireComponent(typeof(Splittable.SplittableObject))]
    public class ElectricityFence : ElectronicObject
    {
        [SerializeField] bool defaultOn;
        [SerializeField] ParticleSystem ps;
        [SerializeField] AudioSource audioSource;
        [SerializeField] List<LineRenderer> lineRenderers;
        [SerializeField] float lineWidth;

        Splittable.SplittableObject so;
        float explosionLength;

        bool curOn;

        private void Awake()
        {
            explosionLength = Mathf.Max(audioSource.clip.length, ps.main.duration) - 0.3f;
            curOn = defaultOn;
            InitLineRenderer();
            so = GetComponent<Splittable.SplittableObject>();
            Core.World.Instance.OnTransitionEnd.AddListener(CheckFences);
            so.ObjectColor.OnColorChanged.AddListener(CheckFences);
            so.OnInitialized.AddListener(CheckFences);
        }

        void InitLineRenderer()
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                lr.positionCount = 2;
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3(0.0f, 0.0f, Mathf.Abs(lr.transform.localPosition.z)*2.0f));
            }
        }

        void ToggleLineRenderer(bool state)
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                lr.enabled = state;
            }
        }

        void CheckFences()
        {
            if (so.IsInCorrectDim() && curOn)
            {
                ToggleLineRenderer(true);
                UpdateLineRendererColor();
            }
            else
            {
                ToggleLineRenderer(false);
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

        void OnTriggerEnter(Collider col)
        {
            if (curOn && col.gameObject.tag == "Player")
            {
                StartCoroutine(Explode());
            }
        }

        IEnumerator Explode()
        {
            ps.Play();
            audioSource.Play();
            Input.InputManager.Instance.pause = true;
            yield return new WaitForSeconds(explosionLength);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public override void TurnOn()
        {
            if (so.IsInCorrectDim())
                curOn = true;
        }

        public override void TurnOff()
        {
            curOn = false;
        }

        public override void OnColorChange()
        {
            CheckFences();
        }

        public override void OnDimensionChange()
        {
            CheckFences();
        }
    }

}
