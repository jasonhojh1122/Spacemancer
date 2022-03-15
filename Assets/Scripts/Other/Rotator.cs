
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float angularSpeed = 10.0f;
    [SerializeField] Vector3 axis = new Vector3(0f, 1f, 0f);
    [SerializeField] bool checkColor = false;
    [SerializeField] Splittable.SplittableObject so;

    private void Update()
    {
        if (checkColor && !so.IsInCorrectDim()) return;
        transform.Rotate(axis, angularSpeed * Time.deltaTime, Space.World);
    }
}