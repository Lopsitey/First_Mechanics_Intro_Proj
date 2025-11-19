#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class GroundSensor : MonoBehaviour
    {
        [SerializeField] private CollisionDetector m_CollisionDetector;
        [SerializeField] private LayerMask m_GroundLayer;
        [SerializeField] private Vector2 m_BoxSize = new Vector2(0.95f, 0.2f);
        [SerializeField] private float m_HeadWidth = 0.25f;

        private Collider2D m_HitComp;
        private ContactPoint2D? m_Contact;
        public bool m_IsGrounded { get; private set; } = true; //on the ground by default

        private void OnEnable() //subscribe to the collision event
        {
            m_CollisionDetector.CollisionOverlap += CheckGround;
        }

        private void OnDisable() //unsubscribe from the collision event
        {
            m_CollisionDetector.CollisionOverlap -= CheckGround;
        }

        //event function
        public void CheckGround()
        {
            m_HitComp = Physics2D.BoxCast(transform.position - new Vector3(0, (0.75f), 0), m_BoxSize, 0.0f,
                Vector2.down,
                0f, m_GroundLayer).collider;
            m_IsGrounded = m_HitComp;
        }

        //overloads the previous function, also runs on collision event - checks if grounded
        public bool CheckGround(out Collider2D hitComp)
        {
            CheckGround();
            hitComp = m_HitComp;
            return m_IsGrounded;
        }

        public int CheckCeiling()
        {
            // Adjust the '0.5f' to match the top of your sprite's head
            Vector2 headCenter = (Vector2)transform.position + new Vector2(0, 0.5f);

            Vector2 leftShoulder = headCenter + new Vector2(-m_HeadWidth, 0);
            Vector2 rightShoulder = headCenter + new Vector2(m_HeadWidth, 0);

            float rayDist = 0.3f;

            bool hitLeft = Physics2D.Raycast(leftShoulder, Vector2.up, rayDist, m_GroundLayer);
            bool hitRight = Physics2D.Raycast(rightShoulder, Vector2.up, rayDist, m_GroundLayer);

            //shifts the centre to the left using the half of the rayDist so the gap either side of the horizontal ray is even 
            Vector2 newCentre = headCenter + new Vector2(-rayDist / 2, 0.3f);
            bool hitCentre = Physics2D.Raycast(newCentre, Vector2.right, rayDist, m_GroundLayer);

            if (hitCentre) return 1; //centre hit - do nothing
            if (hitLeft && hitRight) return 1; //weird hit - between two blocks? - do nothing

            if (hitLeft) return 2; //hit left
            if (hitRight) return 3; //hit right

            return 0; //returns 0 by default
        }

        void OnDrawGizmos() //Visualises the boxcast when the gizmos overlay is enabled
        {
            Gizmos.color = m_IsGrounded ? Color.green : Color.red;

            Gizmos.DrawWireCube(transform.position - new Vector3(0, (0.75f), 0), m_BoxSize); //Draws the boxcast area

            Vector2 headCenter = (Vector2)transform.position + new Vector2(0, 0.5f);

            Vector2 leftShoulder = headCenter + new Vector2(-m_HeadWidth, 0);
            Vector2 rightShoulder = headCenter + new Vector2(m_HeadWidth, 0);

            float rayDist = 0.3f;

            Gizmos.DrawRay(leftShoulder, Vector2.up * rayDist);
            Gizmos.DrawRay(rightShoulder, Vector2.up * rayDist);

            Vector2 newCentre = headCenter + new Vector2(-rayDist / 2, 0.3f);
            Gizmos.DrawRay(newCentre, Vector2.right * rayDist);
        }
    }
}