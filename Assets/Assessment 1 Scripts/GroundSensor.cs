using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private Transform m_RaycastPosition;
    [SerializeField] private LayerMask m_GroundLayer;
    public bool m_IsGrounded { get; private set; }

    private void OnEnable()
    {
        CollisionDetector.CollisionEntered += Handle_CollisionEntered;
    }
    
    private void OnDisable()
    {
        CollisionDetector.CollisionEntered -= Handle_CollisionEntered;
    }

    void Handle_CollisionEntered(bool isColliding)
    {
        if (isColliding)
            m_IsGrounded = Physics2D.BoxCast(m_RaycastPosition.position, Vector2.one, 0.1f, Vector2.down,0.1f, m_GroundLayer);
    }
}
