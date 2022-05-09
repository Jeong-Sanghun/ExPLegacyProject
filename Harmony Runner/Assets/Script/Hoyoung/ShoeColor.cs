using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 신발 색 변경

public class ShoeColor : MonoBehaviour {

    SpriteRenderer render;

    float timer;
    float waitingTime;

    [Range(0, 1)]  // 범위설정 슬라이더
    public float speed = 0.2f; // 속도 조절
    [Range(0, 2)]
    public float T = 0;

    

    // Use this for initialization
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        timer = 0.0f;
        waitingTime = 1f;

    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime; // 타이머(시간)
        T = timer * speed;

        if (timer > waitingTime)
        {
            if (T < 1 && T >= 0)
            {
                render.color = new Color(T, 0, 1, 1);    // 파란색(0,0,1,1) -> 보라색(1,0,1,1)
                
            }
            else if (T >= 1 && T < 2)
            {
                render.color = new Color(2 - T, 0, 1, 1); // 보라색 -> 파란색
            }
            else if (T >= 2)
            {
                timer = 0; // 타이머 리셋
            }


        }

    }
}
