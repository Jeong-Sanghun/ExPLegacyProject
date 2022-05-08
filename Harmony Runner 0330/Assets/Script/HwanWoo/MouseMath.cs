using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMath : MousePos {

    public Vector2 RealMousePos;
    static public Vector2 MouseNote1, MouseNote2;     // 수선의 발을 내리기 위한 세 점 선언.
    public Vector2 NotePos;        // 보정한 값을 넣어둘 곳.
    public GameObject blink;
    public static bool staticstart;

    private void Start()
    {
        JointList = new GameObject[Index];
        RouteList = new GameObject[Index];

        for (int i = 0; i < Index; i++)
        {
            JointList[i] = Instantiate(Joint);
            JointList[i].SetActive(false);

            RouteList[i] = Instantiate(Route);
            RouteList[i].SetActive(false);
        }
    }

    void Update()
    {
        staticstart = MouseRoute.StaticStart;
    }

    public void judgePos()   
    {
        int ii = 0;     // 어느 루트에 속해있는지 알기 위한 작업. 그래야 어디 두 점을 설정할지 판단하지.

        while (true) 
        {
            if (RouteList[ii].GetComponent<CapsuleCollider2D>().enabled)        // joint의 index 받아오기
                break;
            ii++;
        }
        MouseNote1.x = JointList[ii - 1].transform.position.x; MouseNote1.y = JointList[ii - 1].transform.position.y;           // joint의 두 좌표값 넣어주기.
        MouseNote2.x = JointList[ii].transform.position.x; MouseNote2.y = JointList[ii].transform.position.y;
    }

    public void slope()         // 순수하게 계산하는 함수.
    {
        float a = MouseNote1.x; float b = MouseNote1.y; float c = MouseNote2.x; float d = MouseNote2.y; float i = RealMousePos.x; float j = RealMousePos.y; // 편리함을 위하여 알파벳 변수로 바꿔줌.
        float k = (b - d) / (a - c);    // mousenote1,2를 이용한 직선의 기울기
        NotePos.x = ((c * k) - d + (i / k) + j) / k + (1 / k);      // 보정된 x 값.
        NotePos.y = k * (NotePos.x - c) + d;        // 보정된 y 값.
        blink.transform.position = new Vector2(NotePos.x, NotePos.y);       // 좌표 보정해주기.
    }

    public void cal()
    {
        if (staticstart)
        {
            judgePos();
            slope();
        }

    }
    public void blinkstart()
    {
        if(staticstart)
            blink.SetActive(true);
    }

    public void blinkend()
    {
        blink.SetActive(false);
    }
    }