
using UnityEngine;
using Core;
using System.Collections.Generic;

namespace Splittable
{
    public class SplittableInert : MonoBehaviour
    {
        [SerializeField] List<GameObject> targets;

        private void Awake()
        {
            for (int i = 1; i < World.Instance.Dimensions.Count; i++)
            {
                foreach (var target in targets)
                {
                    var instantiated = GameObject.Instantiate<GameObject>(target);
                    World.Instance.MoveObjectToDimension(instantiated, i);
                }
            }
        }
    }
}
