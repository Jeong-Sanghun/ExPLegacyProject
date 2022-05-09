using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePos : MonoBehaviour {

    public List<Vector2> Position;
    public GameObject[] JointList;
    public GameObject[] RouteList;
    public GameObject Route;
    public GameObject Joint;
    public NewNoteMaker noteMaker;
    public Sprite[] StartEndSprite; //0이 시작, 1이 엔드, 2가 조인트
    public Judgement judgement;
    public int Index;
    public bool ActionStart = false;

    int NoteIndexForLength = 0;
    bool ActionEnd = true;

    // Update is called once per frame
    private void Start()
    {
        JointList = new GameObject[Index];
        RouteList = new GameObject[Index];

        for(int i = 0; i < Index; i++)
        {
            JointList[i] = Instantiate(Joint);
            JointList[i].SetActive(false);

            RouteList[i] = Instantiate(Route);
            RouteList[i].SetActive(false);
        }
    }

    public void MakeActionNote(int NoteIndex)
    {
        NoteIndexForLength = NoteIndex;
        ActionEnd = false;
        StartCoroutine(TimeLimit());
        Position = new List<Vector2>();
        Position = noteMaker.NoteBook.ActionList[NewNoteMaker.NowRoute].MouseNoteList[NoteIndex].VectorList;
        Index = Position.Count;
        judgement.WholeComboAdd(Index);

        for(int i = 0; i < Index; i++)
        {
            JointList[i].SetActive(true);
            JointList[i].transform.position = Position[i];

            RouteList[i].SetActive(true);
        }
        RouteList[0].SetActive(false);
        JointList[0].GetComponent<SpriteRenderer>().sprite = StartEndSprite[0];
        JointList[0].GetComponent<BoxCollider2D>().enabled = true;
        JointList[0].GetComponent<MouseRoute>().enabled = true;
        JointList[0].GetComponent<MouseRoute>().IsStart = true;
        JointList[Index-1].GetComponent<SpriteRenderer>().sprite = StartEndSprite[1];
        JointList[Index - 1].GetComponent<BoxCollider2D>().enabled = true;
        JointList[Index-1].GetComponent<MouseRoute>().enabled = true;
        JointList[Index - 1].GetComponent<MouseRoute>().IsEnd = true;

        /* Route.transform.position = new Vector2((Cubes[0].transform.position.x + Cubes[1].transform.position.x) / 2,
             (Cubes[0].transform.position.y + Cubes[1].transform.position.y) / 2);*/

        float JointAngle = 0;
        if(JointList[1].transform.position.x - JointList[0].transform.position.x>0)
        {
            //0이 1보다 작으면, 즉, 1이 0거보다 오른쪽에 있으면. 오른쪽 방향이라면 원래대로.
            JointAngle = 0;
        }
        else
        {
            JointAngle = 180f;
        }
        JointList[0].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((JointList[1].transform.position.y - JointList[0].transform.position.y) /
                    (JointList[1].transform.position.x - JointList[0].transform.position.x)) * 180.0f / Mathf.PI + JointAngle);

        if (JointList[Index-1].transform.position.x - JointList[Index-2].transform.position.x > 0)
        {
            //0이 1보다 작으면, 즉, 1이 0거보다 오른쪽에 있으면. 오른쪽 방향이라면 원래대로.
            JointAngle = 0;
        }
        else
        {
            JointAngle = 180;
        }

        JointList[Index-1].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((JointList[Index-1].transform.position.y - JointList[Index-2].transform.position.y) /
            (JointList[Index-1].transform.position.x - JointList[Index-2].transform.position.x)) * 180.0f / Mathf.PI + JointAngle);

        for (int i = 1; i < Index; i++)
        {

            float dumy = JointList[i].transform.position.x - JointList[i-1].transform.position.x;
            float angle;
            if (dumy < 0)
            {
                angle = 90f;
            }
            else
            {
                angle = -90f;
            }
            RouteList[i].SetActive(true);
            CapsuleCollider2D RouteCollider = RouteList[i].GetComponent<CapsuleCollider2D>();
            RouteList[i].transform.position = (JointList[i - 1].transform.position + JointList[i].transform.position) / 2;
            RouteList[i].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((JointList[i].transform.position.y - JointList[i - 1].transform.position.y) /
                         (dumy)) * 180.0f / Mathf.PI + angle);
            float magnitude = (JointList[i - 1].transform.position - JointList[i].transform.position).magnitude;
            if(i == Index-1)
            {
                RouteCollider.size = new Vector2(2.0f, magnitude-0.3f);
            }
            else
            {
                RouteCollider.size = new Vector2(2.0f, magnitude + 2);
            }
           
            RouteList[i].transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(7.7f*magnitude, 10);
            /*
            Instantiate(Route, (JointList[i - 1].transform.position + JointList[i].transform.position) / 2,
                Quaternion.Euler(0, 0, Mathf.Atan((JointList[i].transform.position.y - JointList[i - 1].transform.position.y) /
                         (dumy)) * 180.0f / Mathf.PI + angle));*/
        }
    }

    IEnumerator TimeLimit()
    {
        float time = 0;
        float LimitLength = noteMaker.NoteBook.ActionList[NewNoteMaker.NowRoute].MouseNoteList[NoteIndexForLength].Length;
        while (!ActionEnd && time<LimitLength)
        {
            time += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (!ActionEnd)
        {
            Debug.Log("시간때문이야");
            judgement.Miss(-1);
            DestroyActionNote();
        }
    }
    
    public void DestroyActionNote()
    {
        ActionEnd = true;
        MouseRoute.reset();
        StopCoroutine(TimeLimit());
        StartCoroutine(ActionOpacity());
    }

    IEnumerator ActionOpacity()
    {
        float opacity = 1;
        List<SpriteRenderer> mySprites = new List<SpriteRenderer>();
        for(int i = 0; i < Index; i++)
        {
            mySprites.Add(JointList[i].GetComponent<SpriteRenderer>());
            mySprites.Add(RouteList[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
        }
        while (opacity > 0)
        {
            opacity -= 0.05f;
            foreach (SpriteRenderer sprite in mySprites)
            {
                sprite.color = new Color(1, 1, 1, opacity);
            }
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < Index; i++)
        {
            JointList[i].SetActive(false);
            RouteList[i].SetActive(false);
        }
        foreach (SpriteRenderer sprite in mySprites)
        {
            sprite.color = new Color(1, 1, 1, 1);
        }
        JointList[0].GetComponent<SpriteRenderer>().sprite = StartEndSprite[2];
        JointList[0].GetComponent<BoxCollider2D>().enabled = false;
        JointList[0].GetComponent<MouseRoute>().enabled = false;
        JointList[0].GetComponent<MouseRoute>().IsStart = false;
        JointList[Index - 1].GetComponent<SpriteRenderer>().sprite = StartEndSprite[2];
        JointList[Index - 1].GetComponent<BoxCollider2D>().enabled = false;
        JointList[Index - 1].GetComponent<MouseRoute>().enabled = false;
        JointList[Index - 1].GetComponent<MouseRoute>().IsEnd = false;
    }

    //이제 문제는 그건데, 이거 중간에 틀리면 어떻게 될 것인가....
    //아예 오브젝트를 하나 추가해서 마우스콜라이더를 박자.

}
