using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using VecDic = System.Collections.Generic.Dictionary<Core.Dimension.Color, UnityEngine.Vector3>;
using QuaDic = System.Collections.Generic.Dictionary<Core.Dimension.Color, UnityEngine.Quaternion>;

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

        [SerializeField] Character.PlayerController playerController;
        [SerializeField] List<Transform> targetPos;
        [SerializeField] float transitionDuration = 1.0f;
        [SerializeField] float endPause = 0.2f;
        [SerializeField] GlitchSetting glitchSetting;

        List<Dimension.Color> dimensionOrder;
        bool transitting;

        public bool Transitting {
            get => transitting;
        }

        public Dimension.Color ActiveDimensionColor {
            get => dimensionOrder[0];
        }

        private void Awake()
        {
            dimensionOrder = new List<Dimension.Color>();
            foreach (Dimension.Color color in Dimension.BaseColor)
            {
                dimensionOrder.Add(color);
            }
        }

        public IEnumerator SplitTransition()
        {
            OnTransitionStartEnd(true);
            StartCoroutine(Glitch());
            World.Instance.SplitObjects();
            ToggleDimensionActivation(true);
            yield return StartCoroutine(MoveAnimation(false));
            World.Instance.Dims[Dimension.Color.WHITE].gameObject.SetActive(false);
            Physics.SyncTransforms();
            OnTransitionStartEnd(false);
            yield return new WaitForSeconds(endPause);
        }

        public IEnumerator MergeTransition()
        {
            OnTransitionStartEnd(true);
            StartCoroutine(Glitch());
            yield return StartCoroutine(MoveAnimation(true));
            Physics.SyncTransforms();
            World.Instance.MergeObjects();
            World.Instance.Dims[Dimension.Color.WHITE].gameObject.SetActive(true);
            ToggleDimensionActivation(false);
            OnTransitionStartEnd(false);
            yield return new WaitForSeconds(endPause);
        }

        public IEnumerator RotateTransition(int direction)
        {
            OnTransitionStartEnd(true);
            StartCoroutine(Glitch());
            yield return StartCoroutine(RotateAnimation(direction));
            OnTransitionStartEnd(false);
            yield return new WaitForSeconds(endPause);
        }

        IEnumerator MoveAnimation(bool ToCenter)
        {
            float t = 0.0f;
            while (t < transitionDuration)
            {
                t += Time.deltaTime;
                for (int i = 0; i < dimensionOrder.Count; i++)
                {
                    Transform dim = World.Instance.Dims[dimensionOrder[i]].transform;
                    Vector3 start, end;
                    if (ToCenter)
                    {
                        start = targetPos[i].position;
                        end = World.Instance.Dims[Dimension.Color.WHITE].transform.position;
                    }
                    else
                    {
                        start = World.Instance.Dims[Dimension.Color.WHITE].transform.position;
                        end = targetPos[i].position;
                    }
                    dim.position = Vector3.Lerp(start, end, t / transitionDuration);
                }
                yield return null;
            }
        }

        IEnumerator RotateAnimation(int direction)
        {
            Dictionary<Dimension.Color, int> newOrder = new Dictionary<Dimension.Color, int>();
            for (int i = 0; i < dimensionOrder.Count; i++)
            {
                if (direction > 0)
                {
                    int n = (i-1) < 0 ? (i-1) + dimensionOrder.Count : (i-1);
                    newOrder.Add(dimensionOrder[i], n);
                }
                else
                {
                    newOrder.Add(dimensionOrder[i], (i+1) % dimensionOrder.Count);
                }
            }

            float t = 0.0f;
            while (t < transitionDuration)
            {
                t += Time.deltaTime;

                for (int i = 0; i < dimensionOrder.Count; i++)
                {
                    Vector3 start = targetPos[i].position;
                    Vector3 end = targetPos[newOrder[dimensionOrder[i]]].position;
                    Transform dim = World.Instance.Dims[dimensionOrder[i]].transform;
                    dim.position = Vector3.Lerp(start, end, t / transitionDuration);
                }
                yield return null;
            }

            foreach (KeyValuePair<Dimension.Color, int> pair in newOrder)
            {
                dimensionOrder[pair.Value] = pair.Key;
            }
        }


        void ToggleDimensionActivation(bool status)
        {
            foreach (Dimension.Color color in Dimension.BaseColor)
            {
                World.Instance.Dims[color].gameObject.SetActive(status);
            }
        }

        void OnTransitionStartEnd(bool isStart)
        {
            Physics.gravity = isStart ? Vector3.zero : new Vector3(0f, -9.8f, 0f);
            Physics.autoSimulation = !isStart;
            playerController.paused = isStart;
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

    }

}