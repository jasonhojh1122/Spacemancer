using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;

namespace SpaceDevice
{
    [RequireComponent(typeof(LineRenderer))]
    public class Laser : MonoBehaviour
    {
        [SerializeField] float width;
        [SerializeField] float speed;
        [SerializeField] Transform player;
        LineRenderer lr;
        MaterialPropertyBlock _property;
        ObjectColor _hittedObject;
        Dimension.Color _color;
        bool _IsOn = false;
        Vector3 _contactPoint, _localEndPoint;
        float maxLength = 150.0f;
        float _curLength;

        /// <summary>
        /// The <c>ObjectColor</c> hitted by laser.
        /// </summary>
        /// <value></value>
        public ObjectColor HittedObject
        {
            get => _hittedObject;
            set => _hittedObject = value;
        }

        /// <summary>
        /// Sets or gets the color of the laser.
        /// </summary>
        public Dimension.Color Color
        {
            get => _color;
            set
            {
                _color = value;
                lr.GetPropertyBlock(_property);
                _property.SetColor("_LaserColor", Dimension.MaterialColor[value]);
                lr.SetPropertyBlock(_property);
                if (HittedObject != null)
                    HittedObject.SelectColor = Color;
            }
        }

        /// <summary>
        /// Turns on or off the laser.
        /// </summary>
        public bool IsOn
        {
            get => _IsOn;
            set
            {
                _IsOn = value;
                if(value == false)
                {
                    HittedObject = null;
                    TurnOff();
                }
                else
                {
                    TurnOn();
                }
            }
        }

        /// <summary>
        /// The contact point between the laser and the hitted object.
        /// </summary>
        public Vector3 ContactPoint
        {
            get => _contactPoint;
            set
            {
                _contactPoint = value;
                _localEndPoint = transform.InverseTransformPoint(_contactPoint);
                _curLength = _localEndPoint.magnitude;
            }
        }

        Vector3 LocalEndPoint {
            get => _localEndPoint;
            set
            {
                _localEndPoint = value;
                _curLength = _localEndPoint.magnitude;
                _contactPoint = transform.TransformPoint(_localEndPoint);
            }
        }

        float CurLength
        {
            get => _curLength;
            set
            {
                _curLength = Mathf.Min(value, maxLength);
                _contactPoint = transform.position + player.forward * CurLength;
                _localEndPoint = transform.InverseTransformPoint(_contactPoint);
            }
        }

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.zero);
            CurLength = 0.0f;
            _property = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Turns on the laser.
        /// </summary>
        void TurnOn()
        {
            StopAllCoroutines();
            StartCoroutine(ActiveLoop());
        }

        /// <summary>
        /// Turns off the laser.
        /// </summary>
        void TurnOff()
        {
            StopAllCoroutines();
            StartCoroutine(TurnOffAnim());
        }

        /// <summary>
        /// Continuously updates the laser end points and checks for
        /// new hitted objects.
        /// </summary>
        System.Collections.IEnumerator ActiveLoop()
        {
            lr.enabled = true;
            ContactPoint = Vector3.zero;
            CurLength = 0.0f;
            while (true)
            {
                var newHittedObject = CheckHit();
                if (newHittedObject != null)
                {
                    UpdateHittedObject(newHittedObject);
                }
                else
                {
                    CurLength += speed * Time.deltaTime;
                    if (HittedObject != null && HittedObject.gameObject.activeSelf)
                    {
                        UnselectHittedObject();
                        HittedObject = null;
                    }
                }
                lr.SetPosition(1, LocalEndPoint);
                yield return null;
            }
        }

        /// <summary>
        /// Gradually turns off the laser.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator TurnOffAnim()
        {
            while (CurLength > 0.1f)
            {
                CurLength -= speed * Time.deltaTime;
                lr.SetPosition(1, LocalEndPoint);
                yield return null;
            }
            lr.enabled = false;
        }

        /// <summary>
        /// Checks if the laser hitted any <c>SplittableObject</c>.
        /// Updates <c>contactPoint</c> if hits.
        /// </summary>
        /// <returns> The <c>ObjectColor</c> is hitted, otherwise null is returned </returns>
        ObjectColor CheckHit()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, player.forward);
            IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);
            ObjectColor newHittedObject;
            foreach (RaycastHit hit in orderedHits)
            {
                if (hit.collider != null && !hit.collider.isTrigger &&
                    (newHittedObject = hit.collider.gameObject.GetComponent<ObjectColor>()) != null)
                {
                    ContactPoint = hit.point;
                    return newHittedObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the <c>HittedObject</c> by checking new hitted object and the
        /// contact point.
        /// </summary>
        /// <param name="newHittedObject"> The new hitted object. </param>
        void UpdateHittedObject(ObjectColor newHittedObject)
        {
            bool selectNew = false, unselectOld = false;
            if (HittedObject == null)
            {
                selectNew = true;
            }
            else if (HittedObject.GetInstanceID() != newHittedObject.GetInstanceID() &&
                        !IsInSameGroup(newHittedObject))
            {
                selectNew = true;
                unselectOld = true;
            }

            if (unselectOld)
                UnselectHittedObject();
            HittedObject = newHittedObject;
            if (selectNew)
                SelectHittedObject();
        }

        void SelectHittedObject()
        {
            HittedObject.SelectColor = Color;
            HittedObject.ContactPoint = ContactPoint;
            HittedObject.Select();
        }

        void UnselectHittedObject()
        {
            HittedObject.ContactPoint = ContactPoint;
            HittedObject.Unselect();
        }

        bool IsInSameGroup(ObjectColor newHittedObject)
        {
            if (HittedObject.IsRoot)
                return HittedObject.GroupSet.Contains(newHittedObject);
            else
                return HittedObject.Root.GroupSet.Contains(newHittedObject);
        }

    }

}

