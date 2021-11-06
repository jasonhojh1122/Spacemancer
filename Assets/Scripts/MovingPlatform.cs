using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SplitableObject))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    // [SerializeField] Dimension.Color activeColor; // None if no restriction
    [SerializeField] float moveSpeed = 3f;

    Vector3 startPos;
    Vector3 endPos;
    SplitableObject so;
    Rigidbody rb;
    bool moveToEnd = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        so = GetComponent<SplitableObject>();
        startPos = startPoint.localPosition;
        endPos = endPoint.localPosition;
    }

    private void Update()
    {
        if (so.IsInCorrectDim())
            Move();
    }

    void Move() {
        Vector3 target = (moveToEnd) ? endPos : startPos;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed*Time.deltaTime);
        if (moveToEnd && Fuzzy.CloseVector3(transform.localPosition, endPos)) {
            moveToEnd = false;
        }
        else if (!moveToEnd && Fuzzy.CloseVector3(transform.localPosition, startPos)) {
            moveToEnd = true;
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
