using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteClass : MonoBehaviour
{
    float CommonK = 7.66f/30f;
    float CommonBody = 765f/30f;
    public PassScript passScript;
    public NewSoundManager soundManager;
    protected float Velocity = 1;
    protected bool IsEditing = false;
    protected Vector2 EditorNotePos = new Vector2(0,0);
    //13.1

    protected GameObject myNote;

    int NoteChapter;

    public GameObject NumberLine;
    public GameObject PlayerGrid;

    public GameObject[] UpArrowObject;
    public GameObject[] UpLongArrowObject;

    public GameObject[] LeftArrowObject;
    public GameObject[] LeftLongArrowObject;

    public GameObject[] RightArrowObject;
    public GameObject[] RightLongArrowObject;

    public GameObject[] DownArrowObject;
    public GameObject[] DownLongArrowObject;
    public GameObject ActionNote;
    public GameObject YellowLine;
    public GameObject RouteRecall;

    public List<GameObject> UpArrowObjectPool;
    public List<GameObject> UpLongArrowObjectPool;

    public List<GameObject> LeftArrowObjectPool;
    public List<GameObject> LeftLongArrowObjectPool;

    public List<GameObject> RightArrowObjectPool;
    public List<GameObject> RightLongArrowObjectPool;

    public List<GameObject> DownArrowObjectPool;
    public List<GameObject> DownLongArrowObjectPool;

    public List<GameObject> ActionNotePool;
    public List<GameObject> RouteNotePool;
    public List<GameObject> RecallNotePool;

    public List<Transform> NoteTransformList = new List<Transform>();
    public List<Transform> ActiveGameNoteList = new List<Transform>();
    //드래그해서 넣을라그럴때 이게 쓰여버린다. 귀찮쓰.

    public virtual void EditOrPlayStart(bool isPlaying,int chapter)
    {
        if (!isPlaying)
        {
            chapter = 1;
        }
        NoteChapter = chapter;
        UpArrowObjectPool.Clear();
        DownArrowObjectPool.Clear();
        RightArrowObjectPool.Clear();
        LeftArrowObjectPool.Clear();

        UpLongArrowObjectPool.Clear();
        DownLongArrowObjectPool.Clear();
        RightLongArrowObjectPool.Clear();
        LeftLongArrowObjectPool.Clear();

        ActionNotePool.Clear();
        RouteNotePool.Clear();
        RecallNotePool.Clear();

        YellowLine = Instantiate(YellowLine, NumberLine.transform);
        RouteRecall = Instantiate(RouteRecall, NumberLine.transform);


        for(int i = 0; i < 10; i++)
        {
            RecallNotePool.Add(Instantiate(RouteRecall, NumberLine.transform));
            RecallNotePool[i].SetActive(false);
            NoteTransformList.Add(RecallNotePool[i].transform);

            RouteNotePool.Add(Instantiate(YellowLine, NumberLine.transform));
            RouteNotePool[i].SetActive(false);
            NoteTransformList.Add(RouteNotePool[i].transform);

            ActionNotePool.Add(Instantiate(ActionNote, NumberLine.transform));
            ActionNotePool[i].SetActive(false);
            NoteTransformList.Add(ActionNotePool[i].transform);

        }


        for (int i = 0; i < 40; i++)
        {

            UpArrowObjectPool.Add(Instantiate(UpArrowObject[chapter - 1], NumberLine.transform));
            UpArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(UpArrowObjectPool[i].transform);

            DownArrowObjectPool.Add(Instantiate(DownArrowObject[chapter - 1], NumberLine.transform));
            DownArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(DownArrowObjectPool[i].transform);

            RightArrowObjectPool.Add(Instantiate(RightArrowObject[chapter - 1], NumberLine.transform));
            RightArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(RightArrowObjectPool[i].transform);

            LeftArrowObjectPool.Add(Instantiate(LeftArrowObject[chapter - 1], NumberLine.transform));
            LeftArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(LeftArrowObjectPool[i].transform);

            UpLongArrowObjectPool.Add(Instantiate(UpLongArrowObject[chapter - 1], NumberLine.transform));
            UpLongArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(UpLongArrowObjectPool[i].transform);

            DownLongArrowObjectPool.Add(Instantiate(DownLongArrowObject[chapter - 1], NumberLine.transform));
            DownLongArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(DownLongArrowObjectPool[i].transform);

            RightLongArrowObjectPool.Add(Instantiate(RightLongArrowObject[chapter - 1], NumberLine.transform));
            RightLongArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(RightLongArrowObjectPool[i].transform);

            LeftLongArrowObjectPool.Add(Instantiate(LeftLongArrowObject[chapter - 1], NumberLine.transform));
            LeftLongArrowObjectPool[i].SetActive(false);
            NoteTransformList.Add(LeftLongArrowObjectPool[i].transform);



            //비운다음에 새로 만들어주고 
        }
    }

    public float BPM;

    protected void NoteDestroy()
    {
        UpArrowObjectPool.Clear();
        DownArrowObjectPool.Clear();
        RightArrowObjectPool.Clear();
        LeftArrowObjectPool.Clear();

        UpLongArrowObjectPool.Clear();
        DownLongArrowObjectPool.Clear();
        RightLongArrowObjectPool.Clear();
        LeftLongArrowObjectPool.Clear();
    }

    private void MakeEditLongNote(ref GameObject Note, float myTime, float LongNoteLength)
    {
        Transform myBody;
        Transform myTail;

        myNote = Note;
        Note.SetActive(true);   //노트 찾아서 켜주고,
        Note.transform.localPosition = EditorNotePos;   //지정한 포지션으로 가게한다.

        myBody = Note.transform.GetChild(0);    //바디는 차일드중에 0번째거
        myTail = Note.transform.GetChild(1);    //테일은 1번째꺼
        myTail.transform.SetParent(Note.transform.parent);   //테일의 패런츠는, 테일의 패런츠의 패런츠, 즉, 수직선이다.
        myTail.localPosition = new Vector2(myTime + LongNoteLength, -0.005f);   //테일의 포지션은 수직선에서 프레임의 끝.
        myTail.transform.SetParent(Note.transform);                              //다시 테일은 원래자리로 돌아가고.
        myTail.SetSiblingIndex(1);


        //바디도 똑같은 방식으로 해줄 필요는 없다. 중앙점으로만 잡으면 된다
        myBody.localPosition = new Vector2(4.5f, EditorNotePos.y);
        //myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.009f,1,0);
        //myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.009f, 1, 0);
        myBody.GetComponent<SpriteRenderer>().size = new Vector2((myTail.transform.localPosition.x), 8.29f);

        /*
        BoxCollider2D[] ColliderArray = new BoxCollider2D[2];
        ColliderArray[0] = myBody.GetComponent<BoxCollider2D>();
        ColliderArray[1] = myTail.GetComponent<BoxCollider2D>();

        
        if(!ColliderArray[0].IsTouching(ColliderArray[1]))
        {
            StartCoroutine(BodyScale(myBody, ColliderArray[0], ColliderArray[1]));
        }
        //가장 문제되는게 이거다. 바디 스케일.이걸 코루틴으로 해야하긴 해야하는데,*/
    }
    /*
    protected IEnumerator BodyScale(Transform body, BoxCollider2D one, BoxCollider2D two)
    {
        while (!one.IsTouching(two))
        {
            body.localScale = new Vector3(body.localScale.x + 500, 1, 0);
            yield return new WaitForEndOfFrame();
        }
        body.localScale = new Vector3(body.localScale.x + 100, 1, 0);
        // 식으로 구하기  0.005 Debug.Log((one.transform.localPosition.x - two.transform.localPosition.x)/body.localScale.x);
    }*/

    private void MakeGameLongNote(GameObject Note, float LongNoteLength)
    {
        Transform myBody;
        Transform myTail;

        Note.SetActive(true);
        Note.transform.localPosition = EditorNotePos;
        myBody = Note.transform.GetChild(0);    //바디는 차일드중에 0번째거
        myTail = Note.transform.GetChild(1);    //테일은 1번째꺼
        myTail.transform.SetParent(Note.transform.parent);   //테일의 패런츠는, 테일의 패런츠의 패런츠, 즉, 수직선이다.
        myTail.localPosition = new Vector2(EditorNotePos.x + LongNoteLength, -0.005f);   //테일의 포지션은 수직선에서 프레임의 끝.
        myTail.transform.SetParent(Note.transform);                              //다시 테일은 원래자리로 돌아가고.
        myTail.SetSiblingIndex(1);


        //바디도 똑같은 방식으로 해줄 필요는 없다. 중앙점으로만 잡으면 된다
        // myBody.position = new Vector2((myTail.position.x + Note.transform.position.x) / 2, myBody.position.y);
        myBody.localPosition = new Vector2(4.5f, EditorNotePos.y);
        //myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.009f,1,0);
        //myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.009f, 1, 0);
        myBody.GetComponent<SpriteRenderer>().size = new Vector2((myTail.transform.localPosition.x),8.29f);

        /*
        myBody.localPosition = new Vector2(4.63f + CommonK * LongNoteLength * Velocity * 0.5f, -0.005f);
        myTail.localPosition = new Vector2(CommonK * LongNoteLength * Velocity, -0.005f);
        myBody.localScale = new Vector3(CommonBody * LongNoteLength * Velocity, 1, 0);
        myBody.localScale = new Vector3((myTail.localPosition.x - myBody.localPosition.x) / 0.0047f, 1, 1);
        */
        /*
        Bounds[] BoundArray = new Bounds[2];
        BoundArray[0] = myBody.GetComponent<SpriteRenderer>().bounds;
        BoundArray[1] = myTail.GetComponent<SpriteRenderer>().bounds;
        if (!BoundArray[0].Intersects(BoundArray[1]))
        {
            //만약 바디하고 테일하고 크기가 안맞으면, 맞을때까지 늘려준다

            myBody.localScale = new Vector3(myBody.localScale.x + 300, 1, 0);
        }*/
        ActiveGameNoteList.Add(Note.transform);
        passScript.ObjectEnqueue(Note);
    }

    private void NoteToParent(ref GameObject Note, float myTime)
    {
       Note.transform.SetParent(NumberLine.transform);
        Note.transform.localPosition = new Vector2(myTime, 0);
        NoteTransformList.Add(Note.transform);
    }

    public virtual void EditNoteGenerator(float myTime, char myType,float LongNoteLength,bool adjustment, out GameObject outNote)
    {
        EditorNotePos = new Vector2(myTime, 0);
        if (LongNoteLength == 0)
        {
            //단노트일때
            if (myType == '4')
            {
                foreach (GameObject Note in LeftArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        myNote = Note;
                        Note.SetActive(true);
                        Note.transform.localPosition = EditorNotePos;
                        outNote = Note;
                        return;
                    }
                }
                GameObject Dumy = Instantiate(LeftArrowObject[NoteChapter-1]);
                myNote = Dumy;
                LeftArrowObjectPool.Add(Dumy);
                myNote.transform.localPosition = EditorNotePos;
                outNote = myNote;
                 NoteToParent(ref myNote,myTime);


            }
            else if (myType == '5')
            {
                foreach (GameObject Note in DownArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        myNote = Note;
                        Note.SetActive(true);
                        Note.transform.localPosition = EditorNotePos;
                        outNote = Note;
                        return;
                    }
                }
                GameObject Dumy = Instantiate(DownArrowObject[NoteChapter - 1]);
            
                myNote = Dumy;
                DownArrowObjectPool.Add(Dumy);
                myNote.transform.localPosition = EditorNotePos;
                outNote = myNote;
                 NoteToParent(ref myNote,myTime);

            }
            else if (myType == '6')
            {
                foreach (GameObject Note in RightArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        myNote = Note;
                        Note.SetActive(true);
                            Note.transform.localPosition = EditorNotePos;
                         
                        outNote = Note;
                        return;
                    }
                }
                GameObject Dumy = Instantiate(RightArrowObject[NoteChapter - 1]);
                myNote = Dumy;
                RightArrowObjectPool.Add(Dumy);
                myNote.transform.localPosition = EditorNotePos;
                outNote = myNote;
                 NoteToParent(ref myNote,myTime);
            }
            else if (myType == '8')
            {
                foreach (GameObject Note in UpArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        myNote = Note;
                        Note.SetActive(true);
                            Note.transform.localPosition = EditorNotePos;
                        outNote = Note;
                        return;
                    }
                }
                GameObject Dumy = Instantiate(UpArrowObject[NoteChapter - 1]);
                myNote = Dumy;
                UpArrowObjectPool.Add(Dumy);
                myNote.transform.localPosition = EditorNotePos;
                outNote = myNote;
                 NoteToParent(ref myNote,myTime);

            }
            else if (myType == '2'){
                GameObject Dumy = Instantiate(YellowLine);
                Dumy.transform.localPosition = EditorNotePos;
                NoteToParent(ref Dumy, myTime);
            }
            else if (myType == '1')
            {
                GameObject Dumy = Instantiate(RouteRecall);
                Dumy.transform.localPosition = EditorNotePos;
                NoteToParent(ref Dumy, myTime);
            }
        }
        else
        {
            //롱노트일때
            GameObject Dumy;
            if (myType == '4')
            {
                foreach (GameObject Note in LeftLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        Dumy = Note;
                        MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                        outNote = Note;
                        return;
                    }
                }
                    Dumy = Instantiate(LeftLongArrowObject[NoteChapter - 1]);
                Dumy.transform.SetParent(NumberLine.transform);
                LeftLongArrowObjectPool.Add(Dumy);
                MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                outNote = Dumy;

            }
            else if (myType == '5')
            {
                foreach (GameObject Note in DownLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        Dumy = Note;
                        MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                        outNote = Note;
                        return;
                    }
                }

                    Dumy = Instantiate(DownLongArrowObject[NoteChapter - 1]);
                Dumy.transform.SetParent(NumberLine.transform);
                DownLongArrowObjectPool.Add(Dumy);
                MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                outNote = Dumy;



            }
            else if (myType == '6')
            {
                foreach (GameObject Note in RightLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        Dumy = Note;
                        MakeEditLongNote(ref Dumy, myTime, LongNoteLength);

                        outNote = Note;
                        return;
                    }
                }
 
                    Dumy = Instantiate(RightLongArrowObject[NoteChapter - 1]);
                Dumy.transform.SetParent(NumberLine.transform);
                RightLongArrowObjectPool.Add(Dumy);
                MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                outNote = Dumy;

            }
            else if (myType == '8')
            {
                foreach (GameObject Note in UpLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        Dumy = Note;
                        MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                        outNote = Note;
                        return;
                    }
                }

                    Dumy = Instantiate(UpLongArrowObject[NoteChapter - 1]);
                Dumy.transform.SetParent(NumberLine.transform);
                UpLongArrowObjectPool.Add(Dumy);
                    MakeEditLongNote(ref Dumy, myTime, LongNoteLength);
                outNote = Dumy;


            }

        }
        if (myType == '0')
        {
            foreach (GameObject Note in ActionNotePool)
            {
                if (!Note.activeSelf)
                {
                    myNote = Note;
                    Note.SetActive(true);
                    Note.transform.localPosition = EditorNotePos;
                    outNote = Note;
                    return;
                }
            }
            GameObject Dumy = Instantiate(ActionNote);
            myNote = Dumy;
            LeftArrowObjectPool.Add(Dumy);
            myNote.transform.localPosition = EditorNotePos;
            outNote = myNote;
            NoteToParent(ref myNote, myTime);
        }
        outNote = myNote;
    }



    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //
    //

    protected void NoteGenerator(char myType, float LongNoteLength,float myTime)
    {
        EditorNotePos = new Vector2(myTime, 0);
        if (LongNoteLength == 0)
        {

            //단노트일때
            if (myType == '4')
            {
                foreach (GameObject Note in LeftArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                        Note.SetActive(true);
                        /*//옛날코드
                            Note.transform.position = new Vector2(20, -4);
                        Note.GetComponent<Rigidbody2D>().velocity = new Vector2(-Velocity, 0);
                         */
                        Note.transform.localPosition = EditorNotePos;
                        passScript.ObjectEnqueue(Note);
                        ActiveGameNoteList.Add(Note.transform);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(LeftArrowObject[NoteChapter - 1], NumberLine.transform);
                LeftArrowObjectPool.Add(Dumy);
                    passScript.ObjectEnqueue(Dumy);
                Dumy.transform.localPosition = EditorNotePos;
                ActiveGameNoteList.Add(Dumy.transform);

            }
            else if (myType == '5')
            {
                foreach (GameObject Note in DownArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                         
                        Note.SetActive(true);
                        Note.transform.localPosition = EditorNotePos;
                        passScript.ObjectEnqueue(Note);
                        ActiveGameNoteList.Add(Note.transform);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(DownArrowObject[NoteChapter - 1], NumberLine.transform);
                DownArrowObjectPool.Add(Dumy);
                    passScript.ObjectEnqueue(Dumy);
                Dumy.transform.localPosition = EditorNotePos;
                ActiveGameNoteList.Add(Dumy.transform);

            }
            else if (myType == '6')
            {
                foreach (GameObject Note in RightArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                         
                        Note.SetActive(true);
                        Note.transform.localPosition = EditorNotePos;
                        passScript.ObjectEnqueue(Note);
                        ActiveGameNoteList.Add(Note.transform);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(RightArrowObject[NoteChapter - 1], NumberLine.transform);
                RightArrowObjectPool.Add(Dumy);
                passScript.ObjectEnqueue(Dumy);
                Dumy.transform.localPosition = EditorNotePos;
                ActiveGameNoteList.Add(Dumy.transform);
            }
            else if (myType == '8')
            {
                foreach (GameObject Note in UpArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {
                         
                        Note.SetActive(true);
                        Note.transform.localPosition = EditorNotePos;
                        passScript.ObjectEnqueue(Note);
                        ActiveGameNoteList.Add(Note.transform);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(UpArrowObject[NoteChapter - 1], NumberLine.transform);
                UpArrowObjectPool.Add(Dumy);
                    passScript.ObjectEnqueue(Dumy);
                Dumy.transform.localPosition = EditorNotePos;
                ActiveGameNoteList.Add(Dumy.transform);
            }
        }
        else
        {
            //롱노트일때

            if (myType == '4')
            {
                foreach (GameObject Note in LeftLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {

                        MakeGameLongNote(Note, LongNoteLength);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(LeftLongArrowObject[NoteChapter - 1], NumberLine.transform);
                 
                LeftLongArrowObjectPool.Add(Dumy);
                MakeGameLongNote(Dumy, LongNoteLength);

            }
            else if (myType == '5')
            {
                foreach (GameObject Note in DownLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {

                        MakeGameLongNote(Note, LongNoteLength);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(DownLongArrowObject[NoteChapter - 1], NumberLine.transform);
                 
                DownLongArrowObjectPool.Add(Dumy);
                MakeGameLongNote(Dumy, LongNoteLength);

            }
            else if (myType == '6')
            {
                foreach (GameObject Note in RightLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {

                        MakeGameLongNote(Note, LongNoteLength);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(RightLongArrowObject[NoteChapter - 1], NumberLine.transform);
                 
                RightLongArrowObjectPool.Add(Dumy);
                MakeGameLongNote(Dumy, LongNoteLength);

            }
            else if (myType == '8')
            {
                foreach (GameObject Note in UpLongArrowObjectPool)
                {
                    if (!Note.activeSelf)
                    {

                        MakeGameLongNote(Note, LongNoteLength);
                        return;
                    }
                }
                GameObject Dumy = Instantiate(UpLongArrowObject[NoteChapter - 1], NumberLine.transform);
                UpLongArrowObjectPool.Add(Dumy);
                MakeGameLongNote(Dumy, LongNoteLength);

            }

        }
        if(myType == '0')
        {
            foreach (GameObject Note in ActionNotePool)
            {
                if (!Note.activeSelf)
                {
                    Note.SetActive(true);
                    Note.transform.localPosition = EditorNotePos;
                    passScript.ObjectEnqueue(Note);
                    ActiveGameNoteList.Add(Note.transform);
                    return;
                }
            }
            GameObject Dumy = Instantiate(ActionNote, NumberLine.transform);
            ActionNotePool.Add(Dumy);
            passScript.ObjectEnqueue(Dumy);
            Dumy.transform.localPosition = EditorNotePos;
            ActiveGameNoteList.Add(Dumy.transform);
        }
        if (myType == '2')
        {
            //루트시작
            foreach (GameObject Note in RouteNotePool)
            {
                if (!Note.activeSelf)
                {
                    Note.SetActive(true);
                    /*//옛날코드
                        Note.transform.position = new Vector2(20, -4);
                    Note.GetComponent<Rigidbody2D>().velocity = new Vector2(-Velocity, 0);
                     */
                    Note.transform.localPosition = EditorNotePos;
                    passScript.ObjectEnqueue(Note);
                    ActiveGameNoteList.Add(Note.transform);
                    return;
                }
            }
            GameObject Dumy = Instantiate(YellowLine, NumberLine.transform);
            RouteNotePool.Add(Dumy);
            passScript.ObjectEnqueue(Dumy);
            Dumy.transform.localPosition = EditorNotePos;
            ActiveGameNoteList.Add(Dumy.transform);
        }
        if (myType == '1')
        {
            foreach (GameObject Note in RecallNotePool)
            {
                if (!Note.activeSelf)
                {
                    Note.SetActive(true);
                    /*//옛날코드
                        Note.transform.position = new Vector2(20, -4);
                    Note.GetComponent<Rigidbody2D>().velocity = new Vector2(-Velocity, 0);
                     */
                    Note.transform.localPosition = EditorNotePos;
                    passScript.ObjectEnqueue(Note);
                    ActiveGameNoteList.Add(Note.transform);
                    return;
                }
            }
            GameObject Dumy = Instantiate(RouteRecall, NumberLine.transform);
            RecallNotePool.Add(Dumy);
            passScript.ObjectEnqueue(Dumy);
            Dumy.transform.localPosition = EditorNotePos;
            ActiveGameNoteList.Add(Dumy.transform);
        }


    }



}



//7초일 때 538.455 다 로컬스케일에다가 테일기준.     7.6922142857142857142857142857143
//4초일 때 306.146                                  7.6527999918240954128065325477652
//1초일 때 76.92303                                 7.692303
//velocity = 10
//그냥 이미지 스프라이터 박아넣자...안되겠따...

//1초에는 로컬이 75.3845
//4초면 304.6155