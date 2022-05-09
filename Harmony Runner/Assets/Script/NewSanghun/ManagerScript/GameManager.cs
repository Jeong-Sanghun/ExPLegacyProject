using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public NewJsonManager jsonManager;
    public NewNoteMaker noteMaker;
    public NewSoundManager soundManager;
    public PassScript myPass;

    public static GameManager inst;
    public GameOverManager gameOverManager;
    public bool Read;
    public string MusicName;
    public string Level;
    public int NumLevel;
    public int Chapter;
    public bool AutoNoteMakerMode;

    public bool RandomMode;
    public bool RunnersMode;
    public bool DarkMode;
    public bool FastMode;
    public bool PerfectMode;
    public bool RHighMode;
    public float Velocity;

    public int TopScore;


    public void NoteInput()
    {
        soundManager.MusicStart(MusicName);
        jsonManager.IntLevel = NumLevel;
        jsonManager.Level = Level;
        jsonManager.Chapter = Chapter;
        if (AutoNoteMakerMode)
        {
            jsonManager.NoteAutoInputStart(MusicName);
        }
        else
        {
            jsonManager.NoteInputStart(MusicName);
        }

    }

    public void NoteInputEnd()
    {
        jsonManager.NoteInputEnd(MusicName, Level, NumLevel, Chapter);
    }

    public void SoundStart()
    {
        soundManager.MusicStart(MusicName);
    }

    public void NoteRead()
    {
        //level loader에서 전역변수로 넘겨준다.

        noteMaker.SetOptions(RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode, RHighMode, Velocity, Chapter,TopScore);
        noteMaker.GetComponent<Judgement>().ChangeScoreRate(RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode, RHighMode);
        noteMaker.MusicStart(MusicName, Level, Chapter);

        soundManager.MusicStart(MusicName);
        if (FastMode)
        {
            Time.timeScale = 1.5f;
            soundManager.FastMode();
        }
        myPass.GameStarted = true;
    }

    public void SetOptions(bool random, bool runners, bool dark, bool fast, bool perfect, bool rhigh, float velocity)
    {
        Velocity = 1;
        RandomMode = random;
        RunnersMode = runners;
        DarkMode = dark;
        FastMode = fast;
        PerfectMode = perfect;
        RHighMode = rhigh;
        Velocity = velocity;
    }

    /*
    public void LevelStart(string name,string level)
    {
        noteMaker.MusicStart(MusicName, level);
    }*/

    public void Gameover()
    {
        //judgement에서 불러옴.
        gameOverManager.Gameover();
    }

}
