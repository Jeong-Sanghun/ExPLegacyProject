using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScene : MonoBehaviour {

    public Image image;
    
    private void Start()
    {
        image = GameObject.Find("MainMenuCutScene").GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0);
        StartCoroutine(Fade());
    }
    public void StartGame()
    {
        LevelLoader.inst.CutSceneLoad(0,SpriteChangeManager.Way.Start);
    }

    public void NoteMaker()
    {
        LevelLoader.inst.LoadLevel("NoteMakerGuide", LevelLoader.SceneType.Gallery);
    }


    public void Option()
    {
        LevelLoader.inst.LoadLevel("Option", LevelLoader.SceneType.Gallery);
    }

    public void Gallery()
    {
        LevelLoader.inst.LoadLevel("Gallery", LevelLoader.SceneType.Gallery);
    }

    public void Quit()
    {
        Application.Quit(); 
    }

    float opacity;

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(1f);
        float opacity = 0;
        while (opacity <= 1)
        {
            opacity += Time.deltaTime;
            image.color = new Color(1, 1, 1, opacity);
            yield return null;
        }
        image.color = new Color(1, 1, 1, 1);
    }
}
