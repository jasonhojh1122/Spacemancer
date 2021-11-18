using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core {
    public class CameraTransition : MonoBehaviour {

        [SerializeField] World world;
        float defaultZ;

        void Awake() {
            defaultZ = transform.position.z;
        }

        public IEnumerator Transition() {
            float t = 0.0f;
            while (t < world.TransitionDur) {
                t += Time.deltaTime;
                float p = (world.Splitted) ? t / world.TransitionDur
                        : (world.TransitionDur - t) / world.TransitionDur;
                Vector3 newPos = transform.position;
                newPos.z = defaultZ - p * world.Radius;
                transform.position = newPos;
                yield return null;
            }
        }


    }
}