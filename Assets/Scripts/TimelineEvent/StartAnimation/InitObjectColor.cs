
using UnityEngine;
using System.Collections.Generic;
namespace TimelineEvent.StartAnimation
{
    public class InitObjectColor : MonoBehaviour
    {
        [SerializeField] List<Core.ObjectColor> objects;

        private void OnEnable()
        {
            foreach (var obj in objects)
            {
                obj.Color = obj.Color;
            }
        }
    }
}
