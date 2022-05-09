using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChangeManager : MonoBehaviour {

    int NowChapter;
    public int SpriteIndex;
    int SpriteEnd;
    public Sprite[] SpriteArray;
    Image Canvas;
    bool FirstSprite = false;
    bool EndSprite = false;
    bool CorRunning = false;
    bool Loaded = false;
    LevelLoader levelLoader;
    AudioSource audio;
    Way nextWay;

    public enum Way
    {
        Gallery, MusicEnd, Start
    }

	// Use this for initialization
	void Start () {
        levelLoader = GameObject.Find("Levelloader").GetComponent<LevelLoader>();
        audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(Loaded)
                SpriteClick();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChange();
        }
	}

    public void Setup(int chapter,Way nextScene)
    {
        NowChapter = chapter;
        nextWay = nextScene;
        SpriteLoad();
        FirstSprite = true;
        EndSprite = false;
    }

    void SpriteLoad()
    {
        Canvas = GameObject.Find("ScreenFader").GetComponent<Image>();
        if(NowChapter == 0)
        {
            SpriteArray = Resources.LoadAll<Sprite>("Cutscene/Opening/");
        }
        else if (NowChapter == 1)
        {
            SpriteArray = Resources.LoadAll<Sprite>("Cutscene/Chapter1/");
        }
        else if (NowChapter == 2)
        {
            SpriteArray = Resources.LoadAll<Sprite>("Cutscene/Chapter2/");
        }
        else if (NowChapter == 3)
        {
            SpriteArray = Resources.LoadAll<Sprite>("Cutscene/Chapter3/");
        }
        SpriteEnd = SpriteArray.Length - 1;
        SpriteIndex = 0;
       // Canvas.sprite = SpriteArray[SpriteIndex++];
        levelLoader = GameObject.Find("Levelloader").GetComponent<LevelLoader>();
        Canvas.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeCoroutine());
        Loaded = true;
    }

    void SpriteClick()
    {
        if(!CorRunning)
            StartCoroutine(FadeCoroutine());
    }

    void SpriteChange()
    {
        if (SpriteIndex == SpriteEnd+1)
        {
            EndSprite = true;
            return;
        }
        Canvas.sprite = SpriteArray[SpriteIndex];
        SpriteIndex++;


    }

    void SceneChange()
    {
        string name;
        if (nextWay == Way.Gallery)
        {
            name = "Gallery";
            levelLoader.LoadLevel(name, LevelLoader.SceneType.Gallery);
        }
        else if(nextWay == Way.MusicEnd)
        {
            name = "";
            LevelLoader.inst.ClearSceneLoad();
        }
        else
        {
            name = "MusicPickScene";
            levelLoader.LoadLevel(name, LevelLoader.SceneType.MusicPick);
        }
       
    }

    IEnumerator FadeCoroutine()
    {
        //끄기->나타나기로 해야한다
        audio.Play();
        CorRunning = true;
        float t = 1;
        if (!FirstSprite)
        {
            while (t >0 )
            {
                //끄기단계
                Canvas.color = new Color(t, t, t, 1);
                t -= Time.deltaTime *2;
                yield return null;
            }
        }
        else
        {
            FirstSprite = false;
        }
        t = 0;
        SpriteChange();
        if (!EndSprite)
        {
            while (t < 1)
            {
                //켜기단계
                Canvas.color = new Color(t, t, t, 1);
                t += Time.deltaTime *2;
                yield return null;
            }
        }
        else
        {
            Destroy(Canvas.gameObject);
            SceneChange();
            EndSprite = false;
        }

        CorRunning = false;
      
    }
}
