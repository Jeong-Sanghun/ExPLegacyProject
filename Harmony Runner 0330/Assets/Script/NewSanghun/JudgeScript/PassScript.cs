
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassScript : MonoBehaviour
{

    //롱노트 바로 다음에 단노트 나오면 입력 못할 수도 있음.

    public Judgement judgement;
    public NewNoteMaker noteMaker; //러너스하이 때 스케일 줄어드는거 막으려고
    public NewSoundManager soundManager;
    public AudioSource Tick;
    public MousePos mousePos;
    public GameObject NumberLine;
    public int WholeNote;
    Coroutine LongNoteCor;

    
    NewNote[,] MyNoteDetailQ;
    
    public GameObject[,] MyNotesQ;
    int[] ActionNoteIndex;


    public bool GameStarted = false;
    public int JudgeMiss = 12;
    public int JudgePerfect = 2;
    public int JudgeGood = 4;
    /*
    public bool OnRightNote = false; //요거는 제대로 된 것을 누르고 있느냐 판정하는거야.
    public bool IsGrey = false;
    public bool IsGreyToWhite = false;
    public bool LongNoteEndCoroutine = false;
    public bool IsLongNoteEnd = false;
    public bool IsHolded = false;

    public bool IsKeyUpOK = false;
    public bool LongNoteComboCoroutine = false;
    public bool StartMissCoroutine = false;
    public bool OnLongNoteNow = false;
    */
    public bool MissInStart = false;
    public int[] QueueIndex;
    public int[] DequeueIndex;
    int[] ObjectSize;
    int QueueSize;

    public float BarrierVelocity;
    public Coroutine MissCor;

    public List<int>[] RoutingIndexList;
    public List<int>[] ActionIndexList;
    int RoutedTimes;
    public bool RHighMode;

    //단노트 미스도 롱노트 미스에서 해결해주고있다.


    public void IndexList(List<int> routingList1, List<int> routingList2, List<int> actionList1, List<int> actionList2)
    {
        RoutedTimes = 0;
        RoutingIndexList[0] = routingList1;
        RoutingIndexList[1] = routingList2;
        ActionIndexList[0] = actionList1;
        ActionIndexList[1] = actionList2;
        if (DetailPeek().LongNoteLength != 0)
        {
            StartCoroutine(LongNoteTimeChecker(DetailPeek(), GameobjectPeek()));
            //이거 시작할떄 롱노트 있으면 돌려야돼. 없으면 좃댐;;;
        }
    }

    public void GameStart(int index0,int index1, int index2)
    {
        //Gamestart가 먼저고, 큐잉이 나중에다. 
        //MyNotesQ = new GameObject[QueueSize];
        int max = index0;
        if (index0 > index1)
        {
            if (index0 > index2)
            {
                max = index0;
            }
            else
            {
                max = index2;
            }
        }
        else
        {
            if (index1 > index2)
            {
                max = index1;
            }
            else
            {
                max = index2;
            }
        }
        QueueIndex = new int[3];
        DequeueIndex = new int[3];
        ActionNoteIndex = new int[3];
        for (int i = 0; i < 3; i++)
        {
            QueueIndex[i] = 0;
            DequeueIndex[i] = 0;
            ActionNoteIndex[i] = 0;
        }

        ObjectSize = new int[3];
        ObjectSize[0] = index0;
        ObjectSize[1] = index1;
        ObjectSize[2] = index2;
        MyNotesQ = new GameObject[3, max];
        MyNoteDetailQ = new NewNote[3, max];
        

        QueueSize = max;
        MissCor = StartCoroutine(NoteMiss());

        RoutingIndexList = new List<int>[2];
        ActionIndexList = new List<int>[2];
        judgement.GameStart();
        RHighMode = judgement.RHighMode;
        noteMaker = judgement.noteMaker;
    }

    public void GameClear()
    {
        StopCoroutine(MissCor);
        if (LongNoteCor != null)
        {
            StopCoroutine(LongNoteCor);
        }
        
        judgement.GameClear();

    }

    public void QueueingEnd(bool routed)
    {
        for (int i = 0; i < ObjectSize[1]; i++)
        {
            if(MyNotesQ[1,i] != null)
                MyNotesQ[1, i].SetActive(routed);
        }
        for (int i = 0; i < ObjectSize[2]; i++)
        {
            if (MyNotesQ[2, i] != null)
                MyNotesQ[2, i].SetActive(!routed);
        }
    }

    private void Update()
    {
        if (GameStarted)    //게임이 시작되면 update가 돌아간다, 이건 향후에 UI완성되면 오브젝트 켜고 끄는거로 할거야.
        {
            if (Input.GetKeyDown(KeyCode.S))    //아래키 눌렀을 때 노트 판정하기.
            {
                OnNoteTrigger('5');         //노트가 입력 될 때 불러오는 함수.
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                OnNoteTrigger('8');
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                OnNoteTrigger('6');
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                OnNoteTrigger('4');
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                OnNoteTrigger('0');
            }
            //이제부터 롱노트할거야, 롱노트 기본 알고리즘은,
            //노트큐와 롱큐가 같이 있는데 손을 떼면 노트가 틀린거로 판정해주는거지. 
            //노트가 틀리면 반투명하게 바꾸기.
            /*
            if (Input.GetKeyUp(KeyCode.S))    //키를 뗏을 때 롱노트가 있다면 없애기.
            {
                LongNoteKeyUp('5');
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                LongNoteKeyUp('8');
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                LongNoteKeyUp('6');
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                LongNoteKeyUp('4');
            }*/
        }
    }

    public void Enqueue(NewNote Detail)
    {
        //newnotemaker의 update에서 실행해줌.
        MyNoteDetailQ[NewNoteMaker.NowRoute,QueueIndex[NewNoteMaker.NowRoute]] = Detail;


        QueueIndex[NewNoteMaker.NowRoute]++;
        if (QueueIndex[NewNoteMaker.NowRoute] == ObjectSize[NewNoteMaker.NowRoute])
        {
            QueueIndex[NewNoteMaker.NowRoute] = 0;
        }
        //Debug.Log(QueueIndex[NewNoteMaker.NowRoute] + "은 " + NewNoteMaker.NowRoute);
    }

    public void ObjectEnqueue(GameObject Note)
    {
        //요거는 noteclass의 NoteGenerator메서드에서 해준다.
       // Debug.Log(QueueIndex[NewNoteMaker.NowRoute] + "오브젝트");
        MyNotesQ[NewNoteMaker.NowRoute,QueueIndex[NewNoteMaker.NowRoute]] = Note;
    }

    public GameObject ObjectDequeue()
    {

        GameObject dumy;
        float longNoteTime = DetailPeek().LongNoteLength;
        char type = DetailPeek().NoteType;
        if (longNoteTime == 0)
        {
            if (!(type == '1' || type == '2' || type == '0'))
                judgement.WholeComboAdd(1);
        }
        else
        {
            judgement.WholeComboAdd(2 + (int)(longNoteTime-3)/6);
        }
        if(DequeueIndex[0] == ObjectSize[0]-1)
        {
            Invoke("GameClear", 3);
        }
        if (DequeueIndex[NewNoteMaker.NowRoute] == ObjectSize[NewNoteMaker.NowRoute])
        {
            dumy = MyNotesQ[NewNoteMaker.NowRoute,0];
            MyNotesQ[NewNoteMaker.NowRoute,0] = null;
            MyNoteDetailQ[NewNoteMaker.NowRoute,0] = null;
            DequeueIndex[NewNoteMaker.NowRoute] = 0;
            if(RHighMode)
                noteMaker.DeleteNoteinList(dumy.transform);
            return dumy;
        }
        else
        {
            dumy = MyNotesQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute]];
            //Debug.Log(dumy.transform.localPosition.x + "피크는 : " + DetailPeek().NoteTime);
            MyNotesQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute]] = null;
            MyNoteDetailQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute]] = null;
            DequeueIndex[NewNoteMaker.NowRoute]++;
            if (RHighMode)
                noteMaker.DeleteNoteinList(dumy.transform);
            return dumy;
        }
    }

    public NewNote DetailPeek()
    {
        if (DequeueIndex[NewNoteMaker.NowRoute] == ObjectSize[NewNoteMaker.NowRoute])
        {
           
            return MyNoteDetailQ[NewNoteMaker.NowRoute, 0];
        }
        else
        {
          
            return MyNoteDetailQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute]];
        }
    }
    //여기서 그 전 피크 꺼도 알아야해.
    public NewNote DetailForward()
    {

        if (DequeueIndex[NewNoteMaker.NowRoute] == QueueSize)
        {
            return MyNoteDetailQ[NewNoteMaker.NowRoute,0];
        }
        else
        {
            return MyNoteDetailQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute] + 1];
        }
    }

    public GameObject GameobjectPeek()
    {

        if (DequeueIndex[NewNoteMaker.NowRoute] == ObjectSize[NewNoteMaker.NowRoute])
        {
            return MyNotesQ[NewNoteMaker.NowRoute,0];
        }
        else
        {
            return MyNotesQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute]];
        }
    }

    public GameObject GameobjectForward()
    {

        if (DequeueIndex[NewNoteMaker.NowRoute] == QueueSize)
        {
            return MyNotesQ[NewNoteMaker.NowRoute,QueueSize];
        }
        else
        {
            return MyNotesQ[NewNoteMaker.NowRoute,DequeueIndex[NewNoteMaker.NowRoute] + 1];
        }
    }

    //배열을 만든 이유는 그 다음 노트가 뭔지 알기 위해서야. 그러면 뒷노트가 보이니까 좋지!

    void OnNoteTrigger(char myType)
    {

        if (DetailPeek() == null)  //만약 큐에 아무것도 없다면, 허공에다가 누른거라면 아무것도 안하기.
        {
            return;
        }
        if (DetailPeek().NoteTime - soundManager.rhythmTool.currentFrame>JudgeMiss)
        {
            return;
        }
        NewNote detail = DetailPeek();
        if (detail.LongNoteLength != 0)
        {
            float PushTime = (detail.NoteTime - soundManager.rhythmTool.currentFrame);
            if(myType == detail.NoteType)
            {
                if (PushTime < JudgePerfect)
                {
                    judgement.Perfect(PushTime);
                    MissInStart = false;
                }
                else if (PushTime < JudgeGood)
                {
                    judgement.Good(PushTime);
                    MissInStart = false;
                }
                else if (PushTime < JudgeMiss)
                {
                    judgement.Miss(PushTime);
                    MissInStart = true;
                }
            }
            else
            {
                judgement.Miss(-1);
                MissInStart = true;
            }
          
        }
        else    //단노트일 때 판정.
        {
            float PushTime = Mathf.Abs(detail.NoteTime - soundManager.rhythmTool.currentFrame);
            //뭐가 있는 상태로 눌렀으면 그거를 꺼주고, 큐에서 없에준다
            if (detail.NoteType == '2')
            {
                if (PushTime < JudgeGood && myType == '0')
                    RoutingNoteDequeue(true);
                else
                    RoutingNoteDequeue(false);
                return;
            }
            else if (detail.NoteType == '1')
            {
                Debug.Log("리콜");
                RoutingRecall();
                return;
            }
            if (detail.NoteType == myType)             //요거는 판정, 마찬가지.
            {
                //맞았을 때 판정해준다
                if (myType == '0')
                {
                    //액션노트일때만이야.
                    //여기서 뭔가를 해줘야겟징
                    if (PushTime < JudgePerfect)
                    {
                        JustNoteDequeue(2, PushTime);
                        mousePos.MakeActionNote(ActionNoteIndex[NewNoteMaker.NowRoute]);
                        judgement.ActionAdd(true);
                        ActionNoteIndex[NewNoteMaker.NowRoute]++;
                    }
                    else if (PushTime < JudgeGood)
                    {
                        JustNoteDequeue(1, PushTime);
                        mousePos.MakeActionNote(ActionNoteIndex[NewNoteMaker.NowRoute]);
                        judgement.ActionAdd(true);
                        ActionNoteIndex[NewNoteMaker.NowRoute]++;
                    }
                    else if (PushTime < JudgeMiss)
                    {
                        StartCoroutine(NoteFadeOut(false));
                        judgement.ActionAdd(false);
                        ActionNoteIndex[NewNoteMaker.NowRoute]++;
                    }


                }
                else
                {
                    if (PushTime < JudgePerfect)
                    {
                        JustNoteDequeue(2, PushTime);
                    }
                    else if (PushTime < JudgeGood)
                    {
                        JustNoteDequeue(1, PushTime);
                    }
                    else if (PushTime < JudgeMiss)
                    {
                        JustNoteDequeue(0, PushTime);
                    }
                }


            }
            else if (PushTime < JudgeMiss)
            {
                JustNoteDequeue(0, -1);

            }
        }


    }
    
    IEnumerator NoteMiss()
    {
        //이 경우는 만약 노트를 안 눌렀을 때를 위해 만든거니까, 노트를 눌렀다면 해제되게 하자.
        //롱노트가 시작될 때 시작해준다. 
        //만약 시간이 넘어버려서 틀렸다면. 하지만 이 시간 내로 입력이 들어왔다면. 입력들어오는건 update에서 받아줘야해.
        //절대 입력을 코루틴에서 받으면 안된다. 그 입력은 longnotecolorchange에서 해준다
        //판정범위 안에 들어왔는가. 들어왔다면 판정범위를 나가는가 코루틴을 실행.
        //판정범위 안에서 판정이 되었으면 이걸 끈다.
        //롱노트 시작판정.
        while (true)
        {
            if (DetailPeek() != null && DetailPeek().LongNoteLength == 0)
            {
                float dumy = soundManager.rhythmTool.currentFrame - DetailPeek().NoteTime;
                if (dumy > JudgeGood)
                {
                    if (DetailPeek().NoteType == '2')
                    {
                        RoutingNoteDequeue(false);
                    }
                    else if (DetailPeek().NoteType == '1')
                    {
                        RoutingRecall();
                    }
                    else if(DetailPeek().NoteType == '0')
                    {
                        judgement.ActionAdd(false);
                        StartCoroutine(NoteFadeOut(false));
                    }
                    else
                    {
                        JustNoteDequeue(0, -1);
                    }

                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

        /*
    IEnumerator LongNoteEnd()
    {
        while (true)
        {
            if (DetailPeek().LongNoteLength != 0)
            {
                if (soundManager.rhythmTool.currentFrame - DetailPeek().NoteTime > DetailPeek().LongNoteLength - 1)
                {
                    StartCoroutine(NoteFadeOut(true));
                    Debug.Log("여기지");
                    break;
                }
            }
            yield return null;
        }
    }

    IEnumerator OnLongNote()
    {
        //콤보 올라가는거 넣어줘야되고, 판정도 넣어줘야해. 콤보올라가는거도 판정에서 해줄거니까 상관안해도 되겠다
        float ComboTime = 0f;
        // && NoteDetailQ.Peek().LongNoteLength > 0
        //콤보가 올라가다가 중간에 놓게되면 어떡해. miss를 해줘야 할 것 아니야. 근데 이거는 그저 콤보를 올릴 뿐인데....
        //중간에 놓았는지를 여기다가 놓으면 되겠다.
        while (true)
        {
            if (ComboTime >= 0.3f)
            {
                judgement.Perfect(-1);
                ComboTime = 0;
            }
            yield return new WaitForSeconds(0.1f);
            ComboTime += 0.1f;
        }
    }
    */
    IEnumerator NoteFadeOut(bool isItLong)
    {
        //longnote getchild(3)
        //short note getchild(0)
        GameObject[] myRb = new GameObject[2];
        GameObject myGo;
        
        //이제 이 아래서부터 쓰이는 피크는 다 문제야.
        myGo = ObjectDequeue();
        if(myGo == null)
        {
            //StopCoroutine(NoteFadeOut(true));
        }
        //오브젝트 디큐에 문제가있따
        myGo.transform.SetParent(null);

        float a = 0.9f;
        if (!isItLong)
        {
            SpriteRenderer[] myRenderer;
            myRenderer = myGo.GetComponentsInChildren<SpriteRenderer>();
            myRb[1] = myGo.transform.GetChild(0).gameObject;
            while (a > 0)
            {
                foreach(SpriteRenderer spr in myRenderer)
                {
                    spr.color = new Color(1, 1, 1, a);
                }
                a -= 0.1f;
                myRb[1].transform.localPosition = new Vector2(myRb[1].transform.localPosition.x - 3f, myRb[1].transform.localPosition.y);
                yield return new WaitForSeconds(0.05f);
            }
            foreach (SpriteRenderer spr in myRenderer)
            {
                spr.color = new Color(1, 1, 1, 1);
            }
        }
        else
        {
            SpriteRenderer[] Sprites = new SpriteRenderer[3];
            Sprites = myGo.GetComponentsInChildren<SpriteRenderer>();
            myRb[1] = myGo.transform.GetChild(3).gameObject;
            while (a > 0)
            {
                foreach (SpriteRenderer NoteColor in Sprites)
                {
                    NoteColor.color = new Color(1, 1, 1, a);    //아무튼 색을 바꾼다;;;
                }
                a -= 0.1f;
                myRb[1].transform.localPosition = new Vector2(myRb[1].transform.localPosition.x - 1f, myRb[1].transform.localPosition.y);
                yield return new WaitForSeconds(0.05f);
            }
            foreach (SpriteRenderer NoteColor in Sprites)
            {
                NoteColor.color = new Color(1, 1, 1, 1);    //아무튼 색을 바꾼다;;;
            }
        }
        myGo.transform.SetParent(NumberLine.transform);
        //다시 붙여줘야함.
        myGo.SetActive(false);
        if (DetailPeek() != null)
        {
            if (DetailPeek().LongNoteLength != 0)
            {
                if(LongNoteCor!=null)
                    StopCoroutine(LongNoteCor);

                LongNoteCor = StartCoroutine(LongNoteTimeChecker(DetailPeek(),GameobjectPeek()));
            }

        }

    }
    
    void JustNoteDequeue(int judge,float delta) //judge가 -1이면 롱노트인걸로 하자
    {
        //NoteSpriteChange(judge);
        
        if (judge == 0)
        {
            judgement.Miss(delta);
            NoteSpriteChange(judge, false, GameobjectPeek());
            StartCoroutine(NoteFadeOut(false));
        }
        else if (judge == 1)
        {
            judgement.Good(delta);
            NoteSpriteChange(judge, false, GameobjectPeek());
            StartCoroutine(NoteFadeOut(false));
        }
        else if (judge == 2)
        {
            judgement.Perfect(delta);
            NoteSpriteChange(judge, false, GameobjectPeek());
            StartCoroutine(NoteFadeOut(false));
        }
        else
        {
            //롱노트일 때.....
            StartCoroutine(NoteFadeOut(true));
        }
        //잠시 그 자리에 남으면서. 
    }

    void NoteSpriteChange(int judge,bool isLong,GameObject noteObj)
    {

        NewNote note = DetailPeek();
        char type = note.NoteType;
        if (judge == 0)
        {
            if (type == '5' || type == '8')
            {
                StartCoroutine(NoteSpriteCor(isLong, noteObj, false));

            }       
        }
        else if (judge == 1 || judge==2)
        {
            if(type == '4')
            {
                StartCoroutine(NoteSpriteCor(isLong,noteObj,false));
            }
            else if(type == '6')
            {
                StartCoroutine(NoteSpriteCor(isLong, noteObj, true));
            }
        }
    }

    IEnumerator NoteSpriteCor(bool isLong,GameObject obj, bool threes)
    {
        GameObject noteObj = obj;
        GameObject[] anime = new GameObject[3];
        if (isLong)
        {
            anime[0] = noteObj.transform.GetChild(3).gameObject;
            anime[1] = noteObj.transform.GetChild(4).gameObject;
            if (threes)
            {
                anime[2] = noteObj.transform.GetChild(5).gameObject;
            }

        }
        else
        {
            if (threes)
            {
                for (int i = 0; i < 3; i++)
                    anime[i] = noteObj.transform.GetChild(i).gameObject;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                    anime[i] = noteObj.transform.GetChild(i).gameObject;

            }

        }
        if (threes)
        {
            anime[0].SetActive(false);
            anime[1].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            anime[1].SetActive(false);
            anime[2].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            anime[0].SetActive(true);
            anime[2].SetActive(false);
        }
        else
        {
            anime[0].SetActive(false);
            anime[1].SetActive(true);
            yield return new WaitForSeconds(1f);
            anime[0].SetActive(true);
            anime[1].SetActive(false);
        }
      
    }

    int routeIndex = 0;
    void RoutingNoteDequeue(bool routed)
    {

        if (routed)
        {
            judgement.RouteAdd(routeIndex);
        }
        else
        {
            judgement.RouteAdd(-1);
        }
        routeIndex++;
        StartCoroutine(NoteFadeOut(false));
        NewNoteMaker.RouteChange(routed);
        Debug.Log("라우팅" + NewNoteMaker.NowRoute);
        QueueingEnd(routed);    //라우팅된거 꺼주는거다.

    }

    void RoutingRecall()
    {
        Debug.Log("돌아오기");
        StartCoroutine(NoteFadeOut(false));
        NewNoteMaker.NowRoute = 0;
        for(int i = 1; i < 3; i++)
        {
            DequeueIndex[i] = RoutingIndexList[i - 1][RoutedTimes];
            ActionNoteIndex[i] = ActionIndexList[i - 1][RoutedTimes];
        }
        RoutedTimes++;
    }

    IEnumerator LongNoteTimeChecker(NewNote note,GameObject noteObject)
    {
        //처음꺼 잘 누르면 +1
        //누르자마자 +1
        //longnotetime / 6(0.2초) 하면 댄다.
        //2 + (int)longnotetime/6
        int EndFrame = note.NoteTime + (int)note.LongNoteLength;
        char type = note.NoteType;
        KeyCode keyType = KeyCode.Space;
        while(soundManager.rhythmTool.currentFrame < note.NoteTime)
        {
            yield return null;
        }
        switch (type)
        {
            case '4':
                keyType = KeyCode.A;
                break;
            case '5':
                keyType = KeyCode.S;
                break;
            case '6':
                keyType = KeyCode.D;
                break;
            case '8':
                keyType = KeyCode.W;
                break;
            default:
                keyType = KeyCode.Space;
                break;
        }
        if (MissInStart)
        {
            float wait = 0;
            NoteSpriteChange(0, true, noteObject);
            GameObject Note = noteObject;
            Note.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            SpriteRenderer[] Sprites = new SpriteRenderer[3];
            Sprites = Note.GetComponentsInChildren<SpriteRenderer>();
            //StartCoroutine(NoteSpriteCor(true, Note,false));
            foreach (SpriteRenderer NoteColor in Sprites)
            {
                NoteColor.color = new Color(0.5f, 0.5f, 0.5f);    //아무튼 색을 바꾼다;;;
            }
            judgement.Miss(-1);
            while (soundManager.rhythmTool.currentFrame < EndFrame-3)
            {
                wait += 0.1f;
                yield return new WaitForSeconds(0.1f);
                if (wait > 1f)
                    break;
            }
            foreach (SpriteRenderer NoteColor in Sprites)
            {
                NoteColor.color = new Color(1, 1, 1);    //아무튼 색을 바꾼다;;;
            }
            MissInStart = false;
        }
        else
        {
            NoteSpriteChange(2, true, noteObject);
        }
        while (soundManager.rhythmTool.currentFrame < EndFrame-3)
        {
            //롱노트 사이에 있다면.
            if (Input.GetKey(keyType))
            {
                judgement.Perfect(-1);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                float wait = 0;
                GameObject Note = noteObject;
                Note.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                SpriteRenderer[] Sprites = new SpriteRenderer[3];
                Sprites = Note.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer NoteColor in Sprites)
                {
                    NoteColor.color = new Color(0.5f, 0.5f, 0.5f);    //아무튼 색을 바꾼다;;;
                }
                judgement.Miss(-1);
                while (soundManager.rhythmTool.currentFrame < EndFrame-3)
                {
                    wait += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                    if (wait > 1f)
                        break;
                }
                foreach (SpriteRenderer NoteColor in Sprites)
                {
                    NoteColor.color = new Color(1, 1, 1);    //아무튼 색을 바꾼다;;;
                }
            }
        }
       
        JustNoteDequeue(-1, 0);
    }
}


/*
 * 문제되는거

여러번 분기하면 그 분기했을 떄 어느지점부터 접근해야되는지 모른다. 접근점을 잡아줘야 하는데, 쓰발...
*/


/*
 *    /*    //업데이트 안에 들어가던 내용
            if (!OnRightNote && Mathf.Abs(detail.NoteTime - soundManager.rhythmTool.currentFrame) < JudgeMiss)    //롱노트이고, grey가 아닐 때 판정.
            {
                OnLongNoteNow = true;
                if (!IsGrey)
                {
                    float PushTime = soundManager.rhythmTool.currentFrame - detail.NoteTime;
                    float AbsPushTime = Mathf.Abs(PushTime);
                    //pushtime은 시간 간격. 음수라면 지나간거, 양수라면 아직 안온거.. 친거하고 노트타임하고의 간격. 이걸 토대로 미스 굿 퍼펙 판정
                    //여기서 고려할 건 무엇일까.
                    //현재 grey라면->nothing
                    //롱노트 판정범위 안이라면 -> 판정
                    //현재 grey였다가 풀린 상태라면 -> 판정
                    //롱노트 바로 뒤에 단노트가 나올 때 어떻게 할 수 없을까?
                    //결국에는 롱노트하고 단노트하고 다르게 만들어줘야 할 것 같다.
                    //롱노트 큐하고 단노트큐하고 다르게두면, 롱노트 뒤에 단노트가 올 때 바로 판정이 가능하다
                    //결국에는 동시 판정이 필요해. 롱노트 큐를 다시 만들자. 롱노트
                    //이외에는 모두 nothing. dequeue를 빠르게 해주는게 중요하겠다.

                    //롱노트 판정범위 안 불? : no
                    //그레이엿다가 풀린 상태 : yes.
                    // 이 불값들을 초기화하는게 중요한데.
                    //노트가 생성되자마자 판정선에 닿는지 유무를 만들어야한다.
                    //뒤에 나오는게 단노트인지 롱노트인지 또 따져야해.

                    if (detail.NoteType == myType)
                    {
                        if (!IsGreyToWhite)
                        {
                            if (AbsPushTime < JudgePerfect)
                            {
                                Debug.Log("퍼펙");
                                judgement.Perfect(Mathf.Abs(AbsPushTime - detail.NoteTime));
                                OnRightNote = true;
                                if (!LongNoteComboCoroutine)
                                {
                                    LongNoteComboCoroutine = true;
                                    StartCoroutine(OnLongNote());
                                }
                                //요거 뗄 때 false해줘야해
                                //여기서 판정 다 해주고 콤보 시작을 해줘야돼. 콤보 코루틴은 코루틴에서 눌렸는지 안눌렸는지 판단하고 그레이를 켜줘야해
                                //콤보 코루틴이 롱노트의 지속시간이 남았는가가 될꺼야. 콤보코루틴 이름을 onLongNote로 하자.
                            }
                            else if (AbsPushTime < JudgeGood)
                            {
                                Debug.Log("굿");
                                OnRightNote = true;
                                judgement.Perfect(Mathf.Abs(AbsPushTime - detail.NoteTime));
                                if (!LongNoteComboCoroutine)
                                {
                                    LongNoteComboCoroutine = true;
                                    StartCoroutine(OnLongNote());
                                }
                            }
                            else if (AbsPushTime < JudgeMiss)
                            {
                                Debug.Log("미스");
                                OnRightNote = true;
                                judgement.Miss(Mathf.Abs(AbsPushTime - DetailPeek().NoteTime));
                                MissInStart = true;
                                if (!IsGrey)
                                {
                                    StartCoroutine(LongNoteToGrey());
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("개에바;");
                            OnRightNote = true;
                            if (!LongNoteComboCoroutine)
                            {
                                LongNoteComboCoroutine = true;
                                StartCoroutine(OnLongNote());
                            }
                        }

                    }
                    else
                    {
                        //허공에다 누르면 자꾸 여기뜨거든?
                        Debug.Log("개에바");
                        OnRightNote = false;
                        judgement.Miss(Mathf.Abs(AbsPushTime - DetailPeek().NoteTime));
                        if (!IsGrey)
                        {
                            StartCoroutine(LongNoteToGrey());
                        }
                    }

                }
                else     //그레이노트일때
                {
                    if (DetailPeek().NoteType == myType)
                    {
                        OnRightNote = true;
                        Debug.Log("된거자너");
                    }
                    else
                    {
                        OnRightNote = false;
                    }
                }
                
            }
            else   //중간에 눌렀을 때.및 OnRightNote가 아닐 때.
            {
                if (IsGreyToWhite)
                {
                    Debug.Log("개에바;");
                    OnRightNote = true;
                    if (!LongNoteComboCoroutine)
                    {
                        LongNoteComboCoroutine = true;
                        StartCoroutine(OnLongNote());
                    }


                }
                else if (IsGrey)
                {
                    if (DetailPeek().NoteType == myType)
                    {
                        OnRightNote = true;
                        Debug.Log("된거자너");
                    }
                    else
                    {
                        OnRightNote = false;
                    }
                }
            }
            */
