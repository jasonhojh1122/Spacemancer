using UnityEngine;

using System.Collections.Generic;
using Core;
using Splittable;

namespace Gameplay
{
    [RequireComponent(typeof(SplittableObject))]
    public class BatteryPlate : GameplayObject
    {

        [SerializeField] string electronicName;
        [SerializeField] string batteryName;
        [SerializeField] Collider trigger;
        [SerializeField] Dimension.Color activeColor = Dimension.Color.WHITE;
        [SerializeField] List<ObjectColor> indicators;
        [SerializeField] UnityEngine.Events.UnityEvent OnCharged;
        [SerializeField] UnityEngine.Events.UnityEvent OnUncharged;
        SplittableObject battery;
        SplittableObject so;

        bool toggled;

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
            World.Instance.OnTransitionStart.AddListener(OnTransitionStart);
            // World.Instance.OnTransitionEnd.AddListener(OnDimensionChange);
            foreach (var oc in indicators)
            {
                oc.Color = activeColor;
            }
            toggled = false;
        }

        private void Start()
        {
            InvokeRepeating("CheckBattery", 0.0f, 0.1f);
        }

        void OnTransitionStart()
        {
            if (battery != null)
            {
                toggled = false;
                RemoveListener();
                battery = null;
            }
        }

        void CheckBattery()
        {
            if (World.Instance.Transitting || so.Dim.color == Dimension.Color.BLACK) return;
            var newBattery = GetNewBattery();
            if (newBattery != null && battery == null)
            {
                battery = newBattery;
                AddListener();
                CheckDimensionStatus();
            }
            else if (newBattery != null && battery != null)
            {
                if (newBattery.GetInstanceID() != battery.GetInstanceID())
                {
                    RemoveListener();
                    battery = newBattery;
                    AddListener();
                }
                else
                {
                    CheckDimensionStatus();
                }
            }
            else if (newBattery == null && battery != null)
            {
                RemoveListener();
                ToggleOff();
                battery = null;
            }
        }

        SplittableObject GetNewBattery()
        {
            Collider[] colliders = Physics.OverlapBox(trigger.bounds.center, trigger.bounds.extents, Quaternion.identity);
            SplittableObject newBattery = null;
            foreach (var collider in colliders)
            {
                if (collider.gameObject.name == batteryName)
                {
                    newBattery = collider.GetComponent<SplittableObject>();
                    break;
                }
            }
            return newBattery;
        }

        void AddListener()
        {
            battery.ObjectColor.OnColorChanged.AddListener(CheckDimensionStatus);
        }

        void RemoveListener()
        {
            battery.ObjectColor.OnColorChanged.RemoveListener(CheckDimensionStatus);
        }

        private void ToggleOn()
        {
            toggled = true;
            OnCharged.Invoke();
            foreach (var device in World.Instance.ObjectPool.ActiveObjects[electronicName])
            {
                if (so.Dim.color == device.Dim.color && so.Color == device.Dim.color)
                {
                    var electronic = device.GetComponent<Electronic.ElectronicObject>();
                    electronic.TurnOn();
                }
            }
        }

        private void ToggleOff()
        {
            toggled = false;
            OnUncharged.Invoke();
            foreach (var device in World.Instance.ObjectPool.ActiveObjects[electronicName])
            {
                if (so.Dim.color == device.Dim.color)
                {
                    var electronic = device.GetComponent<Electronic.ElectronicObject>();
                    electronic.TurnOff();
                }
            }
        }

        public override void OnColorChange()
        {
            CheckDimensionStatus();
        }

        public override void OnDimensionChange()
        {
            Debug.Log(transform.GetInstanceID() + " OnTransitionENd");
            if (battery != null)
            {
                toggled = false;
                RemoveListener();
                battery = null;
            }
            CheckBattery();
        }

        public void CheckDimensionStatus()
        {
            var correctStatus = battery != null && battery.IsInCorrectDim()
                && so.IsInCorrectDim() && so.Dim.color == activeColor;
            if (correctStatus && !toggled)
            {
                ToggleOn();
            }
            else if (!correctStatus && toggled)
            {
                ToggleOff();
            }
        }
    }

}
