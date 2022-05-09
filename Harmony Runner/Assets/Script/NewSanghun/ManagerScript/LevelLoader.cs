using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LevelLoader : MonoBehaviour
{

    public GameObject loadingScreen;    //prefab
    Image LoadingImage;
    GameObject RealScreen;
    public Slider slider;
    public Text progresstext;
    public static LevelLoader inst;
    GameManager gameManager;
    SpriteChangeManager spriteChanger;
    public string MusicName;
    public string Level;
    public int levelNum;
    public int Chapter;
    bool IntroSeen;

    public GameClearManager gameClear;
    public ScoreBoard scoreBoard;
    public SpriteChangeManager.Way cutSceneWay;
    public int nowChapter;
    public CutSceneJson cutSceneJson;


    public bool RandomMode;
    public bool RunnersMode;
    public bool DarkMode;
    public bool FastMode;
    public bool PerfectMode;
    public bool RHighMode;
    float Velocity = 1;

    int TopScore = 0;

    public bool autoNote;

    public enum SceneType
    {
        MusicPick,CutScene,GameClear,GamePlay,Menu,Gallery,NoteMaker
    }

    void Start()
    {
        if (inst == null)
        {
            inst = this;
            
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
        if (PlayerPrefs.HasKey("Intro"))
        {
            int key = PlayerPrefs.GetInt("Intro");
            if (key == 0)
            {
                IntroSeen = false;
            }
            else
            {
                IntroSeen = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("Intro", 0);
            IntroSeen = false;
        }
        CutSceneJson.path = Application.streamingAssetsPath + "/Save/CutSceneSave.txt";

    }
    //0 컷씬, 1 플레이씬
    public void LoadLevel(string scene, SceneType sceneType)
    {
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        StartCoroutine(LoadAsynchronously(scene, sceneType));
    }

    public void CutSceneLoad(int chapter, SpriteChangeManager.Way way)
    {
        cutSceneWay = way;
        nowChapter = chapter;
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        if(way == SpriteChangeManager.Way.Gallery)
        {
            StartCoroutine(LoadAsynchronously("CutScene", SceneType.CutScene));
        }
        else
        {
            if (chapter == 0)
            {
                if (PlayerPrefs.GetInt("Intro") == 1)
                {
                    StartCoroutine(LoadAsynchronously("MusicPickScene", SceneType.MusicPick));
                }
                else
                {
                    PlayerPrefs.SetInt("Intro", 1);
                    StartCoroutine(LoadAsynchronously("CutScene", SceneType.CutScene));
                }
            }
            else
            {
                StartCoroutine(LoadAsynchronously("CutScene", SceneType.CutScene));
            }
        }
   




    }

    IEnumerator LoadAsynchronously(string scene, SceneType sceneType)
    {

        //scenetype
        /*0 : 컷씬
         *1 : 플레이씬
         *2 : 뮤직픽씬
         */
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        RealScreen.SetActive(true);
        LoadingImage = RealScreen.transform.GetChild(0).GetComponent<Image>();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            if(LoadingImage != null)
                LoadingImage.color = new Color(1, 1, 1, progress);

            slider.value = progress;
            if (progresstext != null)
                progresstext.text = progress * 100f + "%";
            yield return null;
        }

        if (sceneType == SceneType.CutScene)
        {
            spriteChanger = GameObject.Find("SpriteChangeManager").GetComponent<SpriteChangeManager>();
            spriteChanger.Setup(nowChapter,cutSceneWay);
        }
        else if (sceneType == SceneType.GamePlay)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.MusicName = MusicName;
            gameManager.Level = Level;

            gameManager.SetOptions(RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode, RHighMode, Velocity);
            gameManager.Chapter = Chapter;
            gameManager.TopScore = TopScore;
            gameManager.NoteRead();
        }
        else if(sceneType == SceneType.GameClear)
        {
            gameClear = GameObject.Find("GameClearManager").GetComponent<GameClearManager>();
            gameClear.scoreBoard = scoreBoard;
            gameClear.ClearStart();
        }
        else if(sceneType == SceneType.Gallery)
        {

        }
        else if(sceneType == SceneType.NoteMaker)
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.Level = Level;
            gameManager.NumLevel = levelNum;
            gameManager.Chapter = Chapter;
            gameManager.MusicName = MusicName;
            gameManager.AutoNoteMakerMode = autoNote;
            gameManager.NoteInput();
        }

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

    public void PlaySceneLoad(string name, string level, int chapter, int topScore)
    {
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        string sceneName = "Chapter1Scene";
        MusicName = name;
        Level = level;
        Chapter = chapter;
        TopScore = topScore;
        StartCoroutine(LoadAsynchronously(sceneName, SceneType.GamePlay));
    }

    public void Replay()
    {
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        string sceneName = "Chapter1Scene";
        StartCoroutine(LoadAsynchronously(sceneName, SceneType.GamePlay));
    }

    public void ClearSceneLoad(ScoreBoard score)
    {
        scoreBoard = score;
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        string sceneName = "GameClearScene";
        StartCoroutine(LoadAsynchronously(sceneName, SceneType.GameClear));
    }

    public void ClearSceneLoad()
    {
        RealScreen = Instantiate(loadingScreen, GameObject.Find("Canvas").transform);
        slider = RealScreen.GetComponentInChildren<Slider>();
        progresstext = RealScreen.GetComponentInChildren<Text>();
        string sceneName = "GameClearScene";
        StartCoroutine(LoadAsynchronously(sceneName, SceneType.GameClear));
    }
}
