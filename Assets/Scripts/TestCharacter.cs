using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    private float nextMove;
    private float elapsedTime;
    private Rigidbody rb;
    public Vector3 movDir;
    // Start is called before the first frame update
    void Start()
    {
        nextMove = 0f;
        elapsedTime = 0f;
        movDir = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    { 
        Time.timeScale = 1;
        var moveDelta = 0.2f;
        elapsedTime += Time.deltaTime;
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");
        Vector3 curMove = new Vector3(0, 0, 0);
        if (elapsedTime > nextMove )
        {
            if (h > 0)
            {
                movDir = new Vector3(1, 0, 0);
                curMove += new Vector3(1, 0, 0);
            }
            else if (h < 0)
            {
                movDir = new Vector3(-1, 0, 0); ;
                curMove += new Vector3(-1, 0, 0);
            }
            if (v > 0)
            {
                movDir =  new Vector3(0, 0, 1);
                curMove += new Vector3(0, 0, 1);
            }
            else if (v < 0)
            {
                movDir = new Vector3(0, 0, -1); 
                curMove += new Vector3(0, 0, -1);
            }
            nextMove = elapsedTime + moveDelta;
            /* TODO */
            /* 檢查是否在特定區域內 */
            transform.position += curMove;
            rb.velocity *= 0;
            rb.isKinematic = true;
        }
    }
}
