using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Core.SplittableObject))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    // [SerializeField] Dimension.Color activeColor; // None if no restriction
    [SerializeField] float moveSpeed = 3f;
    MovingPlatformManager manager;
    Vector3 startPos;
    Vector3 endPos;
    Core.SplittableObject so;
    Rigidbody rb;
    /// <summary> Moving Direction </summary>    
    [SerializeField] public bool moveToEnd = true;

    /// <summary>Initialized by manager or not </summary>
    public bool isInit; 
    public int ID;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        so = GetComponent<Core.SplittableObject>();
        startPos = startPoint.localPosition;
        endPos = endPoint.localPosition;
        if(!isInit)
            FindObjectOfType<MovingPlatformManager>().Register(this,ID);
    }
    private void OnEnable(){
        if(isInit)
            moveToEnd = FindObjectOfType<MovingPlatformManager>().getDirection(ID);
    }

    private void FixedUpdate()
    {
        if (so.IsInCorrectDim())
            Move();
    }

    /// <summary>
    /// Handle moves of MovingPlatforms
    /// </summary>
    void Move() {
        Vector3 target = (moveToEnd) ? endPos : startPos;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, moveSpeed*Time.deltaTime);
        if (moveToEnd && Fuzzy.CloseVector3(transform.localPosition, endPos)) {
            moveToEnd = false;
            FindObjectOfType<MovingPlatformManager>().SetDirection(ID,moveToEnd);
            
        }
        else if (!moveToEnd && Fuzzy.CloseVector3(transform.localPosition, startPos)) {
            moveToEnd = true;
            FindObjectOfType<MovingPlatformManager>().SetDirection(ID,moveToEnd);
        }
    }


}
