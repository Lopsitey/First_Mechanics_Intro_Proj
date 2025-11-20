#region

using System.Collections;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement Speed")] [SerializeField]
        private float m_GroundMoveSpeed = 5;

        [SerializeField] private float m_AirMoveSpeed = 8f;

        [Header("Jumping")] [SerializeField] private float m_JumpStrength = 10;

        //the delay after a jump is executed - the minimum being ~ 0.15 due to the coyote time
        [SerializeField] private float m_JumpDelayTime = 0.2f;

        //The minimum amount of time for a jump to be considered held
        [SerializeField] private float m_MinJumpCharge = 0.2f;

        //The longest you can hold a jump to gain more height
        [SerializeField] private float m_MaxJumpCharge = 0.4f;

        //the total duration of the coyote time
        [SerializeField] private float m_CoyoteTimeThreshold = 0.15f;

        //the buffer of time a jump is queued for - before hitting the ground
        [SerializeField] private float m_JumpBufferTime = 0.2f;

        //the level of gravity at the apex of a jump (1 is normal gravity)
        [SerializeField] private float m_ApexGravity = 0.4f;

        //the amount of time you experience reduced gravity at the apex of a jump
        [SerializeField] private float m_ApexGravityTime = 0.2f;

        //How far to nudge the player when they bump their head or clip a corner
        [Header("Bumped Head & Corner Correction")] [SerializeField]
        private float m_NudgeForce = 0.5f;

        [SerializeField] private float m_NudgeCooldown = 0.2f; //How long to wait between nudges
        private float m_NudgeTimer; //Tracks the cooldown

        [Header("Miscellaneous")] [SerializeField]
        private Collider2D m_PlayerCollider;

        //if the player can jump (to prevent double jumps)
        private bool m_CanJump = true;
        private bool m_JumpHeld;
        private float m_JumpHeldTime;

        //time left after leaving a ledge
        private float m_CoyoteTimeCounter;

        //time left for reduced gravity at apex
        private float m_ApexGravityCounter;

        //time left for a buffered jump
        private float m_JumpBufferCounter;
        //all counters decrement

        private JumpStates m_CurrentState;
        private float m_CurrentMoveSpeed;
        private float m_DefaultGravity;
        private GroundSensor m_GroundSensor;

        // ReSharper disable once InconsistentNaming
        private Rigidbody2D m_RB;

        private Coroutine m_CMove;
        private float m_InMove;

        private void Awake()
        {
            m_RB = GetComponent<Rigidbody2D>();
            m_GroundSensor = GetComponentInChildren<GroundSensor>();
            m_CurrentState = JumpStates.Grounded;
            m_CurrentMoveSpeed = m_GroundMoveSpeed;
            m_DefaultGravity = m_RB.gravityScale;
        }

        private void FixedUpdate()
        {
            //Only runs if a nudge previously occured to set the timer 
            if (m_NudgeTimer > 0)
            {
                m_NudgeTimer -= Time.fixedDeltaTime;
            }

            switch (m_CurrentState)
            {
                case JumpStates.Grounded:
                    //Coyote counter is set, so it can decrement
                    m_CoyoteTimeCounter = m_CoyoteTimeThreshold;

                    //the ground sensor can detect if the 
                    if (!m_GroundSensor.m_IsGrounded) // if in the air
                    {
                        m_CurrentState = m_RB.linearVelocityY < 0 ? JumpStates.Falling : JumpStates.Rising;
                    }

                    break;

                case JumpStates.Rising:
                    //m_GroundSensor.CheckGround();//boxcast for an accurate ground check and for the input buffer
                    if (m_NudgeTimer <= 0)
                        CheckNudges();

                    m_CoyoteTimeCounter -= Time.fixedDeltaTime;
                    //changes the state to apex if rising and vertical velocity is near 0
                    if (ALMOST_ZERO(m_RB.linearVelocityY, 1f)) //if the y velocity is within 1 of zero
                    {
                        //essentially a perfect jump
                        m_CurrentState = JumpStates.Apex;
                        m_RB.gravityScale = m_ApexGravity; //reduces gravity at apex for a smoother transition
                        m_ApexGravityCounter = m_ApexGravityTime; //only ran when the state first changes
                    }
                    //Landed early (immediate head bonk)
                    else if (m_GroundSensor.m_IsGrounded)
                        m_CurrentState = JumpStates.Grounded;

                    break;

                case JumpStates.Apex:
                    //Decrements the timer
                    m_ApexGravityCounter -= Time.fixedDeltaTime;

                    //the remaining apex gravity time
                    if (m_ApexGravityCounter <= 0)
                    {
                        //switches to falling state after apex time is over
                        m_CurrentState = JumpStates.Falling;
                    }
                    else if (m_GroundSensor.m_IsGrounded) //Or if landed early for some reason
                    {
                        m_CurrentState = JumpStates.Falling; //Go to Falling for one frame
                        //Completes the cycle
                    }

                    break;

                case JumpStates.Falling:
                    m_GroundSensor.CheckGround();
                    //clamps vertical velocity to prevent extreme values
                    m_RB.linearVelocityY = Mathf.Clamp(m_RB.linearVelocityY, -12f, 30f);

                    m_RB.gravityScale = m_DefaultGravity; // Make sure gravity is normal
                    m_CoyoteTimeCounter -= Time.fixedDeltaTime;

                    //When inevitably grounded
                    if (m_GroundSensor.m_IsGrounded)
                        m_CurrentState = JumpStates.Grounded;
                    break;
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

        /// <summary>
        /// Checks if the argument is close to zero, returns true if it is, false if not
        /// </summary>
        /// <param name="x">The value to check</param>
        /// <param name="y">How close to zero the value has to be</param>
        /// <returns>Boolean</returns>
        private static bool ALMOST_ZERO(float x, float y) => Mathf.Abs(x) < y;

        IEnumerator C_MoveUpdate()
        {
            while (m_InMove != 0)
            {
                yield return new WaitForFixedUpdate();
                // Determines which type of speed to use
                m_CurrentMoveSpeed = m_GroundSensor.m_IsGrounded ? m_GroundMoveSpeed : m_AirMoveSpeed;
                m_RB.linearVelocityX = m_CurrentMoveSpeed * m_InMove;
            }

            //if not moving, set the coroutine reference to null so it can be started again
            m_CMove = null;
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


        /// <summary>
        ///Applies nudges if your legs clip a corner or if the edges of your head clip a corner  
        /// </summary>
        private void CheckNudges()
        {
            //Returns the side of the head hit
            int hitDir = m_GroundSensor.CheckCeiling();

            if (hitDir > 1)
            {
                Vector2 pos = m_RB.position;
                if (hitDir == 2) pos.x += m_NudgeForce; //if hit the left shoulder nudge right
                else if (hitDir == 3) pos.x -= m_NudgeForce; //if hit the right shoulder nudge left

                //Moves player if collision hit a shoulder
                m_RB.MovePosition(pos);

                //Starts the cooldown
                m_NudgeTimer = m_NudgeCooldown;
                return; //Don't try to ledge grab in the same frame
            }

            // Only checks if moving sideways
            if (m_InMove != 0)
            {
                //If hit a corner
                if (m_GroundSensor.CheckLedge(m_InMove))
                {
                    Vector2 targetPos = m_RB.position;
                    targetPos.y += 0.25f; //Upwards nudge height
                    m_RB.MovePosition(targetPos);

                    //Starts the cooldown
                    m_NudgeTimer = m_NudgeCooldown;
                }
            }
        }

        #region InputFunctions

        public void SetInMove(float direction)
        {
            m_InMove = direction;
            if (m_InMove != 0f) //if moving
            {
                //if no coroutine was running, initialise it
                m_CMove ??= StartCoroutine(C_MoveUpdate());
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

        void OnDrawGizmos() //Visualises the current state
        {
            Gizmos.color = m_CurrentState == JumpStates.Rising ? Color.cyan :
                m_CurrentState == JumpStates.Apex ? Color.gold :
                m_CurrentState == JumpStates.Falling ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}