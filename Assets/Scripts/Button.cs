using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core;

[RequireComponent(typeof(Core.SplittableObject))]
public class Button : MonoBehaviour
{

    [SerializeField] SplittableObject generatedObjectRef;
    [SerializeField] string keyObjectName;
    [SerializeField] AudioSource audio;

    Core.SplittableObject so;
    Core.SplittableObject keyObject;
    Core.SplittableObject generatedObject;

    private void Awake()
    {
        so = GetComponent<Core.SplittableObject>();
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
        if (generatedObject == null && Match(other.transform.parent.gameObject))
        {
            ToggleOn();
            keyObject.ObjectColor.OnColorChanged.AddListener(OnkeyObjectColorChanged);
        }

    }

    void OnTriggerExit(Collider other)
    {
        audio.Play();
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
        keyObject = obj.GetComponent<Core.SplittableObject>();
        if (keyObject == null)
        {
            return false;
        }
        else if (keyObject.Color != so.Color || obj.name != keyObjectName)
        {
            keyObject = null;
            return false;
        }
        else
        {
            return true;
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
    }

    public void OnkeyObjectColorChanged()
    {
        if (generatedObject == null && keyObject.Color != so.Color)
        {
            ToggleOn();
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
