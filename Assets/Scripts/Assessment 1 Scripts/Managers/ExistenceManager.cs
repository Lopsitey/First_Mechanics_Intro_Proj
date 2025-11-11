#region

using Assessment_1_Scripts.Objects;
using Assessment_1_Scripts.Player;
using Assessment_1_Scripts.UI.Managers.HUD;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Managers
{
    //doesn't need to inherit from MonoBehaviour because Singleton already does
    /// <summary>
    /// Manages the spawning and destruction of objects
    /// </summary>
    public class ExistenceManager : Singleton<ExistenceManager>
    {
        [SerializeField] private GameObject m_MainCamera; //the main camera for the every scene

        [SerializeField] private GameObject m_PlayerPrefab; //the player to be spawned

        [SerializeField] private HUDManager m_HUD; //the HUD creation script

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
                m_HUD.gameObject.SetActive(false); //deactivates HUD on death
            }

            m_PlayerRef = Instantiate(m_PlayerPrefab); //spawn player

            if (m_PlayerRef)
            {
                m_PlayerRef.GetComponent<CharacterManager>().Init(); //initialise player character manager
                m_PlayerRef.GetComponent<HealthComponent>().OnDeath += SpawnPlayer; //resubscribes to death event
                m_HUD.gameObject.SetActive(true);
                //create HUD for player passing TransformWrapper for data binding
                m_HUD.CreateHUD(m_PlayerRef.GetComponent<TransformWrapper>());
            }

            if (m_MainCamera.TryGetComponent<CameraInitialisation>(out var cameraInit))
                cameraInit.Init(m_PlayerRef); //initialise the camera to follow the player
        }

        private void OnDisable() //defensive programming
        {
            if (m_PlayerRef)
                if (m_PlayerRef.TryGetComponent<HealthComponent>(out var healthComp))
                    healthComp.OnDeath -= SpawnPlayer;
        }
    }
}