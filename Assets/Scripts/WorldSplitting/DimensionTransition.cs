using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class DimensionTransition : MonoBehaviour
    {

        [System.Serializable]
        class GlitchSetting
        {
            public Material mat;
            public Vector2 amount = new Vector2(0.01f, 0.1f);
            public Vector2 freq = new Vector2(0.5f, 1.5f);
            public Vector2 splitCnt = new Vector2(10f, 50f);
            public AnimationCurve curve;
        }

        [SerializeField] List<Transform> targetPos;
        [SerializeField] List<VisualEffect> vfx;
        [SerializeField] List<VisualEffect> borderVfx;
        [SerializeField] List<Material> soMaterials;
        [SerializeField] float dissolveRadiusMax = 20.0f;
        [SerializeField] float dissolveRadiusMin = -10.0f;
        [SerializeField] float transitionDuration = 1.0f;
        [SerializeField] Input.CameraController cameraController;
        [SerializeField] GlitchSetting glitchSetting;
        [SerializeField] UnityEngine.Events.UnityEvent OnTransitionStarted;

        bool transitting;
        Vector3 gravity = new Vector3(0f, -9.8f, 0f);

        public bool Transitting {
            get => transitting;
        }

        private void Awake()
        {
            transitting = false;
            for (int i = 0; i < soMaterials.Count; i++)
            {
                soMaterials[i].SetVector("_Dim1Pos", targetPos[0].position);
                soMaterials[i].SetVector("_Dim2Pos", targetPos[1].position);
                soMaterials[i].SetVector("_Dim3Pos", targetPos[2].position);
                soMaterials[i].SetFloat("_DissolveRadius", dissolveRadiusMax);
            }
        }

        public IEnumerator SplitTransition()
        {
            OnTransitionStart();

            cameraController.ZoomOut(transitionDuration/10);
            StartCoroutine(Glitch());
            PlayBorderVFX();
            PlayVFX();
            yield return StartCoroutine(MaterialAnim(dissolveRadiusMax, dissolveRadiusMin));
            ActualSplit();
            StopVFX();
            cameraController.FollowPlayer();
            cameraController.ZoomIn(transitionDuration/10);
            yield return StartCoroutine(MaterialAnim(dissolveRadiusMin, dissolveRadiusMax));
            cameraController.UnFollowPlayer();

            OnTransitionEnd();
        }

        public IEnumerator MergeTransition()
        {
            OnTransitionStart();

            cameraController.ZoomOut(transitionDuration/10);
            StartCoroutine(Glitch());
            StopBorderVFX();
            PlayVFX();
            yield return StartCoroutine(MaterialAnim(dissolveRadiusMax, dissolveRadiusMin));
            ActualMerge();
            StopVFX();
            cameraController.FollowPlayer();
            cameraController.ZoomIn(transitionDuration/10);
            yield return StartCoroutine(MaterialAnim(dissolveRadiusMin, dissolveRadiusMax));
            cameraController.UnFollowPlayer();

            OnTransitionEnd();
            yield return null;
        }

        void OnTransitionStart()
        {
            OnTransitionStarted.Invoke();
            Physics.gravity = Vector3.zero;
            Physics.autoSimulation = false;
            transitting = true;
            World.Instance.OnTransitionStart.Invoke();
            Input.InputManager.Instance.pause = true;
        }

        void OnTransitionEnd()
        {
            Physics.gravity = gravity;
            Physics.autoSimulation = true;
            transitting = false;
            World.Instance.OnTransitionEnd.Invoke();
            Input.InputManager.Instance.pause = false;
        }

        /// <summary>
        /// Physically splits of gameobjects.
        /// </summary>
        void ActualSplit()
        {
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].transform.position = targetPos[i].position;
                if (World.Instance.Dimensions[i].color != Dimension.Color.NONE)
                    World.Instance.Dimensions[i].gameObject.SetActive(true);
                else
                    World.Instance.Dimensions[i].gameObject.SetActive(false);
            }
            World.Instance.SplitObjects();
            Physics.SyncTransforms();
        }

        /// <summary>
        /// Physically merges the gameobjects.
        /// </summary>
        void ActualMerge()
        {
            for (int i = 0; i < targetPos.Count; i++)
                World.Instance.Dimensions[i].transform.position = World.Instance.ActiveDimension.transform.position;

            Physics.SyncTransforms();
            World.Instance.MergeObjects();

            for (int i = 0; i < targetPos.Count; i++)
                World.Instance.Dimensions[i].gameObject.SetActive(false);
            World.Instance.ActiveDimension.gameObject.SetActive(true);
        }

        /// <summary>
        /// Animates the dissolve radius of the material.
        /// </summary>
        /// <param name="startRadius"> The starting radius. </param>
        /// <param name="endRadius"> The ending radius. </param>
        /// <returns></returns>
        IEnumerator MaterialAnim(float startRadius, float endRadius)
        {
            float t = 0.0f;
            while (t < transitionDuration)
            {
                t += Time.deltaTime;
                foreach (var mat in soMaterials)
                    mat.SetFloat("_DissolveRadius", Mathf.Lerp(startRadius, endRadius, t / transitionDuration));
                yield return null;
            }
            foreach (var mat in soMaterials)
                mat.SetFloat("_DissolveRadius", endRadius);
        }

        /// <summary>
        /// Plays the vfx.
        /// </summary>
        void PlayVFX()
        {
            for (int i = 0; i < targetPos.Count; i++)
            {
                if (World.Instance.Dimensions[i].color != Dimension.Color.NONE)
                    vfx[i].Play();
            }
        }

        /// <summary>
        /// Stops the vfx.
        /// </summary>
        void StopVFX()
        {
            for (int i = 0; i < targetPos.Count; i++)
            {
                vfx[i].Stop();
            }
        }

        void PlayBorderVFX()
        {
            for (int i = 0; i < World.Instance.Dimensions.Count - 1; i++)
            {
                var c1 = World.Instance.Dimensions[i].color;
                var c2 = World.Instance.Dimensions[i+1].color;
                if (c1 != Dimension.Color.NONE && c2 != Dimension.Color.NONE)
                {
                    borderVfx[i].SetVector4("Color1", Dimension.MaterialColor[c1]);
                    borderVfx[i].SetVector4("Color2", Dimension.MaterialColor[c2]);
                    borderVfx[i].Play();
                }
            }
        }

        void StopBorderVFX()
        {
            for (int i = 0; i < borderVfx.Count; i++)
            {
                borderVfx[i].Stop();
            }
        }

        /// <summary>
        /// Animates the global glitch effect.
        /// </summary>
        /// <returns></returns>
        IEnumerator Glitch()
        {
            float t = 0.0f;
            glitchSetting.mat.SetFloat("_Transitting", 1.0f);
            while (t < transitionDuration)
            {
                t += Time.deltaTime;
                float p;
                if (t < transitionDuration / 2)
                {
                    p = t * 2 / transitionDuration;
                }
                else
                {
                    p = 2 - 2 * t / transitionDuration;
                }
                p = glitchSetting.curve.Evaluate(p);
                glitchSetting.mat.SetFloat("_GlitchAmount", Mathf.Lerp(glitchSetting.amount[0], glitchSetting.amount[1], p));
                glitchSetting.mat.SetFloat("_GlitchFrequency", Mathf.Lerp(glitchSetting.freq[0], glitchSetting.freq[1], p));
                glitchSetting.mat.SetFloat("_SplitCnt", Mathf.Lerp(glitchSetting.splitCnt[0], glitchSetting.splitCnt[1], p));
                yield return null;
            }
            glitchSetting.mat.SetFloat("_Transitting", 0.0f);
        }

        /// <summary>
        /// Splits the world without animation.
        /// </summary>
        public void DefaultSplit()
        {
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].transform.position = targetPos[i].transform.position;
                if (World.Instance.Dimensions[i].color != Dimension.Color.NONE)
                    World.Instance.Dimensions[i].gameObject.SetActive(true);
                else
                    World.Instance.Dimensions[i].gameObject.SetActive(false);
            }
            PlayBorderVFX();
        }

    }

}