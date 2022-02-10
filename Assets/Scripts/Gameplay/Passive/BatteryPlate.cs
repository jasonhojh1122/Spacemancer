using UnityEngine;

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
        SplittableObject battery;
        SplittableObject so;

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            so.ObjectColor.OnColorChanged.AddListener(OnColorChange);
            World.Instance.OnDimensionChange.AddListener(OnDimensionChange);
        }

        private void Start()
        {
            InvokeRepeating("CheckBattery", 0.0f, 0.1f);
        }

        void CheckBattery()
        {
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
            foreach (var device in World.Instance.ObjectPool.ActiveObjects[electronicName])
            {
                if (so.Dim.color == device.Dim.color)
                {
                    var electronic = device.GetComponent<Electronic.ElectronicObject>();
                    electronic.TurnOn();
                }
            }
        }

        private void ToggleOff()
        {
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
            CheckBattery();
        }

        public void CheckDimensionStatus()
        {
            if (battery != null && battery.IsInCorrectDim() && so.IsInCorrectDim())
                ToggleOn();
            else
                ToggleOff();
        }
    }

}
