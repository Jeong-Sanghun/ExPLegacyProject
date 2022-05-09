using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NewNoteMaker : NoteClass {

    //NoteClass를 상속해줘서 변수 더럽게 나오는거 재낀다.
    TextAsset NoteList; //기획자님이 짜줄 노트 txt파일
    public RhythmTool rhythmTool;
    int[] NoteIndex = new int[3]; //txt파일에서 몇번째 노트가 흘러가고 있는지 알아내는 방법.
    int ActionIndex = 0;
    Coroutine Stopper;

    //여기서 액션노트 나올 때 코딩해야함

    public NewNoteJson NoteBook = new NewNoteJson();
    bool Gamestart = false;
    public GameObject RunnersMask;
    public GameObject[] ChapterBG;
    public Judgement judgement; //스코어보드에 이름넣으려고



    int[] Index = new int[3];

    public int Offset;
    //velocity는 noteclass소속이다.
    public bool RandomMode { get; set; }
    public bool RunnersMode;
    public bool DarkMode;
    public bool FastMode { get; set; }
    public bool PerfectMode;
    public bool RHighMode;

    public static int NowRoute;
    public float VelocityMemo;

    int Chapter = 1;

    public void Queueing()
    {
        NowRoute = 0;
        while (NowRoute < 3)
        {
            if (RandomMode)
            {
                Random.InitState((int)System.DateTime.Now.Ticks);
                while (Index[NowRoute] > NoteIndex[NowRoute])
                {
                    NewNote dumy = NoteBook.NoteList[NowRoute].NewNoteList[NoteIndex[NowRoute]];
                    char type = dumy.NoteType;
                    if(type== '4' || type == '5' || type == '6' || type == '8')
                    {
                        int rand = Random.Range(1, 5);

                        switch (rand)
                        {
                            case 1:
                                type = '4';
                                break;
                            case 2:
                                type = '5';
                                break;
                            case 3:
                                type = '6';
                                break;
                            case 4:
                                type = '8';
                                break;
                            default:
                                type = '4';
                                break;

                        }

                    }
                    dumy.NoteType = type;
                    NoteGenerator(type, dumy.LongNoteLength, dumy.NoteTime);
                    passScript.Enqueue(dumy);
                    NoteIndex[NowRoute]++;
                }
            }
            else
            {
                while (Index[NowRoute] > NoteIndex[NowRoute])
                {
                    NewNote dumy = NoteBook.NoteList[NowRoute].NewNoteList[NoteIndex[NowRoute]];
                    NoteGenerator(dumy.NoteType, dumy.LongNoteLength, dumy.NoteTime);
                    passScript.Enqueue(dumy);
                    NoteIndex[NowRoute]++;
                }
            }
            NowRoute++;
        }
        NowRoute = 0;
        passScript.QueueingEnd(false);
        passScript.IndexList(NoteBook.RoutingIndexFirst, NoteBook.RoutingIndexSecond, NoteBook.ActionIndexFirst, NoteBook.ActionIndexSecond);

    }

    //스태틱함수로 패스스크립트에서 실행한다. 단순 변수만 바꾸기.
    public static void RouteChange(bool routing)
    {
        if(NowRoute == 0)
        {
            if(routing)
            {
                NowRoute = 1;
            }
            else
            {
                NowRoute = 2;
            }
        }
        else
        {
            NowRoute = 0;
        }
    }

    public void DeleteNoteinList(Transform note)
    {
        ActiveGameNoteList.Remove(note);
    }

    public void ChangeScale(bool Init)
    {
        Vector3 scale = new Vector3((20 / Velocity) * 0.13f,0.13f,0.13f);
        PlayerGrid.transform.localScale = new Vector3(Velocity / 20, 1, 1);

        if (Init)
        {
            UpArrowObject[Chapter-1].transform.localScale = scale;
            UpLongArrowObject[Chapter - 1].transform.localScale = scale;

            LeftArrowObject[Chapter - 1].transform.localScale = scale;
            LeftLongArrowObject[Chapter - 1].transform.localScale = scale;

            RightArrowObject[Chapter - 1].transform.localScale = scale;
            RightLongArrowObject[Chapter - 1].transform.localScale = scale;

            DownArrowObject[Chapter - 1].transform.localScale = scale;
            DownLongArrowObject[Chapter - 1].transform.localScale = scale;
            ActionNote.transform.localScale = scale;
            YellowLine.transform.localScale = scale;
            RouteRecall.transform.localScale = scale;
        }
        else
        {
            foreach(Transform tr in ActiveGameNoteList)
            {
                tr.localScale = scale;
            }
        }

    }

    public void OnRHigh(bool miss)
    {
        if (miss)
        {
            Velocity = VelocityMemo;
        }
        else
        {
            Velocity += 0.01f;
        }
        ChangeScale(false);
    }

    public void SetOptions(bool random, bool runners, bool dark, bool fast, bool perfect, bool rhigh, float velocity, int chapter,int TopScore)
    {
        Velocity = 2f;
        RandomMode = random;
        RunnersMode = runners;
        DarkMode = dark;
        FastMode = fast;
        PerfectMode = perfect;
        RHighMode = rhigh;
        Velocity *= velocity;
        if(chapter > 3 || chapter < 1)
        {
            chapter = 1;
        }
        judgement = GetComponent<Judgement>();
        judgement.TopScore = TopScore;
        Chapter = chapter;
    }

    public void MusicStart(string name,string level,int Chapter)
    {
        string NoteString;
        ChangeScale(true);

        if(RHighMode)
            VelocityMemo = Velocity;
        
        //노트리스트를 다 비우고 시작한다.
        NowRoute = 0;
        EditOrPlayStart(true, Chapter);
        Gamestart = true;
       

        // Offset = (int)((18f / Velocity)*30f);    //1초당 30프레임이니까,

        //NoteList = Resources.Load("ChartResources/NotesJson/" + name + "/" + level) as TextAsset;
        NoteString = File.ReadAllText(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + name + "/" + level + ".txt");

        if (RunnersMask == null)
        {
            RunnersMask = GameObject.Find("RunnersMask");
        }
        if(ChapterBG == null)
        {
            ChapterBG = new GameObject[3];
            ChapterBG[0] = GameObject.Find("Chapter1BG");
            ChapterBG[1] = GameObject.Find("Chapter2BG");
            ChapterBG[2] = GameObject.Find("Chapter1BG");
        }
        for(int i = 0; i < 3; i++)
        {
            ChapterBG[i].SetActive(false);
        }
        ChapterBG[Chapter - 1].SetActive(true);

        RunnersMask.SetActive(RunnersMode);
       //if (NoteList == null)
           // return;
       
        NoteBook = JsonUtility.FromJson<NewNoteJson>(NoteString);
        if (NoteBook != null)
        {
            for(int i = 0; i < 3; i++)
            {
                Index[i] = NoteBook.NoteList[i].NewNoteList.Count;
                NoteIndex[i] = 0;
            }
            //나머지는 상속되어있는 noteclass에서 해준다.
        }
        //노트메이커->패스->저지먼트
        passScript.GameStart(Index[0], Index[1], Index[2]);

        Stopper = StartCoroutine(NumberLineCoroutine());

        Queueing();
        judgement.SetMusicName(NoteBook.MusicName, NoteBook.Level, NoteBook.IntLevel);
        if(FastMode)
            Time.timeScale = 1.5f;
    }

    public void StopCor()
    {
        StopCoroutine(Stopper);
    }

    IEnumerator NumberLineCoroutine()
    {
        AudioSource myAudio = soundManager.nowMusic;
        while (true)
        {
            float process = -1*( myAudio.time * 30f + Offset);
            NumberLine.transform.localPosition = new Vector2(process, 0);
            yield return new WaitForFixedUpdate();
        }
    }

    

    private bool FloatCmp(float a, float b)
    {
        //a>b
        if (a - b > 0.02f)
        {
            return true;
        }
        else{
            return false;
        }

    }

}
