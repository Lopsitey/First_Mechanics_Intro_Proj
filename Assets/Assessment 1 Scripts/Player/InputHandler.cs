using Assessment_1_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private CharacterMovement m_CharacterMovement;
    
    private LayerMask m_InteractLayer;//the layer interactibles are on
    private PlayerControls m_ActionMap;

    #region Bindings
    private void Awake()
    {
        m_ActionMap = new PlayerControls();
        m_InteractLayer = LayerMask.NameToLayer("Interactable");
    }
    
    private void OnEnable()
    {
        m_ActionMap.Enable();

        m_ActionMap.Default.MoveHoriz.performed += Handle_MovePerformed;
        m_ActionMap.Default.MoveHoriz.canceled += Handle_MoveCancelled;
        m_ActionMap.Default.Jump.performed += Handle_JumpStarted;
        m_ActionMap.Default.Jump.canceled += Handle_JumpCancelled;
        m_ActionMap.Default.Interact.performed += Handle_InteractPerformed;
    }

    private void OnDisable()
    {
        m_ActionMap.Disable();

        m_ActionMap.Default.MoveHoriz.performed -= Handle_MovePerformed;
        m_ActionMap.Default.MoveHoriz.canceled -= Handle_MoveCancelled;
        m_ActionMap.Default.Jump.started -= Handle_JumpStarted;
        m_ActionMap.Default.Jump.canceled -= Handle_JumpCancelled;
        m_ActionMap.Default.Interact.performed -= Handle_InteractPerformed;
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

    private void Handle_JumpStarted(InputAction.CallbackContext context)
    {
        m_CharacterMovement.JumpStarted();
    }
    void Handle_JumpCancelled(InputAction.CallbackContext context)
    {
        m_CharacterMovement.JumpEnded();//if the button was lifted
    }

    private void Handle_InteractPerformed(InputAction.CallbackContext context)
    {
        Collider2D colCircle = Physics2D.OverlapCircle(transform.position, 1f, m_InteractLayer);

        //Checks if the overlap object has the correct interface component
        if (colCircle && colCircle.transform.TryGetComponent<IInteractible>(out var interactible))
        {
            interactible.Interact();//if it does - interact
        }
    }
    #endregion
}
