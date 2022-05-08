using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour {
    public Dropdown resolutionDropdown;
    public Dropdown GraphicDropdown;
    public Slider vslider;
    public AudioSource audiosource;
    public Toggle fullscreen;

    void Start()
    { 
        Time.timeScale = 0f;

        if (!PlayerPrefs.HasKey("volume"))
            PlayerPrefs.SetFloat("volume", audiosource.volume);
        else
            vslider.value = PlayerPrefs.GetFloat("volume");         //volume에서 값 불러오기

        if (!PlayerPrefs.HasKey("graphic"))
            PlayerPrefs.SetInt("graphic", GraphicDropdown.value);
        else
            GraphicDropdown.value = PlayerPrefs.GetInt("graphic");

        if (!PlayerPrefs.HasKey("resolution"))
            PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        else
            resolutionDropdown.value = PlayerPrefs.GetInt("resolution");

        if (Screen.fullScreen == true)              // fullscreen toggle 동작
            fullscreen.isOn = true;
        else
            fullscreen.isOn = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    public void Back()
    {
        LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
        Time.timeScale = 1f;
    }

    public void saveButton()
    {
        audiosource.volume = vslider.value;
        PlayerPrefs.SetFloat("volume", audiosource.volume);             // Playerprefs 이용해서 음량 값 저장하기

        PlayerPrefs.SetInt("graphic", GraphicDropdown.value);
        QualitySettings.SetQualityLevel(GraphicDropdown.value);

        PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        if (resolutionDropdown.value == 1)
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        else
            Screen.SetResolution(1280, 720, Screen.fullScreen);

        if (fullscreen.isOn == true)                //전체화면 조건
            Screen.fullScreen = true;
        else
            Screen.fullScreen = false;

        PlayerPrefs.Save(); // 앞에서 정한 PlayerPrefs 값을 저장하기 위한 코드

        LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
        Time.timeScale = 1f;

    }

    public void CreditButton()
    {
        LevelLoader.inst.LoadLevel("Credit", LevelLoader.SceneType.Gallery);
    }
}
