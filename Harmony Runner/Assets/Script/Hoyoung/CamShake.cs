using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

    bool isShaking = false;
    public float shakeDurationValue = 0;
    public float shakeAmount = 1f;
    public float decreaseFactor = 1f;
    public Vector3 originPosition;
    public Transform targets;

    bool a; // 증감 판단_ true: 증가 / false : 감소
    public float b; // 목표점
    public float c; // 기준점_ 이거 넘어가면 바뀜
    Vector3 pos;

    Judgement judgement;
    PauseMenu pauseMenu; // static 아니라 다시 바꿈

    bool isSwinging = false;
    public float swingPoint = 0;

    // Use this for initialization
    void Start () {
        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();

        pos = new Vector3(b, 0, 0);
        pos += originPosition; // x = b + 5.8 인거임
    }
	
	// Update is called once per frame
	void Update () {
        
       if(judgement.HP < 5) // 체력이 5보다 작으면 좌우로 흔들림
        {
            swingPoint = 1;
            if(swingPoint > 0)
            {
                if (pauseMenu.GameIsPaused == true) // 게임멈춤
                {

                    swingPoint = 0;
                }
                else
                {
                    swing();
                }
            }

            if (shakeDurationValue > 0)
            {
                if (pauseMenu.GameIsPaused == true) // 게임멈춤
                {

                    shakeDurationValue = 0;
                }
                else
                {
                    shake();
                }

            }

        }
        else
        {
            swingPoint = 0;
            if (shakeDurationValue > 0)
            {
                if (pauseMenu.GameIsPaused == true) // 게임멈춤
                {

                    shakeDurationValue = 0;
                }
                else
                {
                    shake();
                }

            }
        }
        

    }

    public void swing()
    {
        StartCoroutine("Swinging");
    }

    IEnumerator Swinging()
    {
        if (isSwinging)
            yield break;

        isSwinging = true;

        while (true)
        {
            if(swingPoint > 0)
            {
                
                if (a == true) //증가중
                {
                    if (transform.position.x > 5.8f + c) //기준점 지남
                    {
                        a = false; // 감소해야함

                    }
                    else // 기준점 안지남 -> 증가 더 해야함
                    {
                        targets.transform.position = Vector3.Lerp(targets.transform.position, pos, Time.deltaTime);
                    }
                }
                else if (a == false) // 감소중
                {
                    if (transform.position.x < 5.8f - c) // 기준점 지남
                    {
                        a = true; // 증가해야함

                    }
                    else // 기준점 안지남 -> 감소 더 해야함
                    {
                        targets.transform.position = Vector3.Lerp(targets.transform.position, new Vector3(5.8f - b, 0, -14), Time.deltaTime);
                    }
                }
            }
            else
            {
                swingPoint = 0;
                targets.position = originPosition;

                isSwinging = false;
                break;
            }
            yield return null;
        }
    }

    public void shake()
    {
        StartCoroutine("Shaking");
    }

    IEnumerator Shaking()
    {
        if (isShaking)
            yield break;

        isShaking = true;

        while (true)
        {
            if (shakeDurationValue > 0)
            {
                
                    Vector3 randomValue = Random.insideUnitSphere * shakeAmount;
                    
                    targets.position = Vector3.Lerp(targets.position, originPosition + randomValue, 0.05f);
                
                shakeDurationValue -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDurationValue = 0f;
                
                targets.position = originPosition;
                
                isShaking = false;
                break;
            }
            yield return null;
        }
    }
}
