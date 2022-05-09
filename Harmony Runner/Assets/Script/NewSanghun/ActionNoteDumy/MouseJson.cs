using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseJson : MonoBehaviour {

    public bool ActionStart = false;
    public List<MouseNote> mouseNote;
    GameObject Parents;
    public GameObject Route;
    public GameObject Joint;
    public List<GameObject> RouteList = new List<GameObject>();
    public List<GameObject> JointList = new List<GameObject>();
    int Qindex = 0;
    int index = 0;
    //입력하는거 넣어야돼 아시발..

    private void Start()
    {
        mouseNote = new List<MouseNote>();
        mouseNote.Add(new MouseNote());
        mouseNote[0].VectorList = new List<Vector2>();
        Parents = Instantiate(new GameObject());
        Parents.name = "ActionNoteParents";
        for(int i = 0; i < 15; i++)
        {
            RouteList.Add(Instantiate(Route,Parents.transform));
            JointList.Add(Instantiate(Joint, Parents.transform));
            RouteList[i].SetActive(false);
            JointList[i].SetActive(false);
        }
    }

    private void Update()
    {
        //0이 왼클릭,1이좌클릭인가바.
        if (ActionStart)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Qindex < 15)
                {
                    Vector2 WorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseNote[index].VectorList.Add(WorldPos);
                    mouseNote[index].Length += 1;
                    Connect(WorldPos);
                }
                //이거 렝쓰 식 줘야돼.
            }
        }
    }

    void Connect(Vector2 pos)
    {
        JointList[Qindex].SetActive(true);
        JointList[Qindex].transform.position = pos;

        float JointAngle = 0;
        
 
        if(Qindex != 0)
        {
            if (JointList[Qindex].transform.position.x - JointList[Qindex - 1].transform.position.x > 0)
            {
                //0이 1보다 작으면, 즉, 1이 0거보다 오른쪽에 있으면. 오른쪽 방향이라면 원래대로.
                JointAngle = 0;
            }
            else
            {
                JointAngle = 180f;
            }
            JointList[Qindex - 1].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((JointList[Qindex].transform.position.y - JointList[0].transform.position.y) /
                        (JointList[Qindex].transform.position.x - JointList[Qindex - 1].transform.position.x)) * 180.0f / Mathf.PI + JointAngle);

            float dumy = JointList[Qindex].transform.position.x - JointList[Qindex - 1].transform.position.x;
            float angle;
            if (dumy < 0)
            {
                angle = 90f;
            }
            else
            {
                angle = -90f;
            }
            RouteList[Qindex].SetActive(true);
            RouteList[Qindex].transform.position = (JointList[Qindex - 1].transform.position + JointList[Qindex].transform.position) / 2;
            RouteList[Qindex].transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan((JointList[Qindex].transform.position.y - JointList[Qindex - 1].transform.position.y) /
                         (dumy)) * 180.0f / Mathf.PI + angle);
            float magnitude = (JointList[Qindex - 1].transform.position - JointList[Qindex].transform.position).magnitude;

            RouteList[Qindex].transform.GetChild(0).GetComponent<SpriteRenderer>().size = new Vector2(7.7f * magnitude, 10);

        }
        Qindex++;
    }

    public void mouseNoteReset()
    {
        for (int i = 0; i < 15; i++)
        {
            RouteList[i].SetActive(false);
            JointList[i].SetActive(false);
        }
        mouseNote.Add(new MouseNote());
        index++;
        mouseNote[index].VectorList = new List<Vector2>();
        Qindex = 0;
    }
}
