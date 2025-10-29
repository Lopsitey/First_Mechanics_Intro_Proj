#region

using Assessment_1_Scripts.Player;
using UnityEngine;

#endregion

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayerPrefab; //the player to be spawned

    private GameObject m_PlayerRef; //holds a reference to the player once spawned

    [SerializeField] private GameObject m_MainCamera; //the main camera for the every scene

    void Start()
    {
        m_PlayerRef = Instantiate(m_PlayerPrefab);
        m_PlayerRef.GetComponent<CharacterInitialisation>().init();

        m_MainCamera.GetComponent<CameraInitialisation>().init(m_PlayerRef);
    }
}