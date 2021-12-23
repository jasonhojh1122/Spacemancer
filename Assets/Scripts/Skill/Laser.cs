using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;

namespace Skill
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
        public Dimension.Color Color
        {
            get => _color;
            set
            {
                _color = value;
                lr.GetPropertyBlock(_property);
                _property.SetColor("_Color", Dimension.MaterialColor[value]);
                lr.SetPropertyBlock(_property);
                if (hittedObject != null)
                {
                    hittedObject.ObjectColor.SelectColor = _color;
                }
            }
        }

        public bool IsOn
        {
            get => _IsOn;
            set
            {
                _IsOn = value;
                if(value == false)
                {
                    // lr.enabled = false;
                    if (hittedObject != null && hittedObject.gameObject.activeSelf)
                    {
                        hittedObject.ObjectColor.Unselect(lastContactPoint);
                    }
                    hittedObject = null;
                    TurnOff();
                }
                else
                {
                    // lr.enabled = true;
                    TurnOn();
                }
            }
        }
        public SplittableObject HittedObject
        {
            get => hittedObject;
        }
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

        public void TurnOn()
        {
            StopAllCoroutines();
            StartCoroutine(TurnOnAnim());
        }

        public void TurnOff()
        {
            StopAllCoroutines();
            StartCoroutine(TurnOffAnim());
        }

        System.Collections.IEnumerator TurnOnAnim()
        {
            lastContactPoint = Vector3.zero;
            curDistance = 0.0f;
            RaycastHit [] hits;
            while (true)
            {
                curDistance += Time.deltaTime * speed;
                curDistance = Mathf.Min(curDistance, maxDistance);
                hits = Physics.RaycastAll(transform.position, player.forward);

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
                        UpdateObjectMaterial(hit.point, newHittedObject);
                        lastContactPoint = hit.point;
                        hitted = true;
                        break;
                    }
                }

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

        void UpdateObjectMaterial(Vector3 contactPoint, SplittableObject newHittedObject)
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
        }

        System.Collections.IEnumerator InsertAnim()
        {
            yield return null;
        }

    }

}

