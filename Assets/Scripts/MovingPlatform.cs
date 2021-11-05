using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SplitableObject))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] Dimension.Color activeColor; // None if no restriction
    [SerializeField] float moveSpeed = 3f;
    Rigidbody rb;
    bool moveToEnd = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Vector3LerpCoroutine(transform, endPoint.localPosition, moveSpeed));
    }
    private void Update()
    {
        startPoint = transform.parent.Find("startRefPoint");
        endPoint = transform.parent.Find("endRefPoint");

        Debug.Log("Active Color:");
        Debug.Log(activeColor);
        Debug.Log("Dimension Color:");
        Debug.Log(transform.parent.GetComponent<Dimension>().GetColor());
        if (activeColor != Dimension.Color.NONE  && activeColor!=transform.parent.GetComponent<Dimension>().GetColor())
            return;
        if (transform.localPosition == endPoint.localPosition)
        {
            moveToEnd = false;
            StartCoroutine(Vector3LerpCoroutine(transform, startPoint.localPosition, moveSpeed));
        }
        else if (transform.localPosition == startPoint.localPosition)
        {
            moveToEnd = true;
            StartCoroutine(Vector3LerpCoroutine(transform, endPoint.localPosition, moveSpeed));
        }
        else
        {
            Vector3 tmpPos = startPoint.localPosition;
            if (moveToEnd)
            {
                tmpPos = endPoint.localPosition;
            }
            StartCoroutine(Vector3LerpCoroutine(transform, tmpPos, moveSpeed));
        }
    }
    IEnumerator Vector3LerpCoroutine(Transform t, Vector3 target, float speed)
    {
        Vector3 startPosition = t.localPosition;
        float time = 0f;

        while (t.localPosition != target)
        {
            if (activeColor != Dimension.Color.NONE && activeColor != transform.parent.GetComponent<Dimension>().GetColor())
            {
                speed = 0;
                Debug.Log("Sleep");
            }
            else
            {
                speed = moveSpeed;
            }
            t.localPosition = Vector3.Lerp(startPosition, target, (time / Vector3.Distance(startPosition, target)) * speed);
            time += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            other.attachedRigidbody.velocity = rb.velocity;
        }
    }
    private void OnTriggerExit(Collider other)
    {
       if(other.transform.tag == "Player")
        {
            
        }
    }
}
