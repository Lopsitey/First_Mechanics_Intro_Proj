#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class InteractionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask m_InteractLayer; //the layer interactable are on

        public void Awake()
        {
            //m_InteractLayer = LayerMask.NameToLayer("Interactable"); TODO
        }

        public void Interact_LightSwitch()
        {
            Collider2D colCircle = Physics2D.OverlapCircle(transform.position, 1f, m_InteractLayer);

            //Checks if the overlap object has the correct interface component
            if (colCircle && colCircle.transform.TryGetComponent<IInteractible>(out var interactable))
            {
                interactable.Interact(); //if it does - interact
            }
        }
    }
}