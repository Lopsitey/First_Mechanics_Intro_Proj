#region

using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class GroundSensor : MonoBehaviour
    {
        [Header("Ground Detection")] [SerializeField]
        private CollisionDetector m_CollisionDetector;

        [SerializeField] private LayerMask m_GroundLayer;
        [SerializeField] private Vector2 m_BoxSize = new Vector2(0.95f, 0.2f);

        [Header("Bumped Head & Ledge Correction")] [SerializeField]
        private float m_BodyWidth = 0.25f;

        // Distance from center to feet
        [SerializeField] private float m_FeetYOffset = 0.77f;

        [SerializeField] private float m_LedgeRayLength = 0.5f;

        //The amount of a ledge you can clip through with your legs
        [SerializeField] private float m_LegHeight = 0.34f;

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

        //event function
        public void CheckGround()
        {
            //used a boxcast to make the input buffering more obvious and floaty
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

        /// <summary>
        /// Checks if the edges of your head clip a corner  
        /// </summary>
        /// <returns>The side of your head which clipped the corner (1-centre, 2-left, 3-right)</returns>
        public int CheckCeiling()
        {
            // Adjust the '0.5f' to match the top of your sprite's head
            Vector2 headCenter = (Vector2)transform.position + new Vector2(0, 0.8f);

            //Uses the body width to move the shoulders start points
            Vector2 leftShoulder = headCenter + new Vector2(-m_BodyWidth / 1.6f, 0);
            Vector2 rightShoulder = headCenter + new Vector2(m_BodyWidth / 1.6f, 0);

            float rayDist = 0.25f;

            bool hitLeft = Physics2D.Raycast(leftShoulder, Vector2.left, rayDist, m_GroundLayer);
            bool hitRight = Physics2D.Raycast(rightShoulder, Vector2.right, rayDist, m_GroundLayer);

            //shifts the centre to the left using the half of the rayDist so the gap either side of the horizontal ray is even 
            Vector2 newCentre = headCenter + new Vector2(-rayDist / 1.7f, 0f);
            bool hitCentre = Physics2D.Raycast(newCentre, Vector2.right, rayDist / 0.85f, m_GroundLayer);

            if (hitCentre) return 1; //centre hit - do nothing
            if (hitLeft && hitRight) return 1; //weird hit - between two blocks? - do nothing

            if (hitLeft) return 2; //hit left
            if (hitRight) return 3; //hit right

            return 0; //returns 0 by default
        }

        /// <summary>
        /// Checks if a corner hit the player's "legs"
        /// </summary>
        /// <param name="moveDirection"></param>
        /// <returns>TRUE if they hit a corner low enough to clip over</returns>
        public bool CheckLedge(float moveDirection)
        {
            //Uses the direction to choose the side of the body the rays start from
            //If moving right, move the starting pos to the right, if left, move the starting pos to the left
            float xOffset = (moveDirection > 0) ? m_BodyWidth : -m_BodyWidth;
            float xPos = transform.position.x + xOffset * 1.85f; //sets the pos

            //Uses the direction to determine the way the rays face
            Vector2 dir = (moveDirection > 0) ? Vector2.left : Vector2.right;
            //points left when facing right and right when facing left because the starting point is outside the body

            //Uses the offset to set the position of the feet
            float feetY = transform.position.y - m_FeetYOffset;


            //Makes the legs into a vector 2 using the feet as a starting point
            Vector2 legOrigin = new Vector2(xPos, feetY + m_LegHeight);

            //Makes the feet into a vector 2 and shifts them up slightly
            Vector2 feetOrigin = new Vector2(xPos, feetY + 0.05f);

            bool hitLegs = Physics2D.Raycast(legOrigin, dir, m_LedgeRayLength * 0.2f, m_GroundLayer);

            bool hitFeet = Physics2D.Raycast(feetOrigin, Vector2.up, m_LedgeRayLength, m_GroundLayer);

            return hitFeet && !hitLegs;
        }

        void OnDrawGizmos() //Visualises the boxcast when the gizmos overlay is enabled
        {
            Gizmos.color = m_IsGrounded ? Color.green : Color.red;

            Gizmos.DrawWireCube(transform.position - new Vector3(0, (0.75f), 0), m_BoxSize); //Draws the boxcast area

            Vector2 headCenter = (Vector2)transform.position + new Vector2(0, 0.8f);

            Vector2 leftShoulder = headCenter + new Vector2(-m_BodyWidth / 1.6f, 0);
            Vector2 rightShoulder = headCenter + new Vector2(m_BodyWidth / 1.6f, 0);

            float rayDist = 0.25f;

            Gizmos.DrawRay(leftShoulder, Vector2.left * rayDist);
            Gizmos.DrawRay(rightShoulder, Vector2.right * rayDist);

            Vector2 newCentre = headCenter + new Vector2(-rayDist / 1.7f, 0);
            Gizmos.DrawRay(newCentre, Vector2.right * (rayDist / 0.85f));

            float xPos = transform.position.x + m_BodyWidth * 1.85f;
            float xPos1 = transform.position.x + -m_BodyWidth * 1.85f;

            float feetY = transform.position.y - m_FeetYOffset;

            Vector2 legOrigin = new Vector2(xPos, feetY + m_LegHeight);
            Vector2 feetOrigin = new Vector2(xPos, feetY + 0.05f);

            Vector2 legOrigin1 = new Vector2(xPos1, feetY + m_LegHeight);
            Vector2 feetOrigin1 = new Vector2(xPos1, feetY + 0.05f);

            Gizmos.DrawRay(legOrigin, Vector2.left * m_LedgeRayLength * 0.2f);
            Gizmos.DrawRay(feetOrigin, Vector2.up * m_LedgeRayLength);

            Gizmos.DrawRay(legOrigin1, Vector2.right * m_LedgeRayLength * 0.2f);
            Gizmos.DrawRay(feetOrigin1, Vector2.up * m_LedgeRayLength);
        }
    }
}