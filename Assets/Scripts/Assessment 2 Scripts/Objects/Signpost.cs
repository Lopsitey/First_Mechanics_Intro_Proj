#region

using Assessment_2_Scripts.Interfaces;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class Signpost : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject m_PopupCanvas; //the canvas the sign text is on

        public void Interaction()
        {
            // Toggles the sign text on interaction
            if (m_PopupCanvas)
            {
                m_PopupCanvas.SetActive(!m_PopupCanvas.activeSelf);
            }
        }
    }
}