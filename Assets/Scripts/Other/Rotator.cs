
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float angularSpeed = 10.0f;
    private void Update()
    {
        transform.Rotate(Vector3.up, angularSpeed * Time.deltaTime, Space.World);
    }
}