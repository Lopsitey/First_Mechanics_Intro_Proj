#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Player
{
    public class InteractionHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask m_InteractLayer; //the layer interactables are on
        [SerializeField] private int m_MaxOverlaps = 3; //max number of interactables that can be detected at once

        private Collider2D[] m_OverlapResults; //array to store overlap results
        private ContactFilter2D m_ContactFilter; //filter to define what layers to check for overlaps

        private void Awake()
        {
            //set the contact filter to only check for the interactable layer
            m_ContactFilter.SetLayerMask(m_InteractLayer);

            //if the overlap hits a trigger collider, it will be included in the results
            m_ContactFilter.useTriggers = true;

            m_OverlapResults = new Collider2D[m_MaxOverlaps];
        }

        public void Interact()
        {
            //box cast to detect any interactables in range
            int overlapsFound = Physics2D.OverlapBox(transform.position - new Vector3(0, 0.02f, 0),
                new Vector2(1.1f, 1.95f), 0.0f, m_ContactFilter, m_OverlapResults);

            //Checks if any of the overlap objects have the correct interface component
            for (int i = 0; i < overlapsFound; ++i)
            {
                Collider2D result = m_OverlapResults[i]; //the current collider being checked
                //if the collider is valid and the collider has a component that implements the IInteractible interface
                if (result && result.TryGetComponent<IInteractable>(
                        out var interactable))
                {
                    interactable.Interaction(); //assign it to a var and call its interaction method
                } //TODO forum post on this and why it doesn't do a loop through every interactable script on an object
            }
        }

        void OnDrawGizmos() //Visualizes the boxcast when the gizmos overlay is enabled
        {
            Gizmos.DrawWireCube(transform.position - new Vector3(0, 0.02f, 0),
                new Vector2(1.1f, 1.95f)); //Draws the box cast area
        }
    }
}