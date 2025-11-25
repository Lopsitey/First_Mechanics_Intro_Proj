#region

using System;
using System.Collections;
using Assessment_2_Scripts.Player;
using Assessment_2_Scripts.Utilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class ADDED_DASH : MonoBehaviour
    {
        [Header("Movement Speed")] [SerializeField]
        private float m_GroundMoveSpeed = 5;

        [SerializeField] private float m_AirMoveSpeed = 8f;

        [Header("Jumping")] [SerializeField] private float m_JumpStrength = 10;

        //the delay after a jump is executed - the minimum being ~ 0.15 due to the coyote time
        [SerializeField] private float m_JumpCooldown = 0.2f;

        //The minimum amount of time for a jump to be considered held
        [SerializeField] private float m_MinJumpCharge = 0.2f;

        //The longest you can hold a jump to gain more height
        [SerializeField] private float m_MaxJumpCharge = 0.4f;

        //The amount of friction the sticky feet on landing mechanic applies
        [SerializeField] private float m_LandingFriction = 0.2f;

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

        [Header("Dashing")] [SerializeField] private float m_DashForce = 25f;

        [SerializeField] private float m_DashDuration = 0.25f; //How long the dash can occur for 

        //Must be a minimum of 0.2 for friction calculations to work
        [SerializeField] [Range(0.2f, 2)] private float m_DashCooldownTime = 0.2f; //Time before another dash can occur

        [SerializeField]
        private float m_MovementLockDuration = 0.5f; //Time that movement is locked after a dash is executed

        [SerializeField] [Range(0, 0.5f)] private float m_PostDashFriction = 0.25f;
        [SerializeField] [Range(0, 0.5f)] private float m_SlowMoScale = 0.25f; //a range between 0 and 0.5

        [SerializeField] private Light2D m_AimLight;

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

        //time left for the movement to be re-enabled
        private float m_MovementLockCounter;
        //all counters decrement

        private Vector3 m_MousePos;

        private JumpStates m_CurrentState;
        private float m_CurrentMoveSpeed;
        private float m_DefaultGravity;
        private bool m_PostDashFrictionEnabled;
        private GroundSensor m_GroundSensor;

        // ReSharper disable once InconsistentNaming
        private Rigidbody2D m_RB;

        private Coroutine m_CDash;
        private Coroutine m_CMove;
        private float m_InMove;
        private Camera m_Camera;
        public event Action OnJump;
        public event Action OnDash;
        public event Action OnAim;

        private void Awake()
        {
            m_Camera = Camera.main;
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
                    //Updates the variables for the input buffer and jumping
                    m_GroundSensor.CheckGround(); //boxcast for a more accurate ground check
                    if (m_NudgeTimer <= 0)
                        CheckNudges();

                    m_CoyoteTimeCounter -= Time.fixedDeltaTime;
                    //changes the state to apex if rising and vertical velocity is near 0
                    if (GameHelpers.ALMOST_ZERO(m_RB.linearVelocityY, 1f)) //if the y velocity is within 1 of zero
                    {
                        //essentially a perfect jump
                        m_CurrentState = JumpStates.Apex;
                        m_RB.gravityScale = m_ApexGravity; //reduces gravity at apex for a smoother transition
                        m_ApexGravityCounter = m_ApexGravityTime; //only ran when the state first changes
                    }

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
                    else
                    {
                        if (m_GroundSensor.m_IsGrounded) //Or if landed early for some reason
                            m_CurrentState = JumpStates.Falling; //Go to Falling for one frame
                        //Completes the cycle
                    }

                    break;

                case JumpStates.Falling:
                    //clamps vertical velocity to prevent extreme values
                    m_RB.linearVelocityY = Mathf.Clamp(m_RB.linearVelocityY, -12f, 30f);

                    m_RB.gravityScale = m_DefaultGravity; // Make sure gravity is normal
                    m_CoyoteTimeCounter -= Time.fixedDeltaTime;

                    //When inevitably grounded
                    if (m_GroundSensor.CheckGround(out _))
                    {
                        //Checks if landing while trying to stop OR turn around
                        bool tryingToStop = GameHelpers.ALMOST_ZERO(m_InMove, 0.01f); //if within 0.01 of 0

                        //If input is left and the player is moving right or vice versa
                        bool tryingToTurn = (m_InMove < 0 && m_RB.linearVelocityX > 0) ||
                                            (m_InMove > 0 && m_RB.linearVelocityX < 0);

                        if (tryingToStop || tryingToTurn)
                        {
                            //Reduces horizontal momentum
                            m_RB.linearVelocityX *= m_LandingFriction;
                        }

                        m_CurrentState = JumpStates.Grounded;
                    }

                    break;
            }

            if (m_JumpBufferCounter > 0) //if there is time left in the buffer
            {
                m_JumpBufferCounter -= Time.fixedDeltaTime; //start counting down the remaining buffer
                if (m_CanJump &&
                    m_GroundSensor.m_IsGrounded) //if the jump delay has ended and the player is near the ground
                {
                    StartCoroutine(C_DoJump(m_JumpCooldown));
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
                        //the only time I should be using AddForce
                        m_RB.AddForce(Vector2.up * (m_JumpStrength * 2.5f),
                            ForceMode2D.Force); //applies jump force while the key is held
                    }
                }
                else if ((m_JumpHeldTime >= m_MaxJumpCharge) && m_CanJump)
                {
                    StartCoroutine(C_DoJump(m_JumpCooldown, false));
                    m_JumpHeld = false;
                }
            }

            if (m_MovementLockCounter > 0) //prevents movement while active
            {
                m_MovementLockCounter -= Time.fixedDeltaTime;
            }

            //if just dashed and the dash is on cooldown
            if (m_PostDashFrictionEnabled && m_CDash != null)
            {
                //Applies friction after the dash
                m_RB.linearVelocityX *= m_PostDashFriction;
            }
        }

        private void Update()
        {
            if (m_CurrentState == JumpStates.Aiming)
                Aim();
        }

        IEnumerator C_MoveUpdate()
        {
            while (m_InMove != 0)
            {
                yield return new WaitForFixedUpdate();

                //Prevents movement if dashing
                if (!(m_MovementLockCounter > 0))
                {
                    //Determines which type of speed to use
                    m_CurrentMoveSpeed = m_GroundSensor.m_IsGrounded ? m_GroundMoveSpeed : m_AirMoveSpeed;
                    m_RB.linearVelocityX = m_CurrentMoveSpeed * m_InMove;
                }
            }

            //if not moving, set the coroutine reference to null so it can be started again
            m_CMove = null;
        }

        /// <summary>
        /// Handles the force of tap jumps, unsetting the coyote timer and starting the jump cooldown 
        /// </summary>
        /// <param name="delay">The jump cooldown</param>
        /// <param name="tap">False by default for tap, true else for held jumps</param>
        /// <returns></returns>
        private IEnumerator C_DoJump(float delay, bool tap = true)
        {
            OnJump?.Invoke(); //starts the jump vfx when you actually jump
            if (tap)
            {
                //This is the most precise way for a normal platformer, it overwrites velocity, so I don't need to reset it
                m_RB.linearVelocity = new Vector2(m_RB.linearVelocity.x, m_JumpStrength);
                //add force ForceMode2D.Impulse is only really good for balloons, 3D or ragdoll physics stuff
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
            Falling,

            Aiming
            //removed the post-dash state as is works netter as a bool
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

        /// <summary>
        /// Handles the rotation of the aiming light 
        /// </summary>
        private void Aim()
        {
            if (m_Camera)
            {
                m_MousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            }

            //Calculates the vector difference between the player and the mouse
            Vector2 lookDir = (m_MousePos - transform.position).normalized;
            //Positive would be left, negative for right

            //Smoothly rotates from the default (up direction - 0,0,0) to the target direction
            Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, lookDir);

            //Rotates the light around the z axis if it exists
            if (m_AimLight)
            {
                m_AimLight.transform.rotation = targetRot;
            }
        }

        /// <summary>
        /// Executes the dash after the aiming state, uses two delays for friction and a cooldown 
        /// </summary>
        /// <returns></returns>
        private IEnumerator C_DoDash() //this is a coroutine because it is a sequence which will never be queried 
        {
            OnDash?.Invoke(); //played the dashed vfx and stops the charging vfx

            //Resets time and physics 
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            Vector3 rawDir = m_MousePos - transform.position;
            //Flattens the z so it doesn't interfere with calculations
            rawDir.z = 0;
            Vector2 direction = rawDir.normalized;
            //Applies the force in the direction directly to the linearVelocity Vector2
            m_RB.linearVelocity = direction * m_DashForce;

            //resets the timer for the movement being locked post-dash
            m_MovementLockCounter = m_MovementLockDuration;

            if (m_AimLight) m_AimLight.gameObject.SetActive(false);

            //Sets the state if you're not on the ground
            if (m_GroundSensor.CheckGround(out _) == false)
                m_CurrentState = m_RB.linearVelocityY > 0 ? JumpStates.Rising : JumpStates.Falling;
            else
                m_CurrentState = JumpStates.Grounded;

            //How long to wait before applying friction 
            yield return new WaitForSeconds(m_DashDuration);

            m_PostDashFrictionEnabled = true;

            //must be at least 0.2f
            const float postDashFrictionTime = 0.2f;
            yield return new WaitForSeconds(postDashFrictionTime);
            m_PostDashFrictionEnabled = false;

            //Time before another dash can happen - the minimum friction time
            yield return new WaitForSeconds(m_DashCooldownTime - postDashFrictionTime);

            //allows the coroutine to be started again using the ??= operator
            m_CDash = null;
        }

        #region InputFunctions

        public void SetInMove(float direction)
        {
            //Prevents movement when a dash is executed

            m_InMove = direction;
            if (m_InMove != 0f) //if moving
            {
                //if no coroutine was running, initialise it
                m_CMove ??= StartCoroutine(C_MoveUpdate());
            }
        }

        public void JumpStarted()
        {
            //Prevents jumping whilst aiming for a dash
            if (m_CurrentState == JumpStates.Aiming)
                return;

            if (m_CanJump)
            {
                if (m_GroundSensor.m_IsGrounded || m_CoyoteTimeCounter > 0f)
                {
                    StartCoroutine(C_DoJump(m_JumpCooldown)); //applies jump velocity and delay
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

        public void AimStarted()
        {
            //if the dash isn't on cooldown
            if (m_CDash == null)
            {
                OnAim?.Invoke(); //plays the charging vfx so it starts its loop
                m_CurrentState = JumpStates.Aiming;
                //Slows down time
                Time.timeScale = m_SlowMoScale;
                //Ensures smooth physics
                Time.fixedDeltaTime = 0.02f * m_SlowMoScale;

                //Turns on the light
                if (m_AimLight) m_AimLight.gameObject.SetActive(true);
            }
        }

        public void AimEnded()
        {
            //if you were aiming - not on dash cooldown or anything
            if (m_CurrentState == JumpStates.Aiming)
            {
                //only start a new coroutine if one isn't already running
                m_CDash ??= StartCoroutine(C_DoDash());
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