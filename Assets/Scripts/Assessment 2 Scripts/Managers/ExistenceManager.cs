#region

using Assessment_2_Scripts.Player;
using Assessment_2_Scripts.UI.Managers.Death_Menu;
using Assessment_2_Scripts.UI.Managers.HUD;
using Unity.Cinemachine;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Managers
{
    //doesn't need to inherit from MonoBehaviour because Singleton already does
    /// <summary>
    /// Manages the spawning and destruction of objects
    /// </summary>
    public class ExistenceManager : Singleton<ExistenceManager>
    {
        //TODO public event Action OnPlayerSpawned; this should be done in the future to decouple all of these systems from the manager

        [SerializeField] private CinemachineCamera m_PlayerCamera; //the CineMachine camera which follows the player

        [SerializeField] private GameObject m_PlayerPrefab; //the player to be spawned

        [SerializeField] private HUDManager m_HUD; //the HUD creation script

        [SerializeField] private DeathMenuManager m_DeathMenu;

        private GameObject m_PlayerRef; //holds a reference to the player once spawned

        void Start()
        {
            SpawnPlayer();
            //create HUD once for player passing PlayerWrapper for data binding
            m_HUD.CreateHUD(m_PlayerRef.GetComponent<PlayerWrapper>());
            //creates the respawn menu once
            m_DeathMenu.CreateMenu(m_PlayerRef.GetComponent<HealthComponent>());
        }

        public void SpawnPlayer(MonoBehaviour instigator = null)
        {
            if (instigator != null) //killed by something
            {
                m_HUD.gameObject.SetActive(false); //deactivates HUD on death
            }

            m_PlayerRef = Instantiate(m_PlayerPrefab); //spawn player

            if (m_PlayerRef)
            {
                m_PlayerRef.GetComponent<CharacterManager>().Init(); //initialise player character manager

                m_HUD.gameObject.SetActive(true); //toggles HUD on again

                m_PlayerCamera.Follow = m_PlayerRef.transform; //initialises the camera to follow the player
            }
        }

        /// <summary>
        /// Gets the player, returns true if found, false if null
        /// </summary>
        /// <param name="player">The player object to return</param>
        /// <returns>Bool</returns>
        public bool TryGetPlayer(out GameObject player)
        {
            if (m_PlayerRef)
                player = m_PlayerRef;
            else
            {
                player = null;
                return false;
            }

            return true;
        }
    }
}