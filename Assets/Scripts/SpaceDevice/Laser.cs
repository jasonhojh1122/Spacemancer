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
        SplittableObject hittedObject;
        Dimension.Color _color;
        bool _IsOn = false;
        Vector3 lastContactPoint;
        float maxDistance = 150.0f;
        float curDistance;

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
                if (hittedObject != null)
                {
                    hittedObject.ObjectColor.SelectColor = _color;
                }
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
                    if (hittedObject != null && hittedObject.gameObject.activeSelf)
                    {
                        hittedObject.ObjectColor.Unselect(lastContactPoint);
                    }
                    hittedObject = null;
                    TurnOff();
                }
                else
                {
                    TurnOn();
                }
            }
        }

        /// <summary>
        /// The <c>SplittableObject</c> hitted by laser.
        /// </summary>
        /// <value></value>
        public SplittableObject HittedObject
        {
            get => hittedObject;
            set => hittedObject = value;
        }

        /// <summary>
        /// The contact point between the laser and the hitted object.
        /// </summary>
        public Vector3 ContactPoint {
            get => lastContactPoint;
        }

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, Vector3.zero);
            curDistance = 0.0f;
            _property = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Turns on the laser.
        /// </summary>
        void TurnOn()
        {
            StopAllCoroutines();
            StartCoroutine(ActiveAnim());
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
        System.Collections.IEnumerator ActiveAnim()
        {
            lr.enabled = true;
            lastContactPoint = Vector3.zero;
            curDistance = 0.0f;
            while (true)
            {
                curDistance += Time.deltaTime * speed;
                curDistance = Mathf.Min(curDistance, maxDistance);

                bool hitted = CheckHit();
                if (!hitted)
                {
                    var endPos = transform.position + player.forward * curDistance;
                    var endPosLocal = transform.InverseTransformPoint(endPos);
                    lr.SetPosition(1, endPosLocal);
                    if (hittedObject != null && hittedObject.gameObject.activeSelf)
                    {
                        hittedObject.ObjectColor.Unselect(endPos);
                    }
                    hittedObject = null;
                    lastContactPoint = endPos;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Checks if the laser hitted any <c>SplittableObject</c>. If hits then
        /// updates the <c>HittedObject</c> and its materials.
        /// </summary>
        /// <returns> True if hits <c>SplittableObject</c> </returns>
        bool CheckHit()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, player.forward);
            SplittableObject newHittedObject;
            bool hitted = false;;
            IEnumerable<RaycastHit> orderedHits = hits.OrderBy(hit => hit.distance);
            foreach (RaycastHit hit in orderedHits)
            {
                if (hit.collider != null && !hit.collider.isTrigger &&
                    (newHittedObject = hit.collider.gameObject.GetComponent<SplittableObject>()) != null)
                {
                    var hitPosLocal = transform.InverseTransformPoint(hit.point);
                    lr.SetPosition(1, hitPosLocal);
                    curDistance = hitPosLocal.magnitude;
                    UpdateObjectAndMaterial(hit.point, newHittedObject);
                    lastContactPoint = hit.point;
                    hitted = true;
                    break;
                }
            }
            return hitted;
        }

        /// <summary>
        /// Updates the <c>HittedObject</c> by checking new hitted object and the
        /// contact point.
        /// </summary>
        /// <param name="contactPoint"> The contact position hitted on the object. </param>
        /// <param name="newHittedObject"> The new hitted object. </param>
        void UpdateObjectAndMaterial(Vector3 contactPoint, SplittableObject newHittedObject)
        {
            bool selectNew = false, unselectOld = false;
            if (hittedObject == null)
            {
                selectNew = true;
            }
            else if (hittedObject.gameObject.GetInstanceID() != newHittedObject.gameObject.GetInstanceID())
            {
                selectNew = true;
                unselectOld = true;
            }

            if (unselectOld)
            {
                hittedObject.ObjectColor.Unselect(lastContactPoint);
            }
            hittedObject = newHittedObject;
            if (selectNew)
            {
                if (hittedObject.IsPersistentColor)
                {
                    hittedObject = null;
                }
                else
                {
                    hittedObject.ObjectColor.SelectColor = _color;
                    hittedObject.ObjectColor.Select(contactPoint);
                }
            }
        }

        /// <summary>
        /// Gradually turns off the laser.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator TurnOffAnim()
        {
            while (curDistance > 0.1f)
            {
                curDistance -= speed * Time.deltaTime;
                lastContactPoint = transform.position + player.forward * curDistance;
                var endPosLocal = transform.InverseTransformPoint(lastContactPoint);
                lr.SetPosition(1, endPosLocal);
                yield return null;
            }
            lr.enabled = false;
        }

    }

}

