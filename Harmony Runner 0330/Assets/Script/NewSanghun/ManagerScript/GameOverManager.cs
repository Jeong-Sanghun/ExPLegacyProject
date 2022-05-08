using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    Judgement judgement;
    //public PlayerHealth playerHealth;       // Reference to the player's health.
    public float restartDelay = 5f;         // Time to wait before restarting the level


    Animator anim;                          // Reference to the animator component.
    float restartTimer;                     // Timer to count up to restarting the level

    public GameObject Canvas;
    public GameObject ShadowObj;
    public float speed; // 현재 스피드에서 몇배할지
    public NewSoundManager soundManager;
    public LevelLoader levelLoader;
    // Use this for initialization

    //게임오버 전에 그림자 따라오게


    void Awake()
    {
        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>();
        soundManager = GameObject.Find("SoundManager").GetComponent<NewSoundManager>();
        Canvas = GameObject.Find("Canvas");
       // Set up the reference.
         anim = Canvas.GetComponent<Animator>();

    }

    public void Gameover()
    {


        StartCoroutine(ShadowMove());
        soundManager.rhythmTool.Stop();
    }

    IEnumerator ShadowMove()
    {
        while(ShadowObj.transform.position.x <= 0f)
        {
            //게임오버매니저 스크립트에
            // 게임오버 창
            //Debug.Log("게임오버");
            ShadowObj.transform.Translate(0.03f * speed, 0, 0); // 그림자 따라옴 속도조절
            yield return null;
        }
        AnimationTrigger();
        levelLoader = GameObject.Find("Levelloader").GetComponent<LevelLoader>();
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;
        levelLoader.LoadLevel("MusicPickScene", LevelLoader.SceneType.MusicPick);
    }

    void AnimationTrigger()
    {
        anim.SetTrigger("GameOver");
    }
}
