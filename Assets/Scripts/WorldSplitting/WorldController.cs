
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class WorldController : InputController
    {

        [SerializeField] World world;

        InputAction splitMerge;
        InputAction rotateLeft;
        InputAction rotateRight;

        new void Awake()
        {
            base.Awake();
            splitMerge = playerInput.actions["SplitMerge"];
            rotateLeft = playerInput.actions["RotateLeft"];
            rotateRight = playerInput.actions["RotateRight"];
        }

        private void Update()
        {
            if (pause) return;
            if (splitMerge.triggered)
            {
                world.Toggle();
                Debug.Log("Split");
            }
            if (rotateLeft.triggered)
            {
                world.RotateDimensions(-1);
                Debug.Log("Rotate Left");
            }
            if (rotateRight.triggered)
            {
                world.RotateDimensions(1);
                Debug.Log("Rotate Right");
            }

        }


    }

}
