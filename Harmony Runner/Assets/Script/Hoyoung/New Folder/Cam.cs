using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {
    // 진동 세기가 이상한데?????????????????????


    public GameObject cam;
    Vector3 pos;
    Vector3 originalPos;
    bool a; // 증감 판단_ true: 증가 / false : 감소
    public float b; // 목표점
    public float c; // 기준점_ 이거 넘어가면 바뀜

    Judgement judgement;
    PauseMenu pauseMenu; // static 아니라 다시 바꿈

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;



    // Use this for initialization
    void Start()
    {
        pos = new Vector3(b, 0, 0);
        originalPos = cam.transform.position;
        pos += originalPos; // x = b + 5.8 인거임
        a = true;

        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(originalPos);
        //Debug.Log(pos);
        //Debug.Log(transform.position);





        if (judgement.HP < 5) // 5보다 작아서 좌우로 흔들림 + 맞을때 진동 -> 진동 후 리셋 할 필요 없음
        {
            shakeAmount = 0.2f;
            if (shakeDuration > 0)
            {
                if (pauseMenu.GameIsPaused == true) // esc 눌러서 멈출 때
                {
                    transform.position = originalPos; // 리셋만 하고 shakeDuration 은 안 건드리면 될듯?
                }
                else // 정상 진행
                {
                    transform.position = originalPos; // 보정
                    transform.position = originalPos + Random.insideUnitSphere * shakeAmount;

                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }

            }
            else
            {
                shakeDuration = 0f;
                //transform.position = originalPos; // 이거 때문에 밑에꺼가 거의 작동을 안했음/ 이거 끄면 문제는 hp가 5로 올라서 회복 될 때 카메라 위치가 그대로 멈춤
                // 두개로 나눠서 hp가 5 아래일때는 리셋 지우고
                // hp 가 5 위 일때는 카메라 위치 리셋 한다
            }

            if (a == true)
            {
                if (transform.position.x > 5.8f + c)
                {
                    a = false;

                }
                else // 증가
                {
                    cam.transform.position = Vector3.Lerp(cam.transform.position, pos, Time.deltaTime);
                }
            }
            else if (a == false)
            {
                if (transform.position.x < 5.8f - c)
                {
                    a = true;

                }
                else // 감소
                {
                    cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(5.8f - b, 0, -14), Time.deltaTime);
                }
            }
        }
        else // 5보다 커서 좌우로 안 흔들리고 맞을때 진동만 있음
        {
            shakeAmount = 0.04f;
            if (shakeDuration > 0)
            {
                if (pauseMenu.GameIsPaused == true) // esc 눌러서 멈출 때
                {
                    transform.position = originalPos; // 리셋만 하고 shakeDuration 은 안 건드리면 될듯?
                }
                else // 정상 진행
                {
                    transform.position = originalPos; // 보정
                    transform.position = originalPos + Random.insideUnitSphere * shakeAmount * Time.time;

                    shakeDuration -= Time.deltaTime * decreaseFactor;
                }

            }
            else
            {
                shakeDuration = 0f;
                transform.position = originalPos; // 이거 때문에 밑에꺼가 거의 작동을 안했음/ 이거 끄면 문제는 hp가 5로 올라서 회복 될 때 카메라 위치가 그대로 멈춤
            }
        }

        // 문제가 있는거 같은데
        // 첨에 5보다 작아졌다가
        // 다시 잘 쳐서 5 위로 가면
        // 좌우 흔들리는 거 없어지는데
        // 그 이후로 안 틀리면 리셋 작업을 못함!
        // 아니네! 그냥 리셋 되네.. 근데 아까는 안 그런거 같았는데 뭐지



    }
}
