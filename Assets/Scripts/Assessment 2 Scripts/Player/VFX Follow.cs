#region

using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class VFXFollow : MonoBehaviour
    {
        private Transform m_Target;
        private Vector3 m_Offset;

        public void Setup(Transform target, Vector3 offset)
        {
            m_Target = target;
            m_Offset = offset;
        }

        void LateUpdate()
        {
            if (m_Target)
            {
                //Follows the player using their transform position and offset
                transform.position = m_Target.position + m_Offset;
            }
            else
            {
                //Dies with the player
                Destroy(gameObject);
            }
        }
    }
}