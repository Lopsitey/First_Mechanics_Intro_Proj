#region

using Assessment_1_Scripts.Objects;
using Assessment_1_Scripts.Player;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_MainCamera; //the main camera for the every scene

        [SerializeField] private GameObject m_PlayerPrefab; //the player to be spawned

        private GameObject m_PlayerRef; //holds a reference to the player once spawned

        void Start()
        {
            SpawnPlayer();
        }

        private void SpawnPlayer(MonoBehaviour instigator = null)
        {
            if (instigator != null) //killed by something
            {
                m_PlayerRef.GetComponent<HealthComponent>().OnDeath -= SpawnPlayer;
            }

            m_PlayerRef = Instantiate(m_PlayerPrefab); //spawn player

            if (m_PlayerRef)
            {
                m_PlayerRef.GetComponent<CharacterManager>().Init(); //initialize player character manager
                m_PlayerRef.GetComponent<HealthComponent>().OnDeath += SpawnPlayer; //resubscribes to death event
            }

            if (m_MainCamera.TryGetComponent<CameraInitialisation>(out var cameraInit))
                cameraInit.Init(m_PlayerRef); //initialize the camera to follow the player
        }

        private void OnDisable() //defensive programming
        {
            if (m_PlayerRef.TryGetComponent<HealthComponent>(out var healthComp))
                healthComp.OnDeath -= SpawnPlayer;
        }
    }
}