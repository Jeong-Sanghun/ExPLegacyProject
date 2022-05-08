using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using System.Text;

public class NewJsonManager : NoteClass {

    public int BeatHelper = 3;
    public bool EditStartBool = false;
    int LIneLevel = 2;
    public Text saveText;
    public NewNoteJson myNotes;
    public RhythmTool rhythmTool;
    RhythmEventProvider rhythmEvent;
    public int Length = 9;
    public MouseJson mouseJson;
    AudioSource audioSource;
    NoteDrag noteDrag;
    public NoteMakerUIManager uiManager;

    public GameObject OneBeatLine;
    public GameObject EditorGrid;
    public GameObject[] RouteBackground;
    List<GameObject> OneBeatLineList = new List<GameObject>();
    List<Transform> BeatTransformList = new List<Transform>();

    [SerializeField]
    public List<List<GameObject>> RoutedObjects = new List<List<GameObject>>();
    //라인위에 있는거 꺼줘야되가지구

    List<int> RouteStartList = new List<int>();
    //루팅이 시작하는 부분의 프레임을 저장, 여길 만나면 0->1로 가자.
    //코루틴에서 0.5초전의 프레임과 현재의 프레임을 비교해서, 이 프레임 경계에 이게 있으면 루트 인덱스 바꾸기
    //20190201

    List<int> RouteEndList = new List<int>();
    //스타트와 동일. 1이든 2든 여기를 지나면 0으로 변한다.밖에서 여기로 들어오면 0->1
    //그 상태에서 버튼 누르면 2로 바꾸기다. 루팅하다가 여기로 나가면 2는 꺼주고 1은 켜주고.

    Dictionary<int, GameObject> NoteDictionary = new Dictionary<int, GameObject>();
    //이제부터 여기서 찾을거야. 순서 겹치는건 어쩔수 없지만. 몇프레임의 dictionary인지


    Vector2 OneBeatLineFirstpos = new Vector2(9, -0.3f);
    Vector2 QuarterBeatLineFirstPos = new Vector2(9, -0.5f);
    int OneBeatIndex = 0;
    int QuarterBeatIndex = 0;
    int ActionIndex = 0;
    public int Offset = 0;
    public float Scale;
    float firstScale = 1;
    public Text NowTimeText;

    public int IntLevel = 10;
    public int Chapter = 1;

    public ReadOnlyDictionary<int, Beat> myBeats = new ReadOnlyDictionary<int, Beat>();

    //일단 입력부터하자.

    float TimeForStopMoving = 0;
    int[] myTime = new int[7];
    //4 5 6 8 액션 분기 재분기
    bool KeyUp = true;
    char[] myType = { '4', '5', '6', '8','0','1','2' };
    bool OnAction = false;

    public int NowRoute;
    public string Level = "Easy";


    //액션노트가 끝나고 모션이 나오는 시간인 0.5초동안은 노트배치 못하게
    //마우스로 잘 입력했다면 액션노트가 끝나는 제한시간에 모션이 실행.
    //ui상에서는 롱노트처럼 뜨게, 빨간줄로 입력이 제한되는 곳 표시. 

    private void Start()
    {
       
         IsEditing = true;
        rhythmEvent = soundManager.rhythmEvent;
        audioSource = soundManager.GetComponent<AudioSource>();
        noteDrag = GetComponent<NoteDrag>();


    }

    void OnKeyUp(int index)
    {
        int currentFrame = rhythmTool.currentFrame - Offset;
        if (currentFrame - myTime[index] < Length) //짧게눌렀을때 단노트 넣기.
        {

            bool adjustment = false;
            Beat frame;
            for (int k = myTime[index] - BeatHelper; k < currentFrame + BeatHelper; k++)
            {
                if (myBeats.TryGetValue((int)k, out frame))
                {
                    myTime[index] = frame.index;
                    Debug.Log("보정됐어요");
                    adjustment = true;
                    break;
                }
            }

            GameObject DumyNote;
            EditNoteGenerator(myTime[index], myType[index], 0, adjustment, out DumyNote);
            myNotes.NoteList[NowRoute].NewNoteList.Add(new NewNote(myTime[index], myType[index], 0, ref DumyNote));
            RoutedObjects[NowRoute].Add(DumyNote);
        }
        else //f롱노트
        {
            bool adjustment = false;
            bool EndAdjustment = false;    //나올 때 비트보정.
            int LongNoteLength = 0;
            for (int k = myTime[index] - BeatHelper; k < myTime[index] + BeatHelper; k++)
            {
                Beat StartFrame;        //out에 쓸 startframe

                if (myBeats.TryGetValue((int)k, out StartFrame))
                {
                    myTime[index] = StartFrame.index;
                    adjustment = true;
                    break;
                }
            }


            for (float i = currentFrame - BeatHelper; i < currentFrame + BeatHelper; i++)
            {
                Beat OutFrame;
                if (myBeats.TryGetValue((int)i, out OutFrame))
                {
                    EndAdjustment = true;
                    LongNoteLength = OutFrame.index - myTime[index];  //엔드프레임이 보정되면 Length에 애드

                    break;
                }
            }
            if (EndAdjustment == false)//이거는 롱노트 뒤쪽 비트보정 안되는경우야.
            {
                LongNoteLength = (currentFrame - myTime[index]);   //되면 좋아/
            }
            GameObject DumyNote;
            EditNoteGenerator(myTime[index], myType[index], LongNoteLength, adjustment, out DumyNote);
            myNotes.NoteList[NowRoute].NewNoteList.Add(new NewNote(myTime[index], myType[index], LongNoteLength, ref DumyNote));
            RoutedObjects[NowRoute].Add(DumyNote);

        }
    }
    void OnAutoKeyUp(int index)
    {
        bool adjustment = false;
        /*
        Beat frame;
        for (int k = myTime[index] - BeatHelper; k < currentFrame + BeatHelper; k++)
        {
            if (myBeats.TryGetValue((int)k, out frame))
            {

                Debug.Log("보정됐어요");
                adjustment = true;
                break;
            }
        }*/

        GameObject DumyNote;
        EditNoteGenerator(myTime[index], myType[index], 0, adjustment, out DumyNote);
        myNotes.NoteList[NowRoute].NewNoteList.Add(new NewNote(myTime[index], myType[index], 0, ref DumyNote));
        RoutedObjects[NowRoute].Add(DumyNote);
    }

    public void Update()
    {
        if (EditStartBool)      //에딧 모드에 들어갔다면 업데이트를 돌린다. 추후에 오브젝트 켜고 끄기로 조절할거야.
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                myTime[0] = rhythmTool.currentFrame - Offset;
            }
           else if (Input.GetKeyDown(KeyCode.S))
            {
                myTime[1] = rhythmTool.currentFrame - Offset;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                myTime[2] = rhythmTool.currentFrame - Offset;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                myTime[3] = rhythmTool.currentFrame - Offset;
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                OnKeyUp(0);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                OnKeyUp(1);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                OnKeyUp(2);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                OnKeyUp(3);
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!OnAction)
                {
                    OnAction = true;
                    myTime[4] = rhythmTool.currentFrame-Offset;

                    //이 스크립트에서 환우형이 멈춰줘야해.
                    mouseJson.ActionStart = true;

                    GameObject DumyNote;
                    EditNoteGenerator(myTime[4], '0', 0, false, out DumyNote);
                    myNotes.NoteList[NowRoute].NewNoteList.Add(new NewNote(myTime[4], '0', 0, ref DumyNote));
                    //여기에 액션 키는거 넣어야돼. 마우스클릭하는거.
                }
                else
                {
                    mouseJson.ActionStart = false;
                    myNotes.ActionList[NowRoute].MouseNoteList.Add(mouseJson.mouseNote[ActionIndex]);
                    ActionIndex++;
                    mouseJson.mouseNoteReset();
                    OnAction = false;
                }

            }
            
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            OnScroll(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKey(KeyCode.DownArrow))
            OnScroll(-0.1f);
        if (Input.GetKey(KeyCode.UpArrow))
            OnScroll(0.1f);

        //스크롤 안되는 사람을 위해 쓴거고, ctrl+z는 deligate로 방금 수행한 거의 리스트를 받아서 쓰면 되겠다. 델리게이트 스택을 만들어.

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (EditStartBool)
            {
                audioSource.Pause();
                TimeForStopMoving = audioSource.time;
                EditStartBool = false;
                //Time.timeScale = 0;
            }
            else
            {
                if (TimeForStopMoving > 0)
                {
                    audioSource.time = TimeForStopMoving;
                }

                audioSource.Play();
                EditStartBool = true;
                //Time.timeScale = 1;
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //분기노트
            Routing();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!EditStartBool)
            {
                NumberLine.transform.localPosition = new Vector2(NumberLine.transform.localPosition.x - 15f, 0);
                if (TimeForStopMoving < audioSource.clip.length-0.5f)
                    TimeForStopMoving += 0.5f;
                NowTimeText.text = TimeForStopMoving.ToString("N1");
            }
            else
            {
                if (audioSource.time < audioSource.clip.length-0.5f)
                {
                    audioSource.time += 0.5f;
                }

            }

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!EditStartBool)
            {
                NumberLine.transform.localPosition = new Vector2(NumberLine.transform.localPosition.x + 15f, 0);
                if(TimeForStopMoving>0.5f)
                    TimeForStopMoving -= 0.5f;
                NowTimeText.text = TimeForStopMoving.ToString("N1");
            }
            else
            {
                if (audioSource.time > 0.5f)
                {
                    audioSource.time -= 0.5f;
                }

            }

        }


    }

    public void NoteInputStart(string MusicName)
    {
        uiManager = GameObject.Find("NoteMakerUIManager").GetComponent<NoteMakerUIManager>();
        EditStartBool = true;
        EditOrPlayStart(false, 0);
        myNotes = new NewNoteJson();
        myNotes.NoteList = new List<NewNoteWrapper>();

        for (int i = 0; i < 3; i++)
        {
            myNotes.NoteList.Add(new NewNoteWrapper());
            myNotes.NoteList[i].NewNoteList = new List<NewNote>();
            RoutedObjects.Add(new List<GameObject>());

        }        //분기추가

        myNotes.ActionList = new List<MouseNoteWrapper>();
        for (int i = 0; i < 3; i++)
        {
            myNotes.ActionList.Add(new MouseNoteWrapper());
            myNotes.ActionList[i].MouseNoteList = new List<MouseNote>();
        }



        myNotes.MusicName = MusicName;
        soundManager.MusicStart(MusicName);
        myBeats = rhythmTool.beats;
        //여기다가 scale을 bpm에 맞게 정해야돼

        myNotes.BPM = rhythmTool.bpm;
        uiManager.uiSetup();
        NewNote.uiManager = uiManager;
        //2019.01.05 bpm추가

        StartCoroutine(BeatIndexingDelay());
        //EditorGrid.GetComponent<Rigidbody2D>().velocity = new Vector2(-150, 0);

    }

    public void NoteAutoInputStart(string MusicName)
    {
        NoteInputStart(MusicName);
        /*
        ReadOnlyDictionary<int,Onset> a = rhythmTool.all.onsets;
        ICollection<int> du = a.Keys;
        int[] arr = new int[du.Count];
        du.CopyTo(arr, 0);
        foreach(int frame in arr)
        {
            //AutoVoid(frame)
      
           
        }*/
        
        rhythmEvent.Onset += AutoEvent;
        rhythmEvent.Beat += BeatEvent;
        //EditorGrid.GetComponent<Rigidbody2D>().velocity = new Vector2(-150, 0);
    }

    int lastFrame = 0;
    void AutoEvent(OnsetType type, Onset onset)
    {
        if (onset.strength < 30)
            return;
        if (Mathf.Abs(rhythmTool.currentFrame - lastFrame) < 4)
        {
            return;
        }
        lastFrame = rhythmTool.currentFrame;
        Debug.Log("스트렝쓰 : " + onset.strength +" 랭크 : " + onset.rank);
        switch (type)
        {
            case OnsetType.Low:
                myTime[0] = rhythmTool.currentFrame;
                OnAutoKeyUp(0);
                break;
            case OnsetType.Mid:
                myTime[1] = rhythmTool.currentFrame;
                OnAutoKeyUp(1);
                break;
            case OnsetType.High:
                myTime[2] = rhythmTool.currentFrame;
                OnAutoKeyUp(2);
                break;
            case OnsetType.All:
                myTime[3] = rhythmTool.currentFrame;
                OnAutoKeyUp(3);
                break;
        }
    }

    void AutoVoid(int frame, OnsetType type, Onset onset)
    {
        if (onset.strength < 30)
            return;
        if (Mathf.Abs(frame - lastFrame) < 4)
        {
            return;
        }
        lastFrame = frame;
        Debug.Log("스트렝쓰 : " + onset.strength + " 랭크 : " + onset.rank);
        switch (type)
        {
            case OnsetType.Low:
                myTime[0] = frame;
                OnAutoKeyUp(0);
                break;
            case OnsetType.Mid:
                myTime[1] = frame;
                OnAutoKeyUp(1);
                break;
            case OnsetType.High:
                myTime[2] = frame;
                OnAutoKeyUp(2);
                break;
            case OnsetType.All:
                myTime[3] = frame;
                OnAutoKeyUp(3);
                break;
        }
    }

    void BeatEvent(Beat beat)
    {
        if (Mathf.Abs(rhythmTool.currentFrame - lastFrame) < 4)
        {
            return;
        }
        lastFrame = rhythmTool.currentFrame;
        int a = Random.Range(0, 3);
        myTime[a] = rhythmTool.currentFrame;
        OnAutoKeyUp(a);
    }

    IEnumerator BeatIndexingDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("잘된다");
        int BeatIndexLength = myBeats.Count;
        //이게 안쉬고 하니까 비트 인덱스를 못받아온다. 
        ICollection<Beat> BeatsCollection = myBeats.Values;
        Beat[] BeatsArray = new Beat[BeatIndexLength];
        BeatsCollection.CopyTo(BeatsArray, 0);
        EditorGrid.transform.localScale = new Vector3(Scale, 1,1);
        //애초에 모든 비트를 다 받은다음에 그걸 그려주는거야. 
       
        for (int i = 0; i < BeatIndexLength; i++)
        {
            OneBeatLineList.Add(Instantiate(OneBeatLine, NumberLine.transform));
            OneBeatLineList[i].transform.localPosition = new Vector2(BeatsArray[i].index, -0.3f);
            OneBeatLineList[i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (BeatsArray[i].index / 30f).ToString("N1");
            BeatTransformList.Add(OneBeatLineList[i].transform);
        }

        //NumberLine.GetComponent<Rigidbody2D>().velocity = new Vector2(-30.0f * Scale, 0);

        foreach (Transform transForm in BeatTransformList)
        {
            transForm.localScale = new Vector3(1.0f / Scale, transform.localScale.y, transform.localScale.z);

        }
        foreach (Transform transForm in NoteTransformList)
        {
            transForm.localScale = new Vector3(0.13f * 1.0f / Scale, 0.13f, 0.13f);
        }

        firstScale = Scale;

        while (true)
        {
            if (EditStartBool)
            {
                NowTimeText.text = (rhythmTool.currentFrame / 30f).ToString("N1");
                NumberLine.transform.localPosition = new Vector2(-rhythmTool.currentFrame, 0);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void OnScroll(float delta)
    {
        Scale += delta*0.1f; //scale은 delta만큼 변한다, 만히 휠하면 0.2, 한번하면 0.1이다.
        if (Scale < 0.01f)
        {
            Scale = 0.01f;
        }
        if(Scale < firstScale / 8)
        {
            Scale = firstScale / 8;

        }
        if (Scale > firstScale * 4)
        {
            Scale = firstScale * 4;
        }
        //LIneLevel은 빠른게0, 촘촘한게 4. 4페이즈까지 있게 하자. 1 0.5 0.25 0.125 까지만 하자.
        if (delta < 0 && Scale < firstScale/LIneLevel)
        {

            int i = 0;

           if(LIneLevel >=4)
            {
                foreach (Transform transForm in BeatTransformList)
                {

                    if (i % LIneLevel != 0)
                    {
                        BeatTransformList[i].GetChild(2).gameObject.SetActive(false);
                        BeatTransformList[i].GetChild(1).gameObject.SetActive(false);
                    }
                    i++;
                }
            }
            else
            {
                foreach (Transform transForm in BeatTransformList)
                {
                    if (i % 4 != 0)
                    {
                        BeatTransformList[i].GetChild(1).gameObject.SetActive(false);
                    }
                    i++;
                }
            }

            LIneLevel *= 2;
            //1일때 2, 0.5일떄 4, 0.25일때 8, 0.125일때 16
        }
        else if (delta > 0 && LIneLevel != 2 &&  Scale > firstScale / LIneLevel)
        {

            int i = 0;


            //저번에 껐던것들다 되돌리는거잖아.
            //2면 저번에 꺼졌던 1 3 5 7 9 를 원상복구해줘야한다
            if(LIneLevel >=8)
            {
                foreach (Transform transForm in BeatTransformList)
                {
                    if (i % LIneLevel != 0)
                    {
                        BeatTransformList[i].GetChild(2).gameObject.SetActive(true);
                    }
                    i++;
                }
            }
            else if(LIneLevel>=4)
            {
                foreach (Transform transForm in BeatTransformList)
                {
                    if (i % LIneLevel != 0)
                    {
                        BeatTransformList[i].GetChild(1).gameObject.SetActive(true);
                        
                    }
                    i++;
                }
            }
            else
            {
                Debug.Log("이거 하긴 하냐?");
                foreach (Transform transForm in BeatTransformList)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            LIneLevel /= 2;

        }


        //수직선의 스케일을 scale로 정해준다. 스케일이 커질수록 수직선 눈금의 속도도 빨라져야 한다.
        EditorGrid.transform.localScale = new Vector3(Scale, 1, 1);
        //NumberLine.GetComponent<Rigidbody2D>().velocity = new Vector2(-30.0f * Scale, 0);

        
        foreach(Transform transForm in BeatTransformList)
        {
            transForm.localScale = new Vector3(1.0f / Scale, transform.localScale.y, transform.localScale.z);

        }
        Vector2 body = new Vector2(4.5f, -0.005f);
        foreach (Transform transForm in NoteTransformList)
        {
            if (transForm.CompareTag("LongNote"))
            {
                //이 실행 전에 지금 스케일을다 엎어준거니까, 노트스케일을 원상복구해주는 단계잖아
                //그 과정에서 바디하고 테일의 위치는 일정하게 해주고, 크기만 바꿔줘야되는거지.
                //그래서 바디하고 테일을 떼준 상태에서 스케일 변화시켜주고, 다시 노트헤드에 붙여주는거야. 잘했다.
                if (transForm.gameObject.activeSelf)
                {
                    Transform Body = transForm.GetChild(0);
                    Transform Tail = transForm.GetChild(1);


                    Body.SetParent(null);
                    Tail.SetParent(null);

                    transForm.localScale = new Vector3(0.13f / Scale, 0.13f, 0.13f);
                    Body.SetParent(transForm);
                    Tail.SetParent(transForm);


                    Body.SetSiblingIndex(0);
                    Tail.SetSiblingIndex(1);

                    //순서가 바뀐다. 차일드 순서가 바뀐다.

                    /*
                    BoxCollider2D one = Body.GetComponent<BoxCollider2D>();
                    BoxCollider2D two = Tail.GetComponent<BoxCollider2D>();

                    StartCoroutine(BodyScale(Body, one, two));
                    */
                    Body.localPosition = body;
                    //Body.localScale = new Vector3((Tail.localPosition.x-Body.localPosition.x)/0.0047f,1,1);
                    Tail.localScale = new Vector3(1,1,1);
                    
                    

                }
                else
                {
                    transForm.localScale = new Vector3(0.13f * 1.0f / Scale, 0.13f, 0.13f);
                }

            }
            else
            {
                transForm.localScale = new Vector3(0.13f * 1.0f / Scale, 0.13f, 0.13f);
                //이거 그냥 테일을 그 프레임에 고정한다고 생각하자. 테일 떼고 원래 로컬포지션에 넣고, 그런식으로 하자.
            }
        }

        //이제 위치보정을 해 주어야 한다. 스케일이 늘어난 만큼 할라그랬는데, 스케일의 피벗보정이 되는거였다.

    }

    public void NoteInputEnd(string MusicName,string level, int numberLevel, int chapter)
    {
        if (myNotes.MusicName == MusicName)
        {
            
            int count;
            bool Sorted = false;
            bool SameTime = false;
            int k = 0;
            myNotes.RoutingIndexFirst = new List<int>();
            myNotes.RoutingIndexSecond = new List<int>();
            myNotes.ActionIndexFirst = new List<int>();
            myNotes.ActionIndexSecond = new List<int>();
            while (k<3)
            {
                Sorted = true;
                SameTime = true;
                count = myNotes.NoteList[k].NewNoteList.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    if (myNotes.NoteList[k].NewNoteList[i].NoteTime > myNotes.NoteList[k].NewNoteList[i + 1].NoteTime)
                    {
                        NewNote Dumy = myNotes.NoteList[k].NewNoteList[i + 1];
                        myNotes.NoteList[k].NewNoteList.Remove(myNotes.NoteList[k].NewNoteList[i + 1]);
                        myNotes.NoteList[k].NewNoteList.Insert(i, Dumy);
                        Sorted = false;
                    }
                }
                if (Sorted)
                {
                    while (!SameTime)
                    {
                        for (int i = 0; i < count - 1; i++)
                        {
                            if (myNotes.NoteList[k].NewNoteList[i].NoteTime - myNotes.NoteList[k].NewNoteList[i + 1].NoteTime > -8)
                            {
                                myNotes.NoteList[k].NewNoteList.Remove(myNotes.NoteList[k].NewNoteList[i + 1]);
                                count--;
                                i--;
                                SameTime = false;
                            }
                        }
                    }
                    k++;
                }

            }
            k = 1;
            while (k < 3)
            {
                count = myNotes.NoteList[k].NewNoteList.Count;
                int[] action = new int[2];
                if (k != 0)
                {
                    action[k - 1] = 0;
                    for (int j = 0; j < count; j++)
                    {
                        if (myNotes.NoteList[k].NewNoteList[j].NoteType == '1')
                        {
                            //분기 끝 노트가 1이다. 0 1 2 액션 분기끝 분기시작
                            if (k == 1)
                            {
                                myNotes.RoutingIndexFirst.Add(j+1);
                                myNotes.ActionIndexFirst.Add(action[0]);

                            }
                            else if (k == 2)
                            {
                                myNotes.RoutingIndexSecond.Add(j+1);
                                myNotes.ActionIndexSecond.Add(action[1]);
                            }
                        }
                        else if (myNotes.NoteList[k].NewNoteList[j].NoteType == '0')
                        {
                            action[k - 1]++;
                        }
                    }
                }
                k++;
            }


            myNotes.Level = level;
            myNotes.IntLevel = numberLevel;
            myNotes.Chapter = chapter;



            EditStartBool = false;
            //StopAllCoroutines();
            string JsonText = JsonUtility.ToJson(myNotes,true);
            StreamWriter myFile;

            if (!Directory.Exists(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicName + "/"))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicName + "/");
            }
            myFile = new StreamWriter(Application.streamingAssetsPath+ "/ChartResources/NotesJson/" + MusicName + "/" + level + ".txt");

            /*
#if UNITY_STANDALONE_WIN
            if (!Directory.Exists(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicName + "/"))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicName + "/");
            }
            myFile = new StreamWriter(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + MusicName + "/" + level + ".txt");

#endif*/
            myFile.Write(JsonText);
            myFile.Close();
            myNotes.NoteList.Clear();   
            soundManager.nowMusic.Stop();

            MusicsFormat wholeFormat;
           // TextAsset NoteList = Resources.Load("ChartResources/Format/MusicFormat") as TextAsset;
            string NoteString;

            NoteString = File.ReadAllText(Application.streamingAssetsPath + "/ChartResources/Format/MusicFormat.txt");



            wholeFormat = JsonUtility.FromJson<MusicsFormat>(NoteString);
            Debug.Log(NoteString);
            bool exist = false;
            bool strEx = false;
            bool intEx = false;
            int index = 0;
            

            foreach (JustFormat just in wholeFormat.MusicList)
            {
                int countLv = just.LevelIntList.Count;
                if (just.MusicName.Contains(MusicName))
                {
                    just.ChapterNum = chapter;
                    exist = true;
                    for (int i = 0; i < countLv; i++)
                    {
                        if (level == just.LevelStringList[i])
                        {
                            strEx = true;
                            if (IntLevel == just.LevelIntList[i])
                            {
                                intEx = true;

                            }
                        }
                    }
                    break;

                }
                index++;
            }
            if(exist == false)
            {
                JustFormat dumy = new JustFormat(MusicName);
                dumy.ChapterNum = chapter;
                dumy.LevelStringList.Add(level);
                dumy.LevelIntList.Add(IntLevel);
                wholeFormat.MusicList.Add(dumy);
            }
            else if(strEx == true && intEx == true)
            {
                return;

            }
            else
            {
                wholeFormat.MusicList[index].LevelStringList.Add(level);
                wholeFormat.MusicList[index].LevelIntList.Add(IntLevel);
            }
            string formatTxt = JsonUtility.ToJson(wholeFormat, true);
            StreamWriter formatFile = new StreamWriter( Application.streamingAssetsPath + "/ChartResources/Format/MusicFormat.txt");
            formatFile.Write(formatTxt);
            formatFile.Close();


        }
        saveText.gameObject.SetActive(true);
        LevelLoader.inst.LoadLevel("NoteMakerGuide", LevelLoader.SceneType.Gallery);
    }  

    public override void EditOrPlayStart(bool isPlaying,int chapter)
    {
        NowRoute = 0;
        base.EditOrPlayStart(isPlaying,0);
    }

    float RouteStartTime = 0;
    int RouteEndTime = 0;
    Coroutine RouterCor;

    public void Routing()
    {

        if (RouterCor != null)
        {
            StopCoroutine(RouterCor);
            Debug.Log("스탑은하니?");
        }
        if (NowRoute == 0)
        {
          
            //첫 번쨰 분기할 떄, 루트를 1로 잡아주고, 
            myNotes.routes++;   //route가 0이면 단일길, 1이면 1번분기, 2면 2번분기..
            myTime[5] = rhythmTool.currentFrame - Offset;
            RouteStartList.Add(myTime[5]); //분기점 시작 저장
        
            RouteStartTime = audioSource.time;
            GameObject Dumy;
            NowRoute = 0;
            EditNoteGenerator(myTime[5], '2', 0, false, out Dumy);
            myNotes.NoteList[0].NewNoteList.Add(new NewNote(myTime[5], '2', 0, ref Dumy));
            NowRoute = 1;
            Debug.Log("Routed");
            if (EditStartBool)
            {
                audioSource.Pause();
                TimeForStopMoving = audioSource.time;
                EditStartBool = false;
            }
            RoutedNotesToggle();
            RouteBackground[0].SetActive(true);

            
        }
        else if(NowRoute == 1)
        {
            //한 번 누른상태에서 다시 누르면.
            myTime[6] = rhythmTool.currentFrame - Offset;
            RouteEndList.Add(myTime[6]);//분기점 끝 저장


            RouteEndTime = myTime[6];

            NowRoute = 0;
            GameObject Dumy;
            EditNoteGenerator(myTime[6], '1', 0, false, out Dumy);
            myNotes.NoteList[1].NewNoteList.Add(new NewNote(myTime[6], '1', 0, ref Dumy));
            myNotes.NoteList[2].NewNoteList.Add(new NewNote(myTime[6], '1', 0, ref Dumy));
            NowRoute = 2;

            audioSource.time = RouteStartTime;
            RouteBackground[1].SetActive(true);

            RoutedNotesToggle();
            StartCoroutine(RouteEnd());
            //얘가 버그 다만든다.
        }
    }

    IEnumerator RouteEnd()
    {
        yield return new WaitForSeconds(1f);
        bool end = false;
        while (!end)
        {
            int nowTime = rhythmTool.currentFrame;
            int count = RouteEndList.Count;
            Debug.Log(RouteStartList[count - 1] + "wait" + RouteEndList[count - 1]);
            if (nowTime > RouteStartList[count-1] && nowTime < RouteEndList[count-1])
            {

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                end = true;
                NowRoute = 0;
                RoutedNotesToggle();
                RouterCor = StartCoroutine(RouteChecker());
            }
        }
        
    }

    IEnumerator RouteChecker()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int counts = RouteEndList.Count;
            int nowTime = rhythmTool.currentFrame;
            if(NowRoute == 0)
            {
                for (int i = 0; i < counts; i++)
                {
                    if (nowTime > RouteStartList[i] && nowTime < RouteEndList[i])
                    {
                        Debug.Log("start로 들어감");
                        NowRoute++;
                        RoutedNotesToggle();

                        break;
                    }
                }
            }
            else if(NowRoute == 1 || NowRoute == 2)
            {
                for (int i = 0; i < counts; i++)
                {
                    if (!(nowTime > RouteStartList[i] && nowTime < RouteEndList[i]))
                    {
                        Debug.Log("End로 들어감");
                        NowRoute = 0;
                        RoutedNotesToggle();
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    public void RoutedNotesToggle()
    {
        //NowRoute변수값을 더한다음에 얘를 호출해야한다. 이거 좃될수도 잇겠다
        //20190201 함수 쓴 날짜
        Debug.Log("현재루트 : " + NowRoute);
        if(NowRoute == 0)
        {
            //2->0
            RouteBackground[0].SetActive(false);
            RouteBackground[1].SetActive(false);
            foreach (GameObject notes in RoutedObjects[0])
            {
                notes.SetActive(true);
            }
            foreach (GameObject notes in RoutedObjects[2])
            {
                notes.SetActive(false);
            }
            foreach (GameObject notes in RoutedObjects[1])
            {
                notes.SetActive(true);
            }
        }
        else if(NowRoute == 1)
        {
            RouteBackground[0].SetActive(true);
            RouteBackground[1].SetActive(false);
            foreach (GameObject notes in RoutedObjects[0])
            {
                notes.SetActive(false);
            }
            foreach (GameObject notes in RoutedObjects[1])
            {
                notes.SetActive(true);
            }
            foreach (GameObject notes in RoutedObjects[2])
            {
                notes.SetActive(false);
            }
        }
        else if(NowRoute == 2)
        {
            //1인 경우는 취급을 안하는게, 0 -> 1로 넘어간거라 할필요없다
            //0->2는 고려하지 않는다.
            RouteBackground[0].SetActive(false);
            RouteBackground[1].SetActive(true);
            foreach (GameObject notes in RoutedObjects[0])
            {
                notes.SetActive(false);
            }
            foreach (GameObject notes in RoutedObjects[1])
            {
                notes.SetActive(false);
            }
            foreach (GameObject notes in RoutedObjects[2])
            {
                notes.SetActive(true);
            }
        }
    }

}
