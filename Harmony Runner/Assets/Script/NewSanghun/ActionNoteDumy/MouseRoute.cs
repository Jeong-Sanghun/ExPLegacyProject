using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRoute : MonoBehaviour {

    public bool IsStart = false;    //시작점에만 부여된다
    public bool IsEnd = false;      //끝점에만 부여된다. 나머지는 false
    static public bool RouteStart = false;
   // static bool OnLayer = false;    //콜라이더를 나가지 않은 상태에서 다른 콜라이더와 만났을 때.
    static bool OnRoute = false;    //방금까지 지나가던 콜라이더를 나갔는지 판별해줘. 그래야 겹ㅊ쳤는지를 볼 수 있으니까
    static Judgement judgemnet;
    static MousePos mousePos;
    static int nowIndex = 0, pauseindex = 0;
    public static bool StaticStart = false;
    static bool StaticEnd = false;
    static bool gameispaused;       // Pause 상태를 받아온다. Pause 일 때 입력받으면 안되니까.

    //만약 나갔으면 콜라이더 set false하자. 그래야 뒤로 못돌아오니까.

    //ㅁㅊ 뒤로가는것도 판정해야돼 개같다진자.
    //OnLayer OnRotueAlone
    //0       0             마우스가 콜라이더 바깥에 있다
    //1       1             unable
    //0       1             콜라이더 하나에 있다
    //1       0             콜라이더 두개에 마우스가 겹쳐있다.
    //exit이 enter보다 먼저 일어나.

    private void Start()
    {
        judgemnet = GameObject.Find("NoteManager").GetComponent<Judgement>();
        mousePos = GameObject.Find("ActionManager").GetComponent<MousePos>();
    }

    private void Update()
    {
        gameispaused = GameObject.Find("Canvas").GetComponent<PauseMenu>().GameIsPaused;
    }

    private void OnMouseDown()
    {
        //시작점에 클릭했을 때.
            if (IsStart && !gameispaused)
            {
                StaticStart = true;
                Debug.Log("StartWell");
                nowIndex++;
                pauseindex++;
                RouteStart = true;  //루트가 시작되었다~ 길을 찾자!
                OnRoute = true;
                mousePos.RouteList[nowIndex].GetComponent<CapsuleCollider2D>().enabled = true;
            }
    }


    private void OnMouseUp()
    {
        if (!gameispaused)
        {
            MouseUpAndExit();
            Debug.Log(gameispaused);
            Debug.Log(IsEnd);
            Debug.Log("UpWell");
        }
    }


    private void OnMouseExit()
    {
        if (!gameispaused)
        {
            MouseUpAndExit();
        }
    }

    private void OnMouseEnter()
    {
        if (!gameispaused)
        {
            if (StaticStart)
            {
                if (!IsStart && !OnRoute && Input.GetMouseButton(0))
                {
                    Debug.Log("EnterWell");
                    OnRoute = true;
                    if (!IsEnd)
                    {
                        nowIndex++;
                        //다음거를 추가한다는걸 넣어야돼.
                        pauseindex++;
                        mousePos.RouteList[nowIndex].GetComponent<CapsuleCollider2D>().enabled = true;

                    }
                }
                if (IsEnd)
                {
                    //static end를 true로 해주자. 이때는 떼도 되는걸로 하자.
                    StaticEnd = true;
                }

            }
            else
                mousePos.RouteList[pauseindex].GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    public static void reset()
    {
        OnRoute = false;
        RouteStart = false;
        StaticStart = false;
        StaticEnd = false;
        nowIndex = 0;
        pauseindex = 0;
    }

    void MouseUpAndExit()
    {
        if (StaticStart)
        {
            OnRoute = false;
            if (StaticEnd)
            {
                mousePos.DestroyActionNote();
            }
            else if (!IsEnd)
            {
                StartCoroutine(JudgeCoroutine());

                if (!IsStart)
                {
                    GetComponent<CapsuleCollider2D>().enabled = false;
                }
            }

            //엔드도 아니고 스타트도 아니면 콜라이더 끈다잖아. 결국 루트이면 콜라이더 끈다잖아.
           
        }
    }

    static IEnumerator JudgeCoroutine()
    {
        yield return new WaitForFixedUpdate();
        if (OnRoute)
        {
            judgemnet.Perfect(-1);
        }
        else
        {
            judgemnet.Miss(-1);
            nowIndex = 0;
            StaticStart = false;
        }
    }

    /*
    private void OnMouseOver()
    {
        //마우스가 위에 있을 떄 실행.
        Debug.Log("실행되고 있어");
    }*/

}
