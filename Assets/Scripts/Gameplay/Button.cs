using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;
using Splittable;

namespace Gameplay
{
    [RequireComponent(typeof(SplittableObject))]
    public class Button : MonoBehaviour
    {

        [SerializeField] SplittableObject generatedObjectRef;
        [SerializeField] string keyObjectName;
        [SerializeField] AudioSource audioSource;
        [SerializeField] SplittableObject keyObject;

        SplittableObject so;
        SplittableObject generatedObject;

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
        }

        private void Start()
        {
            World.Instance.BeforeSplit.AddListener(BeforeSplitAndMerge);
            World.Instance.BeforeMerge.AddListener(BeforeSplitAndMerge);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!so.IsInCorrectDim())
                return;
            if (generatedObject == null)
            {
                keyObject = other.transform.parent.gameObject.GetComponent<SplittableObject>();
                if (keyObject == null)
                {
                    return;
                }
                else if (other.transform.parent.gameObject.name != keyObjectName)
                {
                    keyObject = null;
                    return;
                }
                else
                {
                    audioSource.Play();
                    OnkeyObjectColorChanged();
                    keyObject.ObjectColor.OnColorChanged.AddListener(OnkeyObjectColorChanged);
                }
            }

        }

        void OnTriggerExit(Collider other)
        {
            audioSource.Play();
            if (!so.IsInCorrectDim())
                return;
            if (keyObject != null && other.transform.parent.gameObject.GetInstanceID() == keyObject.gameObject.GetInstanceID())
            {
                ToggleOff();
                keyObject.ObjectColor.OnColorChanged.RemoveListener(OnkeyObjectColorChanged);
            }

        }

        private bool Match(GameObject obj)
        {
            keyObject = obj.GetComponent<SplittableObject>();
            if (keyObject == null)
            {
                return false;
            }
            else if (obj.name != keyObjectName)
            {
                keyObject = null;
                return false;
            }
            else
            {
                return keyObject.Color != so.Color;
            }

        }

        private void ToggleOn()
        {
            generatedObject = World.Instance.InstantiateNewObjectToDimension(generatedObjectRef, so.Dim.Color);
            generatedObject.Color = so.Dim.Color;
        }

        private void ToggleOff()
        {
            World.Instance.DeactivateObject(generatedObject);
            generatedObject = null;
        }

        public void OnkeyObjectColorChanged()
        {
            if (generatedObject == null && keyObject.Color == so.Color)
            {
                ToggleOn();
            }
            else if (generatedObject != null && keyObject.Color != so.Color)
            {
                ToggleOff();
            }
        }

        public void BeforeSplitAndMerge()
        {
            if (generatedObject != null)
            {
                World.Instance.DeactivateObject(generatedObject);
                generatedObject = null;
                keyObject.ObjectColor.OnColorChanged.RemoveListener(OnkeyObjectColorChanged);
            }
        }
    }

}
