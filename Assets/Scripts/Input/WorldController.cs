
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class WorldController : InputController
    {

        [SerializeField] Core.World world;

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
            if (IsPaused()) return;
            if (splitMerge.triggered && SpaceDevice.EnergyBar.Instance.IsSufficient())
            {
                SpaceDevice.EnergyBar.Instance.CostSingleAction();
                world.Toggle();
            }
        }


    }

}
