using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerGuideManager : MonoBehaviour {

    NewSoundManager soundManager;
    List<NewMusicClass> musicArr;
    List<string> strList;
    Dropdown dropDown;
    Dropdown levelDropdown;
    Dropdown chapterDropdown;
    public bool autoNote { get; set; }
    string level;
    int numberLevel;
    int chapter;
    public string numberInput { get; set; }
	// Use this for initialization
	void Start () {
        level = "Easy";
        soundManager = GameObject.Find("SoundManager").GetComponent<NewSoundManager>();
        musicArr = soundManager.AllMusics;
        dropDown = GameObject.Find("Dropdown").GetComponent<Dropdown>();
        levelDropdown = GameObject.Find("StringLevel").GetComponent<Dropdown>();
        chapterDropdown = GameObject.Find("Chapter").GetComponent<Dropdown>();
        dropDown.ClearOptions();
        strList = new List<string>();
        foreach(NewMusicClass music in musicArr)
        {
            strList.Add(music.name);
        }
        strList.Add("Tropical Love");
        dropDown.AddOptions(strList);
        dropDown.value = 0;
        levelDropdown.value = 0;
        autoNote = true;
        chapterDropdown.value = 0;
        chapter = 1;
        level = "Easy";


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
    }

    public void DropDownValueChange()
    {
        if (dropDown.value >= strList.Count)
        {
            dropDown.value = 0;
        }
        soundManager.MusicStart(soundManager.AllMusics[dropDown.value].name);
    }

    public void StartButton()
    {

        if (numberInput == null)
        {
            numberLevel = 1;
        }
        else
        {
            numberLevel = int.Parse(numberInput);
        }
        if(numberLevel < 0)
        {
            numberLevel = 0;
        }

        int index = levelDropdown.value;
        if (index == 0)
        {
            level = "Easy";
        }
        if (index == 1)
        {
            level = "Normal";
        }
        if (index >= 2)
        {
            level = "Hard";
        }

        if(chapterDropdown.value == 0)
        {
            chapter = 1;
        }
        else
        {
            chapter = 2;
        }

        LevelLoader.inst.Level = level;
        LevelLoader.inst.levelNum = numberLevel;
        LevelLoader.inst.autoNote = autoNote;
        LevelLoader.inst.Chapter = chapter;

        LevelLoader.inst.MusicName = strList[dropDown.value];
        LevelLoader.inst.LoadLevel("NoteEditor", LevelLoader.SceneType.NoteMaker);
    }
}
