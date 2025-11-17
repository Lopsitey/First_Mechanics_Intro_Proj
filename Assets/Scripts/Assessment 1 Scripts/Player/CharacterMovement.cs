#region

using System.Collections;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Basic Movement Settings")] [SerializeField]
        private float m_MoveSpeed = 5;

        [SerializeField] private float m_JumpStrength = 10;

        [Header("Advanced Movement Settings")] [SerializeField]
        private float m_CoyoteTimeThreshold = 0.15f; //the total duration of the coyote time

        [SerializeField] private float
            m_JumpDelayTime =
                0.2f; //the delay after a jump is executed - the minimum being ~ 0.15 due to the coyote time 

        [SerializeField]
        private float m_JumpBufferTime = 0.2f; //the buffer of time a jump is queued for - before hitting the ground

        [SerializeField]
        private float m_ApexGravity = 0.4f; //the level of gravity at the apex of a jump (1 is normal gravity)

        [SerializeField] private float
            m_ApexGravityTime = 0.2f; //the amount of time you experience reduced gravity at the apex of a jump

        [SerializeField] private float m_MaxJumpCharge = 0.4f; //The longest you can hold a jump to gain more height

        [SerializeField]
        private float m_MinJumpCharge = 0.2f; //The minimum amount of time for a jump to be considered held 

        [Header("Miscellaneous Settings")] [SerializeField]
        private Collider2D m_PlayerCollider;

        private bool m_CanJump = true; //if the player can jump (to prevent double jumps)
        private bool m_JumpHeld;
        private float m_JumpHeldTime;

        private float m_CoyoteTimeCounter; //the decrementing time left after leaving a ledge
        private float m_ApexGravityCounter; //the decrementing time left for reduced gravity at apex
        private float m_JumpBufferCounter; //the decrementing time left for a buffered jump

        private JumpStates m_CurrentState;
        private GroundSensor m_GroundSensor;

        private Coroutine m_CMove;
        private float m_InMove;
        private bool m_IsMoveActive;

        // ReSharper disable once InconsistentNaming
        private Rigidbody2D m_RB;

        private void Awake()
        {
            m_RB = GetComponent<Rigidbody2D>();
            m_GroundSensor = GetComponentInChildren<GroundSensor>();
            m_CurrentState = JumpStates.Grounded;
        }

        private void FixedUpdate()
        {
            if (!m_CanJump || !m_GroundSensor.m_IsGrounded) //essentially if jumped or not grounded
            {
                m_CoyoteTimeCounter -= Time.fixedDeltaTime;
                m_GroundSensor.CheckGround();

                if (m_CurrentState == JumpStates.Rising &&
                    ALMOST_ZERO(m_RB
                        .linearVelocityY)) //changes the state to apex if rising and vertical velocity is near 0
                {
                    m_CurrentState = JumpStates.Apex;
                    m_RB.gravityScale = m_ApexGravity; //reduces gravity at apex by 15% for a smoother transition
                    m_ApexGravityCounter = m_ApexGravityTime; //only ran when the state first changes
                }
            }

            else //if on the ground
            {
                m_CoyoteTimeCounter = m_CoyoteTimeThreshold;
                m_CurrentState = JumpStates.Grounded;
            }

            if (m_CurrentState == JumpStates.Apex) //if at the apex state
            {
                if (m_ApexGravityCounter > 0) //the remaining apex gravity time
                    m_ApexGravityCounter -= Time.fixedDeltaTime;
                else
                    m_CurrentState = JumpStates.Falling; //switches to falling state after apex time is over
            }
            else if (m_CurrentState == JumpStates.Falling)
            {
                m_RB.gravityScale = 1f; //reverts gravity back to normal
            }

            if (m_JumpBufferCounter > 0) //if there is time left in the buffer
            {
                m_JumpBufferCounter -= Time.fixedDeltaTime; //start counting down the remaining buffer
                if (m_CanJump &&
                    m_GroundSensor.m_IsGrounded) //if the jump delay has ended and the player is near the ground
                {
                    StartCoroutine(DoJump(m_JumpDelayTime));
                    m_JumpBufferCounter = 0f; //resets the buffer so you can't spam jump
                }
            }

            if (m_JumpHeld) //charges a jump while the key is held
            {
                if (m_CurrentState == JumpStates.Grounded)
                    m_RB.linearVelocityY = 0f; //Resets Y velocity for consistent jump height

                if (m_JumpHeldTime < m_MaxJumpCharge)
                {
                    m_JumpHeldTime += Time.fixedDeltaTime;
                    if (m_JumpHeldTime >= m_MinJumpCharge)
                    {
                        m_RB.AddForce(Vector2.up * m_JumpStrength,
                            ForceMode2D.Force); //applies jump force while the key is held
                    }
                }
                else if ((m_JumpHeldTime >= m_MaxJumpCharge) && m_CanJump)
                {
                    StartCoroutine(DoJump(m_JumpDelayTime, false));
                    m_JumpHeld = false;
                }
            }
        }

        //TODO more forum posts on potential future interface ideas and more ways I can implement design patters and SOLID principles
        //TODO make a static helper function file with public functions like this?
        private static bool ALMOST_ZERO(float x) => Mathf.Abs(x) < 0.1f; //helper function to check for near-zero values

        IEnumerator C_MoveUpdate()
        {
            while (m_IsMoveActive)
            {
                yield return new WaitForFixedUpdate();
                m_RB.linearVelocityX = m_MoveSpeed * m_InMove;
                m_RB.linearVelocityY =
                    Mathf.Clamp(m_RB.linearVelocityY, -10f, 10f); //clamps vertical velocity to prevent extreme values
            }
        }

        private IEnumerator DoJump(float delay, bool tap = true)
        {
            if (tap)
            {
                m_RB.linearVelocityY = 0f;
                m_RB.AddForce(Vector2.up * m_JumpStrength, ForceMode2D.Impulse);
            }

            m_CoyoteTimeCounter = 0f;
            m_CurrentState = JumpStates.Rising;

            m_CanJump = false;
            yield return new WaitForSeconds(delay);
            m_CanJump = true;
        }

        /// <summary>
        /// Allows the player to drop through semi-solid platforms  
        /// </summary>
        /// <param name="platformCollider">A reference to the platform to drop through</param>
        /// <returns></returns>
        private IEnumerator C_DropThroughPlatform(Collider2D platformCollider)
        {
            // Ignores individual platform
            Physics2D.IgnoreCollision(m_PlayerCollider, platformCollider, true);

            // Waits for the player to pass through
            yield return new WaitForSeconds(0.75f);

            // Tells the physics engine to stop ignoring it
            Physics2D.IgnoreCollision(m_PlayerCollider, platformCollider, false);
        }

        private enum JumpStates
        {
            Grounded,
            Rising,
            Apex,
            Falling
        }

        #region InputFunctions

        public void SetInMove(float direction)
        {
            m_InMove = direction;

            if (m_InMove == 0f)
            {
                m_IsMoveActive = false; //disables the move coroutine if not already moving
            }


            else
            {
                if (m_IsMoveActive)
                    return;

                m_IsMoveActive = true;
                m_CMove = StartCoroutine(C_MoveUpdate());
            }
        }

        public void JumpStarted() //false by default for tap, anything else for hold
        {
            if (m_CanJump)
            {
                if (m_GroundSensor.m_IsGrounded || m_CoyoteTimeCounter > 0f)
                {
                    StartCoroutine(DoJump(m_JumpDelayTime)); //applies jump velocity and delay
                    m_JumpHeld = true; //allows the held jump to end without needing to be grounded
                    m_JumpHeldTime = 0f; //reset the help time when back on the ground
                }
                else
                {
                    m_JumpBufferCounter = m_JumpBufferTime; //resets the jump buffer counter if midair
                }
            }
        }

        public void JumpEnded()
        {
            m_JumpHeld = false;
        }

        public void MoveDown()
        {
            //uses the overloaded version to get the collider and return if on the ground
            if (m_GroundSensor.CheckGround(out var platformCollider))
            {
                //just realised I don't need to do TryGet<comp>(out var compName) and can do the below instead
                //the _ means I'm just not using the out variable, it's called a discard
                if (platformCollider && platformCollider.TryGetComponent(out PlatformEffector2D _))
                {
                    // Allows the player to drop through the platform
                    StartCoroutine(C_DropThroughPlatform(platformCollider));
                }
            }
        }

        #endregion
    }
}