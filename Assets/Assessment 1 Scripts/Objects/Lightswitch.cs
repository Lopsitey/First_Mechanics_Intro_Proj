using UnityEngine;

namespace Assessment_1_Scripts
{
    public class Lightswitch : MonoBehaviour, IInteractible
    {
        [SerializeField] private GameObject m_LightObject;
        
        public void Interact()
        {
            m_LightObject.SetActive(!m_LightObject.activeSelf);//allows the lightswitch to be toggled using it's current state
        }
    }
}