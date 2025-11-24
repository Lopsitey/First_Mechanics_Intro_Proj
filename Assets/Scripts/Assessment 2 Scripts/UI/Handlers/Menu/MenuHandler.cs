#region

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

#endregion

namespace Assessment_2_Scripts.UI.Handlers.Menu
{
    public class MenuHandler : MonoBehaviour
    {
        private UIDocument m_UIDocument;
        private Button m_Play;
        private Button m_Exit;

        private void Awake()
        {
            m_UIDocument = GetComponent<UIDocument>();
            if (m_UIDocument == null)
                Debug.LogError("No UIDocument found");

            m_Play = m_UIDocument.rootVisualElement.Q<Button>("PlayBtn");
            m_Exit = m_UIDocument.rootVisualElement.Q<Button>("ExitBtn");
        }

        private void Handle_Play(ClickEvent clickEvent)
        {
            SceneManager.LoadScene("Development Scene");
        }

        private void Handle_Exit(ClickEvent clickEvent)
        {
            Application.Quit();
            Debug.Log("Exit Game Pressed!");
        }

        private void OnEnable()
        {
            m_Play.RegisterCallback<ClickEvent>(Handle_Play);
            m_Exit.RegisterCallback<ClickEvent>(Handle_Exit);
        }

        private void OnDisable()
        {
            m_Play.UnregisterCallback<ClickEvent>(Handle_Play);
            m_Exit.UnregisterCallback<ClickEvent>(Handle_Exit);
        }
    }
}