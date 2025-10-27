using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private CharacterMovement m_CharacterMovement;
    private PlayerControls m_ActionMap;

    #region Bindings
    private void Awake()
    {
        m_ActionMap = new PlayerControls();
    }
    
    private void OnEnable()
    {
        m_ActionMap.Enable();

        m_ActionMap.Default.MoveHoriz.performed += Handle_MovePerformed;
        m_ActionMap.Default.MoveHoriz.canceled += Handle_MoveCancelled;
        m_ActionMap.Default.Jump.performed += Handle_JumpPerformed;
    }

    private void OnDisable()
    {
        m_ActionMap.Disable();

        m_ActionMap.Default.MoveHoriz.performed -= Handle_MovePerformed;
        m_ActionMap.Default.MoveHoriz.canceled -= Handle_MoveCancelled;
        m_ActionMap.Default.Jump.performed -= Handle_JumpPerformed;
    }

    #endregion
    
    #region InputFunctions
    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        m_CharacterMovement.SetInMove(context.ReadValue<float>());
    }
    private void Handle_MoveCancelled(InputAction.CallbackContext context)
    {
        m_CharacterMovement.SetInMove(0);
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext context)
    {
        m_CharacterMovement.JumpPerformed();
    }
    
    #endregion
}
