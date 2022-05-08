using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Roller : MonoBehaviour {

    MusicsFormat wholeFormat;
    ScoreBoard[] ScoreBoardArr;
    List<GameObject> ObjectList;
    List<GameObject> ActiveObjectList;
    List<GameObject> TextList;
    List<GameObject> ActiveTextList;
    public GameObject ObjectPrefab;
    public GameObject RollCenter;
    public GameObject ModeUI;
    public GameObject TextParent;
    public GameObject TextPrefab;
    public GameObject HighscoreText;
    public Image HighRankImage;
    public Text HighScoreID;
    public int ObjectCount;
    public int MusicCount;
    float ScreenHeight;
    float UnitAngle;
    // Use this for initialization
    float[] ConstY = { 606.3281f, 453.4375f, 256.25f, 0, -256.25f, -453.4375f, -606.3281f };
    float[] ConstNameY = { 360,240,120,0,-173,-293,-413  };   //x는 300
    float[] ConstNameScale = { 0, 0.6f, 0.8f, 1.125f, 0.8f, 0.6f, 0 };
    string[] ModeInfoString =
    {
      "Runners Mode (×1.2)\nNo Notes", "Perfect Mode (×1.02)\nOnly Perfect is OK",
      "Speed Mode (×1.15)\nEverything ×1.5","Random Mode (×1.05)\nRandom Notes",  "Hidden Mode (×1.1)\nDark is around you...",
        "Runners High (×1.1)\nAcceleration"
    };

    //RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode, RHighMode
    float ConstNameX = 300f;
    public int Head;
    public int Tail;
    public int ObjectIndex;
    float ConstScale = 0.75f;
    float Constx = 915f;
    bool CorRunning = false;
    public int MusicIndex;
    float ScrollSpeed = 0.1f;
    bool Scrolling;
    Coroutine TimerCor;
    int TouchedIndex;
    int NowChapter;
    int lastMode = 0;

    public bool RandomMode { get; set; }
    public bool RunnersMode { get; set; }
    public bool DarkMode { get; set; }
    public bool FastMode { get; set; }
    public bool PerfectMode { get; set; }
    public bool RHighMode { get; set; }
    public float Velocity;
    public string MusicName { get; set; }
    public string MusicLevel;

    public List<string> MusicNames;
    public List<List<ScoreBoard>> HighScoreList;
    public List<int> NowLevelList;
    public List<int> ChapterList;

    public NewSoundManager soundManager;
    public LevelLoader levelLoader;
    public Text VelocityText;
    public Text ModeText;
    public Image DiffImage;
    public Image DiffBG;
    public Sprite[] DiffSprites;
    public Sprite[] DiffBGSprites;
    public Text DiffText;
    public GameObject[] ChapterBG;
    public Sprite[] RankSprites;

    Transform[] Tarr = new Transform[6];
    Vector2[] pos = new Vector2[6];
    Vector2 box = new Vector2(60, 53);
    Vector2 screen = new Vector2(Screen.width / 2, Screen.height / 2);


    //RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode, RHighMode

    bool modeCor = false;
    Coroutine mode;
    public void ModeTextChange(int i)
    {
        if (i == -1)
        {
            ModeText.gameObject.SetActive(false);
        }
        else
        {
            ModeText.gameObject.SetActive(true);
            ModeText.text = ModeInfoString[i];
            //mode = StartCoroutine(ModeTextFade(true));

        }
    }


    IEnumerator ModeTextFade(bool on)
    {
        modeCor = true;
        float t = 0;
        if (on)
        {
            t = 0;
            while (t <= 1)
            {
                ModeText.color = new Color(0, 0, 0, t);
                t += 0.2f;
                yield return new WaitForFixedUpdate();
            }
        }
        else
        {
            t = 1;
            while (t >= 0)
            {
                ModeText.color = new Color(0, 0, 0, t);
                t -= 0.2f;
                yield return new WaitForFixedUpdate();
            }
        }
        modeCor = false;

    }

    void Start()
    {
        //LoadMusic();
        Setup();
    }



    void Setup()
    {
        Velocity = 1.0f;
        TouchedIndex = -1;
        levelLoader = GameObject.Find("Levelloader").GetComponent<LevelLoader>();
        if(DiffImage == null)
            DiffImage = GameObject.Find("DifficultyButton").GetComponentInChildren<Image>();
        if (DiffText == null)
            DiffText = GameObject.Find("DifficultyButton").GetComponentInChildren<Text>();
        if (levelLoader == null)
            levelLoader = LevelLoader.inst;
        MusicNames = new List<string>();
        NowLevelList = new List<int>();
        ChapterList = new List<int>();
        HighScoreList = new List<List<ScoreBoard>>();
        string NoteString;
        /*
        NoteList = Resources.Load("ChartResources/Format/MusicFormat") as TextAsset;
        NoteString = NoteList.text;*/


        NoteString = File.ReadAllText(Application.streamingAssetsPath + "/ChartResources/Format/MusicFormat.txt");

        /*
#if UNITY_STANDALONE_WIN
        NoteString = File.ReadAllText(Application.streamingAssetsPath + "/ChartResources/Format/MusicFormat.txt");
        Debug.Log(NoteString);
#endif*/
        wholeFormat = JsonUtility.FromJson<MusicsFormat>(NoteString);

        MusicCount = wholeFormat.MusicList.Count;

        ScreenHeight = Screen.height;
        ObjectList = new List<GameObject>();
        ActiveObjectList = new List<GameObject>();
        TextList = new List<GameObject>();
        ActiveTextList = new List<GameObject>();
        //배열인덱스니까 -1이다.
        if (MusicCount < 7)
        {
            int times = 7 / MusicCount;
            ObjectCount = MusicCount *(times + 1);

        }
        else
        {
            ObjectCount = MusicCount;
        }
        Head = 0;
        Tail = 6;
        for(int i = 0; i < MusicCount; i++)
        {
            string name = wholeFormat.MusicList[i].MusicName;
            ChapterList.Add(wholeFormat.MusicList[i].ChapterNum);
            MusicNames.Add(name);
            NowLevelList.Add(0);
            List<ScoreBoard> dumy;
            List<ScoreBoard> adder = new List<ScoreBoard>();
            dumy = null;
            for (int j = 0; j < wholeFormat.MusicList[i].LevelStringList.Count; j++)
            {
                dumy = LoadScore(name, wholeFormat.MusicList[i].LevelStringList[j]);
                int scoreindex = 0;
                if (dumy != null)
                {
                    int high = dumy[0].TotalScore;
                    for (int t = 1; t < dumy.Count; t++)
                    {
                        if (high < dumy[t].TotalScore)
                        {
                            high = dumy[t].TotalScore;
                            scoreindex = t;
                        }

                    }
                    adder.Add(dumy[scoreindex]);
                }
                else
                {
                    adder.Add(null);
                }
            }
            HighScoreList.Add(adder);

            //기본 레벨은 0이다.
        }
        NowChapter = ChapterList[0];
        for (int i = 0; i < ObjectCount; i++)
        {
            int j = 0;
            j = i % MusicCount;
            GameObject nowObj = Instantiate(ObjectPrefab, RollCenter.transform, true);
            Image renderer = nowObj.GetComponentInChildren<Image>();
            Sprite spr;
            Texture2D texture;
            byte[] bytes;

            //spr = Resources.Load<Sprite>("ChartResources/NotesJson/" + MusicNames[j] + "/" + MusicNames[j]);
            if(File.Exists(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicNames[j] + "/" + MusicNames[j] + ".jpg")){
                bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicNames[j] + "/" + MusicNames[j] + ".jpg");
                if (bytes != null)
                {
                    texture = new Texture2D(1920,1080, TextureFormat.ARGB32, false);
                    texture.LoadImage(bytes);
                    spr = Sprite.Create(texture, new Rect(0.0f, 0.0f,texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
                }
                else
                {
                    spr = null;
                }
            }
            else
            {
                spr = null;
            }

            /*
#if UNITY_STANDALONE_WIN
            bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicNames[j] + "/" + MusicNames[j] + ".png");
            if (bytes != null)
            {
                texture = new Texture2D(480, 270, TextureFormat.ARGB32, false);
                texture.LoadImage(bytes);
                spr = Sprite.Create(texture, new Rect(0.0f, 0.0f, 480.0f, 270.0f), new Vector2(0.5f, 0.5f));

            }
            else
            {
                spr = null;
            }
#endif*/

            if (spr != null)
            {
                renderer.sprite = spr;
            }
            Button button = nowObj.GetComponentInChildren<Button>();
            nowObj.SetActive(false);
            ObjectList.Add(nowObj);
            //RectTransform rectT = nowObj.GetComponent<RectTransform>();
            //Text text = nowObj.transform.GetChild(0).GetComponent<Text>();


            int a = i;
            button.onClick.AddListener(delegate { OnButton(j,a); });
            //text.text = MusicNames[j].ToString();

            GameObject nowTxt = Instantiate(TextPrefab, TextParent.transform, true);
            TextList.Add(nowTxt);
            nowTxt.GetComponent<Text>().text = MusicNames[j];



            // rectT.sizeDelta = new Vector2(rectT.sizeDelta.x * ScreenHeight / 1080f, 
            //   rectT.sizeDelta.y * ScreenHeight / 1080f);
        }
        for(int i = 0; i < 7; i++)
        {
            ActiveObjectList.Add(ObjectList[i]);
            ObjectList[i].SetActive(true);
            ObjectList[i].transform.localPosition = new Vector2(915, ConstY[i]);
            float scale = Mathf.Pow(ConstScale, Mathf.Abs(i - 3));
            ObjectList[i].transform.localScale = new Vector2(scale, scale);

            ActiveTextList.Add(TextList[i]);
            TextList[i].SetActive(true);
            TextList[i].transform.localPosition = new Vector2(ConstNameX, ConstNameY[i]);
            TextList[i].transform.localScale = new Vector2(ConstNameScale[i], ConstNameScale[i]);
        }
        MusicIndex = 3% MusicCount ;
        ObjectIndex = 3;
        UIactive(false);


        //러너스 퍼펙트 패스트 랜덤 다크 러너스하이
        for (int i = 0; i < 6; i++)
        {
            Tarr[i] = ModeUI.transform.GetChild(i);

            pos[i] = Tarr[i].localPosition;

        }

    }

    public void Back()
    {
        LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            Scrolling = true;
            TimerCor = StartCoroutine(Timer());
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            Scrolling = false;
            ScrollSpeed = 0.1f;
            StopCoroutine(TimerCor);
        }


        if (Input.GetKey(KeyCode.W))
        {
            Scroll(false);
            TouchedIndex = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Scroll(true);
            TouchedIndex = -1;
        }
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel < 0)
        {
            Scroll(false);
            TouchedIndex = -1;
        }
        if (wheel > 0)
        {
            Scroll(true);
            TouchedIndex = -1;
        }

        
        if (Input.GetKeyUp(KeyCode.Space))
        {

            OnButton(MusicIndex,ObjectIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIactive(false);
            TouchedIndex = -1;
        }

        if (modeUI)
        {
            //  Vector2 point;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(ModeUI.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out point);
            Vector2 mouse = Input.mousePosition;
            mouse = mouse - screen;
            bool nowhere = true;
            //Debug.Log(mouse);
            for (int i = 0; i < 6; i++)
            {
                Vector2 dist1 = mouse - pos[i];
                Vector2 dist2 = dist1 - box;

                if (dist1.x > 0 && dist1.y > 0 && dist2.x < 0 && dist2.y < 0)
                {

                    ModeTextChange(i);
                    //lastMode = i;
                    nowhere = false;
                    break;

                }

            }
            if (nowhere)
                ModeTextChange(-1);
        }

    }

    void Scroll(bool down)
    {
        //중앙의 축(패런츠)만 돌려준다. 오브젝트 개수에 따라 돌리는 양도 달라져야 한다. 오브젝트 개수가 많으면 많을수록, 반지름이 늘어난다. 
        //크기 변환 식은 y= -0.5x + 1 x에들어가는 값은, abs(pos.y)/540
        if(CorRunning == false)
        {
            if (ModeUI.activeSelf)
                UIactive(false);
            StartCoroutine(ScrollCor(down));
        }


    }

    //y좌표 5개는 고정 하드코딩이다.
    //-606.3281, -453.4375, -256.25, 0 , 256.25, 453.4375, 606.3281
    
    IEnumerator Timer()
    {
        float time = 0;
        while (Scrolling)
        {
            time += Time.deltaTime;
            if (time > 1f)
            {
                ScrollSpeed += 0.05f;
                time = 0;
            }
            if(ScrollSpeed > 0.5f)
            {
                ScrollSpeed = 0.5f;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ScrollCor(bool indexDown)
    {
        //여기서 헤드는 안보이는 맨 윗부분 얘기하는거다
        //여기서 테일은 안보이는 맨 아랫부분 얘기하는거다.
        CorRunning = true;
        HighscoreText.SetActive(false);
        ActiveTextList[3].GetComponent<Text>().fontStyle = FontStyle.Normal;
        if (indexDown)
        {
            Head--;
            ObjectIndex--;
            Tail--;
            MusicIndex--;
            if (Head ==-1)
            {
                Head = ObjectList.Count-1;
            }
            if(Tail == -1)
            {
                Tail = ObjectList.Count-1;
            }
            if(MusicIndex == -1)
            {
                MusicIndex = MusicCount - 1;
            }
            if(ObjectIndex == -1)
            {
                ObjectIndex = ObjectList.Count - 1;
            }

        }
        else
        {
            Head++;
            Tail++;
            ObjectIndex++;
            MusicIndex++;
            if (Head == ObjectList.Count)
            {
                Head = 0;
            }
            if (Tail == ObjectList.Count)
            {
                Tail = 0;
            }
            if (MusicIndex == MusicCount)
            {
                MusicIndex = 0;
            }
            if (ObjectIndex == ObjectList.Count)
            {
                ObjectIndex =0;
            }
        }

        //상수는 0~6까지 총 7개 있다. 얘네를 그 다음거로 lerp하는게 문제다.
        float T = 0;
        int count = ActiveObjectList.Count;
        Vector2[] array = new Vector2[count];
        Vector2[] scales = new Vector2[count];
        Vector2[] Tarray = new Vector2[count];
        Vector2[] Tscales = new Vector2[count];
        for(int i = 0; i < count; i++)
        {
            float scale = Mathf.Pow(ConstScale, Mathf.Abs(i-3));
            array[i] = new Vector2(Constx, ConstY[i]);
            scales[i] = new Vector2(scale, scale);
            Tarray[i] = new Vector2(ConstNameX, ConstNameY[i]);
            Tscales[i] = new Vector2(ConstNameScale[i], ConstNameScale[i]);
        }

        while (T <= 1)
        {
            if (T + ScrollSpeed > 1)
                T = 1;
            for (int i = 0; i < count-1; i++)
            {
                if (indexDown)
                {
                    ActiveObjectList[i].transform.localPosition = Vector2.Lerp(array[i], array[i + 1], T);
                    ActiveObjectList[i].transform.localScale = Vector2.Lerp(scales[i], scales[i + 1], T);

                    ActiveTextList[i].transform.localPosition = Vector2.Lerp(Tarray[i], Tarray[i + 1], T);
                    ActiveTextList[i].transform.localScale = Vector2.Lerp(Tscales[i], Tscales[i + 1], T);
                }
                else
                {
                    int process = i + 1;
                    ActiveObjectList[process].transform.localPosition = Vector2.Lerp(array[process], array[process - 1], T);
                    ActiveObjectList[process].transform.localScale = Vector2.Lerp(scales[process], scales[process - 1], T);

                    ActiveTextList[process].transform.localPosition = Vector2.Lerp(Tarray[process], Tarray[process - 1], T);
                    ActiveTextList[process].transform.localScale = Vector2.Lerp(Tscales[process], Tscales[process - 1], T);
                }
               
            }
            T += ScrollSpeed;
            yield return new WaitForFixedUpdate();
        }
        
       
        if (indexDown)
        {
            GameObject headObj = ObjectList[Head];
            GameObject tailObj = ActiveObjectList[count-1];
            tailObj.SetActive(false);
            headObj.SetActive(true);
            ActiveObjectList.Remove(tailObj);
            ActiveObjectList.Insert(0, headObj);
            headObj.transform.localPosition = array[0];
            headObj.transform.localScale = scales[0];

            GameObject headTxt = TextList[Head];
            GameObject tailTxt = ActiveTextList[count - 1];
            tailTxt.SetActive(false);
            headTxt.SetActive(true);
            ActiveTextList.Remove(tailTxt);
            ActiveTextList.Insert(0, headTxt);
            headTxt.transform.localPosition = Tarray[0];
            headTxt.transform.localScale = Tscales[0];


        }
        else
        {
            GameObject headObj = ActiveObjectList[0];
            GameObject tailObj = ObjectList[Tail];
            tailObj.SetActive(true);
            headObj.SetActive(false);
            ActiveObjectList.Remove(headObj);
            ActiveObjectList.Add(tailObj);
            tailObj.transform.localPosition = array[count-1];
            tailObj.transform.localScale = scales[count-1];

            GameObject headTxt = ActiveTextList[0];
            GameObject tailTxt = TextList[Tail];
            tailTxt.SetActive(true);
            headTxt.SetActive(false);
            ActiveTextList.Remove(headTxt);
            ActiveTextList.Add(tailTxt);
            tailTxt.transform.localPosition = Tarray[count - 1];
            tailTxt.transform.localScale = Tscales[count - 1];
        }
        ActiveTextList[3].GetComponent<Text>().fontStyle = FontStyle.Bold;
        HighScoreChange();
       
        HighscoreText.SetActive(true);
        CorRunning = false;
    }

    void HighScoreChange()
    {
        if (HighScoreList[MusicIndex][NowLevelList[MusicIndex]] != null)
        {
            ScoreBoard score = HighScoreList[MusicIndex][NowLevelList[MusicIndex]];
            HighscoreText.GetComponent<Text>().text = score.TotalScore.ToString() + " / " +
            score.Accuracy.ToString("N1");
            HighRankImage.sprite = RankSprites[(int)score.ScoreRank];
            HighRankImage.color = new Color(1, 1, 1, 1);
            HighScoreID.text = score.ID;
        }
        else
        {
            HighscoreText.GetComponent<Text>().text = 0 + "/" + 0;
            HighRankImage.sprite = null;
            HighRankImage.color = new Color(1, 1, 1, 0);
            HighScoreID.text = "NoOne";
        }
    }


    public void OnButton(int musicIndex,int objIndex)
    {

        if (CorRunning)
            return;
        if (TouchedIndex != objIndex)
        {
            soundManager.MusicStart(MusicNames[musicIndex]);
            if(ChapterList[musicIndex] != NowChapter)
            {
                NowChapter = ChapterList[musicIndex];
                BGChange(NowChapter);
            }
            TouchedIndex = objIndex;
            StartCoroutine(OnButtonCor(ObjectIndex, objIndex));
            //여기서 ObjectIndex는 내가 방금까지 보고있던 거.
        }
        else
        {
            soundManager.MusicStop();
            MusicLevel = wholeFormat.MusicList[MusicIndex].LevelStringList[NowLevelList[MusicIndex]];
            levelLoader.SetOptions(RandomMode, RunnersMode, DarkMode, FastMode, PerfectMode,RHighMode, Velocity);
            int topScore =0;
            if(HighScoreList[MusicIndex][NowLevelList[MusicIndex]] != null)
            {
                topScore = HighScoreList[MusicIndex][NowLevelList[MusicIndex]].TotalScore;
            }
            levelLoader.PlaySceneLoad(MusicNames[musicIndex], MusicLevel, NowChapter,topScore);
        }
    }

    IEnumerator OnButtonCor(int obj,int index)
    {
        //music이 내가 지금 보고있는거, index가 내가 누른거.
        //Debug.Log("보고있는게" + obj + " 누른게 "+ index);
        int dist = obj - index;

        if (Mathf.Abs(dist) > ObjectCount - 3)
        {
            if (dist < 0)
            {
                dist = 1 * (dist + ObjectCount);
            }
            else
            {
                dist = 1 * (dist - ObjectCount);
            }
        }
        if (dist > 0)
        {
            ScrollSpeed = 0.1f;
            //인덱스가 내려가는 쪽이 true다.
            for (int i = 0; i < dist; i++)
            {
                Scroll(true);
                yield return new WaitForSeconds(0.4f);
            }
        }
        else if (dist < 0)
        {
            ScrollSpeed = 0.1f;
            for (int i = 0; i < -1*dist; i++)
            {
                Scroll(false);
                yield return new WaitForSeconds(0.4f);
            }
        }
        ScrollSpeed = 0.1f;
        UIactive(true);
    }

    public void SpeedUp(bool up)
    {
        if (up)
            Velocity += 0.5f;
        else
            Velocity -= 0.5f;
        if (Velocity < 0.5f)
            Velocity = 0.5f;
        else if (Velocity > 5)
            Velocity = 5f;
        VelocityText.text = Velocity.ToString("N1");
    }

    bool modeUI = false;
    void UIactive(bool toggle)
    {
        if (toggle)
            VelocityText.text = Velocity.ToString("N1");
        ModeUI.SetActive(toggle);
        modeUI = toggle;
        TextParent.SetActive(!toggle);

        OnDifficultyButton(false);
    }

    public void OnDifficultyButton(bool changing)
    {
        //현재 인덱스는 MusicIndex
        //현재 포맷은 WholeFormat
        //1.지금 내 인덱스에서 선택된 레벨이 무엇인지, -> 스트링 리스트를 만들어서, 현재 인덱스를 표시해준다.
        //2.다음게 있는지. -> wholeFormat에서 확인한다.
        //3.다음게 있으면 순환.
        int nowLevelIndex;
        string strLevel;
        if (changing)
        {
            NowLevelList[MusicIndex]++;
            if (NowLevelList[MusicIndex] == wholeFormat.MusicList[MusicIndex].LevelStringList.Count)
                NowLevelList[MusicIndex] = 0;
        }

        nowLevelIndex= NowLevelList[MusicIndex];
        strLevel = wholeFormat.MusicList[MusicIndex].LevelStringList[nowLevelIndex];
        HighScoreChange();

        if (strLevel.Contains("Easy"))
        {
            DiffButtonColor(0);
        }
        else if (strLevel.Contains("Hard"))
        {
            DiffButtonColor(2);
        }
        else
        {
            DiffButtonColor(1);
            //normal이 디폴트다.
        }
        DiffText.text = wholeFormat.MusicList[MusicIndex].LevelIntList[nowLevelIndex].ToString();
    }

    void DiffButtonColor(int lvl)
    {
        //lvl 0     1     2
        //  easy, normal hard
        if(lvl>=0 && lvl <= 2)
        {
            DiffBG.sprite = DiffBGSprites[lvl];
            DiffImage.sprite = DiffSprites[lvl];
        }
        else
        {
            DiffBG.sprite = DiffBGSprites[1];
            DiffImage.sprite = DiffSprites[1];
        }
    }

    void BGChange(int chp)
    {
        ChapterBG[0].SetActive(false);
        ChapterBG[1].SetActive(false);
        ChapterBG[2].SetActive(false);
        switch (chp)
        {
            case 2:
                ChapterBG[1].SetActive(true);
                break;
            case 3:
                ChapterBG[2].SetActive(true);
                break;
            default:
                ChapterBG[0].SetActive(true);
                break;
        }
    }





    TextAsset[] JsonTextAssets;
    string[] JsonRawString;
    NewNoteJson[] JsonNote;

     //포맷파일 만드려고 만들어놓음. jsonmanager에서 이용중. 만약에 포맷이 맞지않다면 이걸로 다시 맞춰야한다.
    public void LoadMusic()
    {
        int musicCount =0;
        MusicNames = new List<string>();
        JsonTextAssets = Resources.LoadAll<TextAsset>("ChartResources/NotesJson/");
        JsonRawString = new string[JsonTextAssets.Length];
        JsonNote = new NewNoteJson[JsonTextAssets.Length];
        

        MusicsFormat musics = new MusicsFormat();

        List<JustFormat> Formats = new List<JustFormat>();

        for (int i = 0; i < JsonTextAssets.Length; i++)
        {
            string name;
            JsonRawString[i] = JsonTextAssets[i].text;
            JsonNote[i] = JsonUtility.FromJson<NewNoteJson>(JsonRawString[i]);
            name = JsonNote[i].MusicName;
            if (!MusicNames.Contains(name))
            {
                JustFormat just = new JustFormat(name);
                just.LevelStringList.Add(JsonNote[i].Level);
                just.LevelIntList.Add(JsonNote[i].IntLevel);
                just.ChapterNum = JsonNote[i].Chapter;
                Formats.Add(just);
                
                MusicNames.Add(name);
                musicCount++;
            }
            else
            {
              
                int j = Formats.Count;
                for(int k = 0; k < j; k++)
                {
                    if (Formats[k].MusicName.Contains(JsonNote[i].MusicName))
                    {
                        Formats[k].ChapterNum = JsonNote[i].Chapter;
                        if (Formats[k].LevelStringList.Contains(JsonNote[i].Level))
                        {
                            break;
                        }
                        else
                        {
                            Formats[k].LevelStringList.Add(JsonNote[i].Level);
                            Formats[k].LevelIntList.Add(JsonNote[i].IntLevel);
                        }
                        
                    }


                }

                
                /*
                For.LevelStringList.Add(JsonNote[i].Level);
                just.LevelIntList.Add(JsonNote[i].IntLevel);
                */
            }
        }
        musics.MusicList = Formats;



        string JsonText = JsonUtility.ToJson(musics, true);
        StreamWriter myFile = new StreamWriter("Assets/resources/ChartResources/Format/MusicFormat.txt");
        myFile.Write(JsonText);
        myFile.Close();
    }

    public List<ScoreBoard> LoadScore(string name,string level)
    {
        List<ScoreBoard> arr;
        string[] files;
        int num = 0;

        if (!Directory.Exists(Application.streamingAssetsPath + "/ChartResources/ScoreBoard/" + name + "/"+ level+"/"))
        {
            return null;
        }
        files = Directory.GetFiles(Application.streamingAssetsPath + "/ChartResources/ScoreBoard/" + name +"/" + level);
        if (files == null)
        {
            return null;
        }
        JsonRawString = new string[files.Length];
        arr = new List<ScoreBoard>();
        num = 0;

        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".meta"))
            {
                JsonRawString[num] = File.ReadAllText(files[i]);
                arr.Add(JsonUtility.FromJson<ScoreBoard>(JsonRawString[num]));
                num++;
            }
           
        }

        
        return arr;
    }

}
