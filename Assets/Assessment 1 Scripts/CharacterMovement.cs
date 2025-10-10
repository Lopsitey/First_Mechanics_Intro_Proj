using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
	private Rigidbody2D m_RB;

	[SerializeField] private float m_MoveSpeed;
	[SerializeField] private float m_JumpStrength;
	[SerializeField] private Transform m_RaycastPosition;
	[SerializeField] private LayerMask m_GroundLayer;
	private float m_InMove;
	private bool m_IsGrounded;

	private void Awake()
	{
		m_RB = GetComponent<Rigidbody2D>();
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
        if (m_IsGrounded)
        {
            m_RB.AddForce(Vector2.up * m_JumpStrength, ForceMode2D.Impulse);
        }
    }
    #endregion


	private void FixedUpdate()
	{
		m_RB.linearVelocityX = m_MoveSpeed * m_InMove;

        m_IsGrounded = Physics2D.Raycast(m_RaycastPosition.position, Vector2.down, 0.1f, m_GroundLayer);
	}
}
