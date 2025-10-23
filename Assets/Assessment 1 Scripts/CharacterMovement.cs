using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	private Rigidbody2D m_RB;

	[Header("Basic Movement Settings")]
	[SerializeField] private float m_MoveSpeed = 5;
	[SerializeField] private float m_JumpStrength = 10;
	private bool m_Jumped = false;//if the player has jumped (to prevent double jumps)
	
	[Header("Advanced Movement Settings")]
	[SerializeField] private float m_CoyoteTimeThreshold = 0.15f;//the total duration of the coyote time
	private float m_CoyoteTimeCounter;//the decrementing time left after leaving a ledge
	[SerializeField] private float m_JumpDelayTime = 0.2f;//the delay after a jump is executed - the minimum being ~ 0.15 due to the coyote time 
	
	private GroundSensor m_GroundSensor;
	private float m_InMove;

	private void Awake()
	{
		m_RB = GetComponent<Rigidbody2D>();
		m_GroundSensor=GetComponentInChildren<GroundSensor>();
	}

	

    #region InputFunctions

    public void MovePerformed(float direction)
    {
        m_InMove = direction;
    }
    public void MoveCancelled()
    {
        m_InMove = 0f;
    }

    public void JumpPerformed()
    {
	    if (!m_Jumped)
	    {
		    if (m_GroundSensor.m_IsGrounded || m_CoyoteTimeCounter > 0f)
		    {
			    m_RB.AddForce(Vector2.up * m_JumpStrength, ForceMode2D.Impulse);
			    m_CoyoteTimeCounter = 0f;
			    StartCoroutine(JumpDelay(m_JumpDelayTime));
		    }
	    }
    }
    #endregion

    private void FixedUpdate()
	{
		m_RB.linearVelocityX = m_MoveSpeed * m_InMove;
		
        
        if (m_GroundSensor.m_IsGrounded)
        {
	        m_CoyoteTimeCounter=m_CoyoteTimeThreshold;
        }
        else
	        m_CoyoteTimeCounter-=Time.deltaTime;
	}

	private IEnumerator JumpDelay(float delay)
	{
		m_Jumped = true;
		yield return new WaitForSeconds(delay);
		m_Jumped = false;
	}
}
