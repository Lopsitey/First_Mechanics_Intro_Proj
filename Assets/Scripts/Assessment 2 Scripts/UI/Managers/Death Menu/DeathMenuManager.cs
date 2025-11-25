#region

using Assessment_2_Scripts.Managers;
using Assessment_2_Scripts.Player;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Assessment_2_Scripts.UI.Managers.Death_Menu
{
    public class DeathMenuManager : MonoBehaviour
    {
        private UIDocument m_UIDocument; //whole doc
        private VisualElement m_DeathContainer; //main container
        private Button m_RespawnButton;
        private Button m_QuitButton;
        private Label m_DeathLbl; //the "you were killed by" text 

        private void Awake()
        {
            //Gets the main container
            m_UIDocument = GetComponent<UIDocument>();
            VisualElement container = m_UIDocument.rootVisualElement;

            //gets the subcomponents made in the design doc and assigns them to the relevant vars
            m_DeathContainer = container.Q<VisualElement>("container");
            m_RespawnButton = container.Q<Button>("RespawnBtn");
            m_QuitButton = container.Q<Button>("QuitBtn");
            m_DeathLbl = container.Q<Label>("DeathLbl");

            // Hide it by default
            m_DeathContainer.style.display = DisplayStyle.None;
        }

        private void OnEnable()
        {
            // Subscribe to button clicks
            m_RespawnButton.clicked += HandleRespawn;
            m_QuitButton.clicked += HandleQuit;
        }

        private void OnDisable()
        {
            //Unsubscribes
            m_RespawnButton.clicked -= HandleRespawn;
            m_QuitButton.clicked -= HandleQuit;
        }

        //Finds the new health comp when the player spawns
        public void Init(HealthComponent healthComp)
        {
            healthComp.OnDeath += ShowDeathScreen;
        }

        //Reveals the death screen on death
        private void ShowDeathScreen(MonoBehaviour instigator)
        {
            m_DeathContainer.style.display = DisplayStyle.Flex;
            m_DeathLbl.text = "You were killed by: " + instigator.gameObject.name + "!";
        }

        //When the respawn button is pressed
        private void HandleRespawn()
        {
            //Tells the ExistenceManager to respawn the player
            ExistenceManager.Instance.SpawnPlayer();

            //Hides the screen again
            m_DeathContainer.style.display = DisplayStyle.None;
        }

        //Exits the game
        private void HandleQuit()
        {
            Application.Quit();
        }
    }
}