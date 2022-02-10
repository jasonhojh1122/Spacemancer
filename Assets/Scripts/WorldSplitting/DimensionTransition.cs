using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] float transitionDuration = 1.0f;
        [SerializeField] float endPause = 0.2f;
        [SerializeField] GlitchSetting glitchSetting;

        bool transitting;

        public bool Transitting {
            get => transitting;
        }

        private void Awake()
        {
            transitting = false;
        }

        public IEnumerator SplitTransition()
        {
            OnTransitionStartEnd(true);
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].transform.position = targetPos[i].transform.position;
                World.Instance.Dimensions[i].gameObject.SetActive(true);
            }
            World.Instance.SplitObjects();
            Physics.SyncTransforms();
            OnTransitionStartEnd(false);
            yield return new WaitForSeconds(1);
            World.Instance.OnDimensionChange.Invoke();
            yield return null;
        }

        public IEnumerator MergeTransition()
        {
            OnTransitionStartEnd(true);
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].transform.position = World.Instance.ActiveDimension.transform.position;
            }
            Physics.SyncTransforms();
            World.Instance.MergeObjects();
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].gameObject.SetActive(false);
            }
            World.Instance.ActiveDimension.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            OnTransitionStartEnd(false);
            World.Instance.OnDimensionChange.Invoke();
            yield return null;
        }

        void OnTransitionStartEnd(bool isStart)
        {
            // playerController.pause = !isStart;
            Physics.gravity = isStart ? Vector3.zero : new Vector3(0f, -9.8f, 0f);
            Physics.autoSimulation = !isStart;
            transitting = isStart;
        }

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

        public void DefaultSplit()
        {
            for (int i = 0; i < targetPos.Count; i++)
            {
                World.Instance.Dimensions[i].transform.position = targetPos[i].transform.position;
                World.Instance.Dimensions[i].gameObject.SetActive(true);
            }
            Physics.SyncTransforms();
        }

    }

}