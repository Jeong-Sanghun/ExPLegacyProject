using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDrag : MonoBehaviour {

    //패널에서 가져오는거 작업함. 19.01.10
    //기본 로직
    /*
     * 1.노트를 눌러서 당겨온다
     * 2.수직선위에 놓으면 jsonManager로 수직선 내의 localposition좌표를 보내준다(프레임값을 보낸다)
     * 3.수직선에 parent로 달아준다
     * 19.01.10
     */

    SpriteRenderer noteSprite;  //마우스 올릴때 색바뀔라고 스프라이트 만들었어
    int myTime = 0;
    bool KeyDown = false;
    bool Attached = false;
    bool BodyAttached = false;
    bool TailAttached = false;
    bool PanelAttached = false;
    GameObject NowNote;        //현재 내가 들고있는 오브젝트
    public NewJsonManager jsonManager;  //제이슨 참조해서 리스트를 바꿔줘야함.
    public NoteMakerUIManager uiManager;    //노트 추가되는거를 uiManager로 보내줘야함. 스파게티느낌난다 시발
    GameObject NumberLine;                  //수직선에 노트를 달아줘야해.
    char PanelType;

    private void Start()
    {
        //초기화
        //스타트에다가 절 대 로 함수 시작하지 말 것. 절 대 로. 19.01.10
        noteSprite = GetComponent<SpriteRenderer>();    
        NumberLine = GameObject.Find("NumberLine");
        uiManager = GameObject.Find("NoteMakerUIManager").GetComponent<NoteMakerUIManager>();
        Debug.Log(gameObject.name);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !KeyDown)
        {
            //마우스 클릭했을 때 애기들이 있다면   
            Vector2 myRay;
            myRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D myHit  = Physics2D.Raycast(myRay, myRay);

            if (myHit)
            {
                Debug.Log(myHit.transform.name);
                if (myHit.transform.CompareTag("JustNote") || myHit.transform.CompareTag("LongNote") || myHit.transform.CompareTag("RouteNote"))
                {
                    //노트, 롱노트머리, 테일
                    Attached = true;   
                    NowNote = myHit.transform.gameObject;
                    myTime = (int)NowNote.transform.localPosition.x;
                    //맞으면 붙여준다.
                }
                else if(myHit.transform.CompareTag("Body"))
                {
                    BodyAttached = true;
                    Attached = true;
                    NowNote = myHit.transform.parent.gameObject;
                    myTime = (int)NowNote.transform.localPosition.x;
                    Debug.Log(NowNote.name);
                }
                else if (myHit.transform.CompareTag("Tail"))
                {
                    Attached = true;
                    TailAttached = true;
                    NowNote = myHit.transform.gameObject;
                    myTime = (int)NowNote.transform.parent.localPosition.x;
                    Debug.Log(NowNote.name);

                }
                else if (myHit.transform.CompareTag("PanelNote"))
                {
                    Attached = true;
                    PanelAttached = true;
                    NowNote = myHit.transform.gameObject;
                    MakePanelNote(); //여기서 새 노트 만들어서 따준다.
                    myTime = -1;

                }
            }
        }

        if (Input.GetMouseButtonDown(1) && !KeyDown)
        {
            //우클릭하면 삭제해준다.
            Vector2 myRay;
            myRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D myHit = Physics2D.Raycast(myRay, myRay);
            if (myHit)
            {
                if (myHit.transform.CompareTag("JustNote") || myHit.transform.CompareTag("LongNote"))
                {
                    
                    NowNote = myHit.transform.gameObject;
                    myTime = (int)NowNote.transform.localPosition.x;
                    //우클릭하면 삭제인데, 만약 똑같은 놈들이 있다면 어떻게 할까.

                    for(int i =0;i< jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList.Count; i++)
                    {
                        NewNote note = jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList[i];
                        if (note.EditNote == NowNote)
                        {
                            jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList.Remove(note);
                            NowNote.SetActive(false);
                            uiManager.removeNotes();
                        }
                    }

                }
                else if (myHit.transform.CompareTag("Body") || myHit.transform.CompareTag("Tail"))
                {

                    NowNote = myHit.transform.parent.gameObject;
                    myTime = (int)NowNote.transform.localPosition.x;

                    //우클릭하면 삭제인데, 만약 똑같은 놈들이 있다면 어떻게 할까.

                    for (int i = 0; i < jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList.Count; i++)
                    {
                        NewNote note = jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList[i];
                        if (note.EditNote == NowNote)
                        {
                            jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList.Remove(note);
                            NowNote.SetActive(false);
                            uiManager.removeNotes();
                        }
                    }
                }
                else if (myHit.transform.CompareTag("RouteNote"))
                {
                    NowNote = myHit.transform.gameObject;
                    myTime = (int)NowNote.transform.localPosition.x;
                    NowNote.SetActive(false);
                    bool onEnd = false;
                    Debug.Log("루트노트");
                    //우클릭하면 삭제인데, 만약 똑같은 놈들이 있다면 어떻게 할까.
                    if (myHit.transform.name.Contains("Recall"))
                        onEnd = true;

                    for (int i = 0; i < jsonManager.myNotes.NoteList[0].NewNoteList.Count; i++)
                    {
                        NewNote note = jsonManager.myNotes.NoteList[0].NewNoteList[i];
                        if (note.EditNote == NowNote)
                        {
                            NewNote startNote;
                            NewNote endNote;
                            if (onEnd)
                            {
                                startNote = jsonManager.myNotes.NoteList[0].NewNoteList[i - 1];
                                endNote = note;
                            }
                            else
                            {
                                startNote = note;
                                endNote = jsonManager.myNotes.NoteList[0].NewNoteList[i + 1];
                            }
                            int[] counts = new int[2];
                            counts[0] = startNote.NoteTime;
                            counts[1] = endNote.NoteTime;
                            int[] startIndex = new int[2];
                            int[] endIndex = new int[2];

                            for(int j = 0; j < 2; j++)
                            {
                                startIndex[j] = 0;
                                endIndex[j] = 0;
                                //분기 안에서 시작과 끝을 찾아낸다. 분기1은 startIndex[0], endIndex[0]
                                //시작인덱스와 끝 인덱스를 찾으면, 거기서 거기까지 모든 노트를 지운다.
                                for (int k = 0; k < jsonManager.myNotes.NoteList[j+1].NewNoteList.Count; k++)
                                {
                                    NewNote route1 = jsonManager.myNotes.NoteList[j+1].NewNoteList[k];
                                    if (route1.NoteTime > counts[0])
                                    {
                                        startIndex[j] = k;
                                        break;
                                    }
                                }

                                for (int k = jsonManager.myNotes.NoteList[j+1].NewNoteList.Count - 1; k >= 0; k--)
                                {
                                    NewNote route1 = jsonManager.myNotes.NoteList[j+1].NewNoteList[k];
                                    if (route1.NoteTime < counts[1])
                                    {
                                        endIndex[j] = k;
                                        break;
                                    }
                                }

                                for(int k = startIndex[j]; k < endIndex[j] + 1; k++)
                                {
                                    NewNote route1 = jsonManager.myNotes.NoteList[j + 1].NewNoteList[k];
                                    route1.EditNote.SetActive(false);
                                    jsonManager.myNotes.NoteList[j + 1].NewNoteList.Remove(route1);
                                    uiManager.removeNotes();
                                }
                            }
                            jsonManager.myNotes.NoteList[0].NewNoteList.Remove(startNote);
                            jsonManager.myNotes.NoteList[0].NewNoteList.Remove(endNote);
                            uiManager.removeNotes();
                        }
                    }
                }

            }
        }

        if (Attached)
        {
            NowNote.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        }

        if (Input.GetMouseButtonUp(0) && Attached)
        {
            //손 뗄때
            KeyDown = false;
            Attached = false;
            int nowTime = 0;
            if (BodyAttached)
            {
                BodyAttached = false;
                nowTime = (int)NowNote.transform.localPosition.x;
                NowNote.transform.localPosition = new Vector2((int)NowNote.transform.localPosition.x, 0);

                foreach (NewNote note in jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList)
                {
                    if (note.EditNote == NowNote)
                    {
                        note.NoteTime = nowTime;
                        break;
                    }
                }


                //여기서 나우노트는 헤드야
            }
            else if (TailAttached)
            {
                GameObject HeadNote = NowNote.transform.parent.gameObject;
                //nowTime = (int)HeadNote.transform.localPosition.x;
                TailAttached = false;
                //여기서 테일이 넘버라인것이 된다.

                NowNote.transform.SetParent(NumberLine.transform);
                int TailTime = (int)NowNote.transform.localPosition.x;
               // NowNote.transform.localPosition 
                NowNote.transform.SetParent(HeadNote.transform);
                NowNote.transform.SetSiblingIndex(1);
                foreach (NewNote note in jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList)
                {
                    if (note.EditNote == HeadNote)
                    {
                        int LongNoteLength = TailTime - myTime;
                        if (LongNoteLength < 9)
                        {
                            LongNoteLength = 9;
                        }
                        note.LongNoteLength = LongNoteLength;
                        MakeEditLongNote(HeadNote, LongNoteLength);
                        break;
                    }
                }

                //여기서 나우노트는 테일이야. 나우타임은 longnotelength바꾸는데 쓸꺼야.
            }
            else if (PanelAttached)
            {
                PanelAttached = false;
                NowNote.transform.SetParent(NumberLine.transform);
                NowNote.transform.localPosition = new Vector2((int)NowNote.transform.localPosition.x, 0);
                jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList.Add(new NewNote((int)NowNote.transform.localPosition.x, PanelType, 0,ref NowNote));
                jsonManager.NoteTransformList.Add(NowNote.transform);
            }
            else
            {
                //여기서 나우노트는 롱노트헤드나 단노트야.
                nowTime = (int)NowNote.transform.localPosition.x;
                NowNote.transform.localPosition = new Vector2((int)NowNote.transform.localPosition.x, 0);
                foreach (NewNote note in jsonManager.myNotes.NoteList[jsonManager.NowRoute].NewNoteList)
                {
                    if (note.EditNote == NowNote)
                    {
                        note.NoteTime = nowTime;
                        break;
                    }
                }
            }
           

            NowNote = null;
        }


        //down up으로 하자
    }

    private void MakePanelNote()
    {
        //맘편하게 instantiate해주자.
        if (NowNote.name.CompareTo("UpNoteObject")==0)
        {
            NowNote = Instantiate(jsonManager.UpArrowObject[jsonManager.Chapter-1]);
            PanelType = '8';
        }
        else if (NowNote.name.CompareTo("DownNoteObject") == 0)
        {
            NowNote = Instantiate(jsonManager.DownArrowObject[jsonManager.Chapter - 1]);
            PanelType = '5';

        }
        else if (NowNote.name.CompareTo("LeftNoteObject") == 0)
        {
            NowNote = Instantiate(jsonManager.LeftArrowObject[jsonManager.Chapter - 1]);
            PanelType = '4';

        }
        else if (NowNote.name.CompareTo("RightNoteObject") == 0)
        {
            NowNote = Instantiate(jsonManager.RightArrowObject[jsonManager.Chapter - 1]);
            PanelType = '6';

        }
    }


    
    private void MakeEditLongNote(GameObject Note, float LongNoteLength)
    {
        Transform myBody;
        Transform myTail;

        myBody = Note.transform.GetChild(0);    //바디는 차일드중에 0번째거
        myTail = Note.transform.GetChild(1);    //테일은 1번째꺼
        myTail.transform.SetParent(NumberLine.transform);   //테일의 패런츠는, 테일의 패런츠의 패런츠, 즉, 수직선이다.
        myTail.localPosition = new Vector2(Note.transform.localPosition.x + LongNoteLength, -0.005f);   //테일의 포지션은 수직선에서 프레임의 끝.
        myTail.transform.SetParent(Note.transform);                              //다시 테일은 원래자리로 돌아가고.
        myTail.SetSiblingIndex(1);


        //바디도 똑같은 방식으로 해줄 필요는 없다. 중앙점으로만 잡으면 된다
        myBody.position = new Vector2((myTail.position.x + Note.transform.position.x) / 2, myBody.position.y);
        myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.0047f, 1, 0);
    }
}
