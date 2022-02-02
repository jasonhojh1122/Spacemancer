using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Splittable;

namespace Gameplay
{
    public class MovingPlatformManager
    {
        List<bool> isMovingToEnds;
        List<int> lastUpdatedFrame;
        List<Vector3> startPositions;
        List<Vector3> endPositions;
        List<Vector3> curPositions;

        public MovingPlatformManager()
        {
            isMovingToEnds = new List<bool>();
            lastUpdatedFrame = new List<int>();
            startPositions = new List<Vector3>();
            endPositions = new List<Vector3>();
            curPositions = new List<Vector3>();
        }

        /// <summary>
        /// Registers to the manager.
        /// </summary>
        /// <param name="isMovingToEnd"> Is the platform moving toward the end position. </param>
        /// <param name="startPos"> The start position. </param>
        /// <param name="endPos"> The end position. </param>
        /// <param name="curPos"> The current position. </param>
        /// <returns></returns>
        public int Register(bool isMovingToEnd, Vector3 startPos, Vector3 endPos, Vector3 curPos)
        {
            isMovingToEnds.Add(isMovingToEnd);
            startPositions.Add(startPos);
            endPositions.Add(endPos);
            curPositions.Add(curPos);
            lastUpdatedFrame.Add(-1);
            return curPositions.Count - 1;
        }

        /// <summary>
        /// Calculates and gets the new position.
        /// </summary>
        /// <param name="ID"> ID of the movingplatform. </param>
        /// <param name="frame"> Current frame. </param>
        /// <param name="speed"> Moving speed. </param>
        /// <returns></returns>
        public Vector3 GetNewPosition(int ID, int frame, float speed)
        {
            lastUpdatedFrame[ID] = frame;
            Vector3 target = (isMovingToEnds[ID]) ? endPositions[ID] : startPositions[ID];
            curPositions[ID] = Vector3.MoveTowards(curPositions[ID], target, speed * Time.deltaTime);
            if (Util.Fuzzy.CloseVector3(curPositions[ID], target))
                isMovingToEnds[ID] = !isMovingToEnds[ID];
            return curPositions[ID];
        }

        public bool PositionIsUpdated(int ID, int frame)
        {
            return lastUpdatedFrame[ID] == frame;
        }

        public Vector3 GetStartPos(int ID)
        {
            return startPositions[ID];
        }

        public Vector3 GetEndPos(int ID)
        {
            return endPositions[ID];
        }

        public Vector3 GetCurPosition(int ID)
        {
            return curPositions[ID];
        }

    }

    [System.Serializable, RequireComponent(typeof(SplittableObject))]
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] Transform startPoint;
        [SerializeField] Transform endPoint;

        [SerializeField] bool isMovingToEnd = true;
        [SerializeField] float moveSpeed = 1.5f;
        [SerializeField, HideInInspector] public bool isRegistered = false;
        [SerializeField, HideInInspector] public int ID = 0;
        static MovingPlatformManager manager;
        SplittableObject so;

        private void Awake()
        {
            so = GetComponent<SplittableObject>();
            if (manager == null)
                manager = new MovingPlatformManager();
        }

        private void Start()
        {
            if (!isRegistered)
                Register();
            else
                transform.localPosition = manager.GetCurPosition(ID);
        }

        private void OnEnable()
        {
            if (isRegistered)
                transform.localPosition = manager.GetCurPosition(ID);
        }

        void Register()
        {
            // TODO: update to new split/merge system.
            /*
            var startPos = Core.World.Instance.Dims[Core.Dimension.Color.WHITE].
                            transform.InverseTransformPoint(startPoint.position);
            var endPos = Core.World.Instance.Dims[Core.Dimension.Color.WHITE].
                        transform.InverseTransformPoint(endPoint.position);
            var curPos = Core.World.Instance.Dims[Core.Dimension.Color.WHITE].
                        transform.InverseTransformPoint(transform.position);
            ID = manager.Register(isMovingToEnd, startPos, endPos, curPos);
            isRegistered = true;
            */
        }

        private void FixedUpdate()
        {
            if (so.IsInCorrectDim() && !InputManager.Instance.pause)
                Move();
        }

        /// <summary>
        /// Handle moves of MovingPlatforms
        /// </summary>
        void Move()
        {
            if (manager.PositionIsUpdated(ID, Time.frameCount))
            {
                transform.localPosition = manager.GetCurPosition(ID);
                return;
            }
            else
            {
                transform.localPosition = manager.GetNewPosition(ID, Time.frameCount, moveSpeed);
            }
        }
    }

}
