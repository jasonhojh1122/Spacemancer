using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace Input
{
    public class CameraController : InputController
    {
        [SerializeField] float minMoveSpeed;
        [SerializeField] float maxMoveSpeed;
        [SerializeField] float minZoomSpeed;
        [SerializeField] float maxZoomSpeed;
        [SerializeField] float minZoom;
        [SerializeField] float maxZoom;
        [SerializeField] float defaultZoom;
        [SerializeField] float accelInterval = 1.5f;
        [SerializeField] AnimationCurve accelCurve;
        [SerializeField] Transform followTarget;
        [SerializeField] Transform player;
        [SerializeField] BoxCollider camConfiner;
        [SerializeField] Cinemachine.CinemachineVirtualCamera cam;

        float moveSpeed, zoomSpeed, startMoveT, startZoomT, moveSpeedDiff, zoomSpeedDiff;

        bool moving, zooming;

        InputAction moveAction;
        InputAction zoomAction;

        new void Awake()
        {
            base.Awake();
            moveAction = playerInput.actions["Movement"];
            zoomAction = playerInput.actions["Zoom"];
            moving = false;
            zooming = false;
            moveSpeedDiff = maxMoveSpeed - minMoveSpeed;
            zoomSpeedDiff = maxZoomSpeed - minZoomSpeed;
            cam.m_Lens.OrthographicSize = defaultZoom;
        }

        private void Update()
        {
            if (IsPaused()) return;
            Move();
            Zoom();
        }

        void Move()
        {
            var dir2d = moveAction.ReadValue<Vector2>();
            bool pressed = !Util.Fuzzy.CloseFloat(dir2d.magnitude, 0.0f);
            if (!moving && pressed)
            {
                moving = true;
                startMoveT = Time.time;
            }
            else if (!pressed)
            {
                moving = false;
            }

            if (moving)
            {
                moveSpeed = minMoveSpeed +
                    accelCurve.Evaluate((Time.time - startMoveT) / accelInterval) * moveSpeedDiff;
                var vel = new Vector3(dir2d.x, 0, dir2d.y) * moveSpeed;
                var newPos = followTarget.position + vel * Time.deltaTime;
                followTarget.position = newPos;
            }
        }

        void Zoom()
        {
            var value = zoomAction.ReadValue<float>();
            bool pressed = !Util.Fuzzy.CloseFloat(value, 0.0f);
            if (!zooming && pressed)
            {
                zooming = true;
                startZoomT = Time.time;
            }
            else if (!pressed)
            {
                zooming = false;
            }

            if (zooming)
            {
                zoomSpeed = minZoomSpeed +
                    accelCurve.Evaluate((Time.time - startZoomT) / accelInterval) * zoomSpeedDiff;
                var lenSize = cam.m_Lens.OrthographicSize + value * zoomSpeed * Time.deltaTime;
                lenSize = Mathf.Clamp(lenSize, minZoom, maxZoom);
                cam.m_Lens.OrthographicSize = lenSize;
            }
        }

        public void FollowPlayer()
        {
            cam.Follow = player;
        }

        public void UnFollowPlayer()
        {
            followTarget.position = player.transform.position;
            cam.Follow = followTarget;
        }

        /// <summary>
        /// Zooms out the camera in the given time.
        /// </summary>
        /// <param name="tLimit"> Time limit to zoom out. </param>
        public void ZoomOut(float tLimit)
        {
            StartCoroutine(ZoomOutAnim(tLimit));
        }

        IEnumerator ZoomOutAnim(float tLimit)
        {
            float t = 0;
            float startSize = cam.m_Lens.OrthographicSize;
            while (t < tLimit)
            {
                t += Time.deltaTime;
                cam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, maxZoom, t / tLimit);
                yield return null;
            }
        }

        /// <summary>
        /// Zooms in the camera in the given time.
        /// </summary>
        /// <param name="tLimit"> Time limit to zoom in. </param>
        public void ZoomIn(float tLimit)
        {
            StartCoroutine(ZoomInAnim(tLimit));
        }

        IEnumerator ZoomInAnim(float tLimit)
        {
            float t = 0;
            float startSize = cam.m_Lens.OrthographicSize;
            while (t < tLimit)
            {
                t += Time.deltaTime;
                cam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, defaultZoom, t / tLimit);
                yield return null;
            }
        }

    }

}
