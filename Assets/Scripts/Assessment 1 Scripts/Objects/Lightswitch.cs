#region

using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Objects
{
    public class Lightswitch : MonoBehaviour, IInteractable
    {
        [Header("Light and Sprite Settings")] [SerializeField]
        private GameObject m_LightObject;

        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Sprite m_NewSprite;

        [Header("Lever Settings")] [SerializeField]
        private GameObject m_LeverPivot;

        [Header("Door Trigger Settings")] [SerializeField]
        private DoorController m_Doors;

        private Sprite m_OriginalSprite;
        private bool m_Toggled;

        private void Awake()
        {
            m_OriginalSprite = m_SpriteRenderer.sprite;
        }

        public void Interaction()
        {
            if (m_Doors)
            {
                //does nothing if the doors are already moving
                if (m_Doors.IsMoving())
                    return;
                //triggers the door if there is one linked to the lightswitch
                else
                    m_Doors.Interaction();
            }

            m_Toggled = !m_Toggled;

            m_LeverPivot.transform.rotation = m_Toggled
                ? Quaternion.Euler(0f, 0f, 45f)
                : Quaternion.Euler(0f, 0f, 0f); //rotates the lightswitch to indicate its state

            if (m_LightObject) //allows the lightswitch to be toggled using its current state
                m_LightObject.SetActive(!m_LightObject.activeSelf);

            if (m_SpriteRenderer)
                m_SpriteRenderer.sprite =
                    m_SpriteRenderer.sprite == m_OriginalSprite
                        ? m_NewSprite
                        : m_OriginalSprite; //does the same for the light sprite
        }
    }
}