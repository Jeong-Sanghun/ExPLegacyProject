using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GalleryManager : MonoBehaviour {

    public GameObject[] buttons;
    CutSceneJson cutSceneJson;

    private void Start()
    {
        JsonLoad();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }


    void JsonLoad()
    {

        string jsonString;
        jsonString = File.ReadAllText(CutSceneJson.path);
        cutSceneJson = JsonUtility.FromJson<CutSceneJson>(jsonString);

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        if (PlayerPrefs.GetInt("Intro") == 1)
        {
            buttons[0].SetActive(true);
        }
        if (cutSceneJson.chapter1Seen == true)
        {
            buttons[1].SetActive(true);
        }
        if (cutSceneJson.chapter2Seen == true)
        {
            buttons[2].SetActive(true);
        }
        if (cutSceneJson.chapter3seen == true)
        {
            buttons[3].SetActive(true);
        }
    }

    public void Back()
    {
        LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
    }

    public void OnButton(int chapter)
    {
        LevelLoader.inst.CutSceneLoad(chapter, SpriteChangeManager.Way.Gallery);
    }
}
