using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public PassScript passscript;
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public Text textcount;
    public GameObject tt;
    public bool onesc = false;      // esc 눌렀을때 아무키도 입력 못받게 하기 위한 변수
    public Slider vslider;
    public AudioSource audiosource;
    public MouseMath mousemath;
    float TimeScale;

    void Start()
    {
        mousemath = GameObject.Find("ActionManager").GetComponent<MouseMath>();


        if (!PlayerPrefs.HasKey("volume"))         // 볼륨값 저장해서 쓰기
            PlayerPrefs.SetFloat("volume", audiosource.volume);
        else
            vslider.value = PlayerPrefs.GetFloat("volume");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mousemath.RealMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);  // 일시정지 할 때의 마우스 위치를 받는다. 그래야 마우스 노트 반짝임 가능

            if (GameIsPaused)   // esc를 누르면 게임이 멈춤
            {
                if (!onesc)
                    Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    void Pause()
    {
        passscript.GameStarted = false;
        pauseMenuUI.SetActive(true);
        onesc = false;
        TimeScale = Time.timeScale;
        Time.timeScale = 0f;
        GameIsPaused = true;
        audiosource.Pause();
        mousemath.cal();
    }


    public void Resume()
    {
        StartCoroutine(coResume());
        //StopCoroutine(coResume());
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MainMenuButton()
    {
        GameIsPaused = false;
        passscript.GameStarted = true;
        Time.timeScale = 1f;
        onesc = true;
        SceneManager.LoadScene("StartScene");
    }

    IEnumerator coResume()
    {

        pauseMenuUI.SetActive(false);
        tt.SetActive(true);
        onesc = true;

        audiosource.volume = vslider.value;
        PlayerPrefs.SetFloat("volume", audiosource.volume);             // Playerprefs 이용해서 음량 값 저장하기

       mousemath.blinkstart();

        textcount.text = "3";
        yield return new WaitForSecondsRealtime(1.0f);
        textcount.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        textcount.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);          // 3초 세기
        Time.timeScale = TimeScale;
        tt.SetActive(false);
        GameIsPaused = false;
        passscript.GameStarted = true;
        audiosource.UnPause();
    }
    public void PauseButton()
    {
        if (!GameIsPaused)
            Pause();
    }

    
}