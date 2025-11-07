using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefsExample : MonoBehaviour
{
    [SerializeField] private Slider m_MasterVolSlider;//linked to the master volume slider in the UI - used to set and get the volume level save data
    [SerializeField] private Slider m_SFXVolSlider;
    [SerializeField] private TMP_Dropdown m_DifficultySetting;
    [SerializeField] private Toggle m_ForceFullScreen;

    private void Start()
    {
        LoadData();
    }

    public void SaveData()//bound through a GUI on click event on the canvas button
    {
        PlayerPrefs.SetFloat("MasterVolumeSlider", m_MasterVolSlider.value);
        PlayerPrefs.SetFloat("SFXVolumeSlider", m_SFXVolSlider.value);
        PlayerPrefs.SetInt("DifficultySetting", m_DifficultySetting.value);
        PlayerPrefs.SetInt("ToggleFullScreen", m_ForceFullScreen ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void DeleteKey()
    {
        PlayerPrefs.DeleteKey("MasterVolumeSlider");
        PlayerPrefs.DeleteKey("SFXVolumeSlider");
        PlayerPrefs.DeleteKey("DifficultySetting");
        PlayerPrefs.DeleteKey("ToggleFullScreen");

        // or PlayerPrefs.DeleteAll();
    }

    public void LoadData()
    {
        m_MasterVolSlider.value = PlayerPrefs.GetFloat("MasterVolumeSlider");
        m_SFXVolSlider.value = PlayerPrefs.GetFloat("SFXVolumeSlider");
        m_DifficultySetting.value = PlayerPrefs.GetInt("DifficultySetting");
        m_ForceFullScreen.isOn = PlayerPrefs.GetInt("ToggleFullScreen") == 1 ? true : false;
    }

    public void ResetSettings()
    {
        m_MasterVolSlider.value = 0;
        m_SFXVolSlider.value = 0;
        m_DifficultySetting.value = 0;
        m_ForceFullScreen.isOn = false;
    }
}
