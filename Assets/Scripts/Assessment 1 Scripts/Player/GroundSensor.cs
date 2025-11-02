#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class GroundSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask m_GroundLayer;
        [SerializeField] private Vector2 m_BoxSize = new Vector2(1, 0.2f);
        private RaycastHit2D m_HitBox;
        public bool m_IsGrounded { get; private set; } = true; //on the ground by default

        private void OnEnable() //subscribe to the collision event
        {
            CollisionDetector.CollisionOverlap += CheckGround;
        }

        private void OnDisable() //unsubscribe from the collision event
        {
            CollisionDetector.CollisionOverlap -= CheckGround;
        }

        public void CheckGround() //also runs on collision event - checks if grounded
        {
            m_HitBox = Physics2D.BoxCast(transform.position - new Vector3(0, (0.88f), 0), m_BoxSize, 0.0f, Vector2.down,
                0f, m_GroundLayer);
            m_IsGrounded = m_HitBox.collider;
        }

        public void SetGround(bool isGrounded)
        {
            m_IsGrounded = isGrounded;
        }

        void OnDrawGizmos() //Visualizes the boxcast when the gizmos overlay is enabled
        {
            Gizmos.color = m_IsGrounded ? Color.green : Color.red;

            Gizmos.DrawWireCube(transform.position - new Vector3(0, (0.88f), 0), m_BoxSize); //Draws the boxcast area
        }
    }
}