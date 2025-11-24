#region

using Unity.Cinemachine;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class CameraPan : MonoBehaviour
    {
        [Header("Room Overview camera")] [SerializeField]
        private CinemachineCamera m_RoomWideCam;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("PlayerCol"))
            {
                //Zooms out
                //Overrides the player camera which has a priority of 10
                m_RoomWideCam.Priority = 15;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("PlayerCol"))
            {
                //Zooms in
                //Gives control back to the player camera by lowering the priority
                m_RoomWideCam.Priority = 5;
            }
        }
    }
}