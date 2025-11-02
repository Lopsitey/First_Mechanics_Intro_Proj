#region

using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Assessment_1_Scripts.Objects
{
    public class CameraInitialisation : MonoBehaviour
    {
        [FormerlySerializedAs("offset")] public Vector3 m_Offset;
        private Transform m_PlayerTransform;

        public void Init(GameObject player)
        {
            m_PlayerTransform = player.transform;
        }

        void LateUpdate()
        {
            if (m_PlayerTransform)
            {
                transform.position = m_PlayerTransform.position + m_Offset;
            }
        }
    }
}