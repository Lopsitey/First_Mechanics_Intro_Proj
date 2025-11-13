#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Objects
{
    public class CameraInitialisation : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Offset;
        private Transform m_PlayerTransform;

        public void Init(Transform playerTrans)
        {
            m_PlayerTransform = playerTrans;
        }

        void LateUpdate()
        {
            if (m_PlayerTransform)
            {
                transform.position = m_PlayerTransform.position + m_Offset;
            } //TODO add code for allowing the player to move a small amount before the camera snaps to them
        }
    }
}