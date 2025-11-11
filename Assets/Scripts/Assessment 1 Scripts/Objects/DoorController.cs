#region

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Objects
{
    public class DoorController : MonoBehaviour, IInteractable
    {
        [Header("Objects")] [SerializeField] private GameObject m_TopDoor;
        [SerializeField] private GameObject m_BottomDoor;
        [SerializeField] private float m_MoveDistance = 1.01f; // Total distance to move
        [SerializeField] private float m_MoveSpeed = 0.33f; // How far (%) the door interpolates each frame

        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;

        private bool m_DoorsMoving;

        public void Interaction()
        {
            if (m_DoorsMoving)
                return;

            StartCoroutine(C_DoorSequence());

            m_MoveDistance *= -1; //reverses the movement direction if the door is toggled
        }

        private IEnumerator C_MoveRoutine(bool isTopDoor, float distance)
        {
            GameObject door; //stored locally to avoid conflicts and interference between top and bottom doors
            Vector3 startPosition;
            Vector3 endPosition;

            if (isTopDoor)
            {
                door = m_TopDoor;
                startPosition = door.transform.position;
                endPosition = startPosition + new Vector3(0, distance, 0);
            }
            else // isBottomDoor
            {
                door = m_BottomDoor;
                startPosition = door.transform.position;
                endPosition = startPosition - new Vector3(0, distance, 0);
            }

            while (Vector3.Distance(door.transform.position, endPosition) > 0.02f)
            {
                // Keeps moving the object a % of the remaining distance towards the target
                
                door.transform.position =
                    Vector3.MoveTowards(door.transform.position, endPosition, Time.fixedDeltaTime * m_MoveSpeed);

                yield return null;
            }
        }

        private IEnumerator C_DoorSequence() //ensures only one instance of door movement occurs at a time
        {
            m_DoorsMoving = true;

            Coroutine moveTop = StartCoroutine(C_MoveRoutine(true, m_MoveDistance));
            Coroutine moveBottom = StartCoroutine(C_MoveRoutine(false, m_MoveDistance));

            yield return moveTop;
            yield return moveBottom;

            m_DoorsMoving = false;
        }

        public bool IsMoving()
        {
            return m_DoorsMoving;
        }
    }
}