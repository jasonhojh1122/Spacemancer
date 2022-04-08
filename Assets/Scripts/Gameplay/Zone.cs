using UnityEngine;
using UnityEngine.Events;

using System;

namespace Gameplay
{
    [Serializable]
    public class ZoneEvent : UnityEvent<Collider>{}

    [RequireComponent(typeof(Collider))]
    public class Zone : MonoBehaviour
    {

        [SerializeField] protected ZoneEvent TriggerEnter;
        [SerializeField] protected ZoneEvent TriggerStay;
        [SerializeField] protected ZoneEvent TriggerExit;
        protected Collider col;

        protected void Awake()
        {
            col = GetComponent<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerEnter.Invoke(other);
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            TriggerStay.Invoke(other);
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            TriggerExit.Invoke(other);
        }

    }

}