#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class GroundSensor : MonoBehaviour
    {
        [SerializeField] private CollisionDetector m_CollisionDetector;
        [SerializeField] private LayerMask m_GroundLayer;
        [SerializeField] private Vector2 m_BoxSize = new Vector2(1, 0.2f);

        private Collider2D m_HitComp;
        public bool m_IsGrounded { get; private set; } = true; //on the ground by default

        private void OnEnable() //subscribe to the collision event
        {
            m_CollisionDetector.CollisionOverlap += CheckGround;
        }

        private void OnDisable() //unsubscribe from the collision event
        {
            m_CollisionDetector.CollisionOverlap -= CheckGround;
        }

        public void CheckGround()
        {
            m_HitComp = Physics2D.BoxCast(transform.position - new Vector3(0, (0.88f), 0), m_BoxSize, 0.0f,
                Vector2.down,
                0f, m_GroundLayer).collider;
            m_IsGrounded = m_HitComp;
        }

        public bool
            CheckGround(
                out Collider2D hitComp) //overloads the previous function, also runs on collision event - checks if grounded
        {
            CheckGround();
            hitComp = m_HitComp;
            return m_IsGrounded;
        }

        void OnDrawGizmos() //Visualises the boxcast when the gizmos overlay is enabled
        {
            Gizmos.color = m_IsGrounded ? Color.green : Color.red;

            Gizmos.DrawWireCube(transform.position - new Vector3(0, (0.88f), 0), m_BoxSize); //Draws the boxcast area
        }
    }
}