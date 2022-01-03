using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    public class ElectricityFence : MonoBehaviour
    {
        [SerializeField] Core.SplittableObject fence;
        [SerializeField] ParticleSystem ps;
        [SerializeField] AudioSource audioSource;
        [SerializeField] List<LineRenderer> lineRenderers;
        [SerializeField] float lineWidth;

        float explosionLength;

        bool curOn;

        private void Awake()
        {
            explosionLength = Mathf.Min(audioSource.clip.length, ps.main.duration);
            curOn = true;
            InitLineRenderer();
            Core.World.Instance.BeforeSplit.AddListener(CheckFences);
            Core.World.Instance.BeforeMerge.AddListener(CheckFences);
            fence.ObjectColor.OnColorChanged.AddListener(CheckFences);
            fence.OnInitialized.AddListener(CheckFences);
        }

        void CheckFences()
        {
            if (fence.IsInCorrectDim())
            {
                curOn = true;
                ToggleLineRenderer(true);
                UpdateLineRendererColor();
            }
            else
            {
                curOn = false;
                ToggleLineRenderer(false);
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

        void UpdateLineRendererColor()
        {
            foreach (LineRenderer lr in lineRenderers)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                lr.GetPropertyBlock(block);
                block.SetColor("_LaserColor", Core.Dimension.MaterialColor[fence.Color]);
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
            yield return new WaitForSeconds(explosionLength);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}
