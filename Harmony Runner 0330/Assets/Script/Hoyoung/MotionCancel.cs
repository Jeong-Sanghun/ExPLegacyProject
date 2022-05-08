using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCancel : MonoBehaviour {

    //애니메이션
    public Animator animator;
    AnimatorStateInfo animInfo;

    //잔상
    public GameObject[] afterImage;
    public int i = 0;

    SpriteRenderer[] rend = new SpriteRenderer[4];
    Vector3 Scale;

    float t1 = 0;
    float t2 = 0;
    float t3 = 0;
    float t4 = 0;
    //float a;

    public int asd = 1;

    bool w = false;
    bool a = false;
    bool s = false;
    bool d = false;



    // Use this for initialization
    void Start()
    {

        for (int n = 3; n >= 0; n--) // 렌더러 비활성화
        {
            rend[n] = afterImage[n].GetComponent<SpriteRenderer>();
            rend[n].enabled = false;
            //Debug.Log(rend[n]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        animInfo = animator.GetCurrentAnimatorStateInfo(0); // 애니메이터 현재 상태 받아옴

        if (animInfo.IsName("jump")) // w
        {

            if (animInfo.normalizedTime < 1 * 1.16 / 2.5) // 모션 끝나고 잔상 남는거 방지 
            {


                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) // 점프 애니메이션 실행중에 다른 키 눌리면 (여기서 속도 차이가 있네)
                {
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        //asd = 10;
                    }
                    rend[0].enabled = true;
                    //Invoke("Remove1", 1f);
                    StartCoroutine(MakeTransparent1());
                   // Debug.Log("qwe " + t1);
                    /*
                    if(t1 > 0.8f) // 이게 계속 돌면서  체크하는게 아니라 한번 아니면 다음번에 되는군..;;
                    {
                        Debug.Log("zzz" + t1);
                        Remove1();
                        Invoke("tReset1", 1f);
                        //여기에 tReset 넣으면 어떨까??
                    }
                    */
                    /*
                    if (rend[0].color.a <= 0.2f)
                    {
                        Remove1();
                    }
                    */
                    //remove를 시간에 대해서 하지 말고 투명도 기준으로 하면 어떨까..?  t 기준이 가장 나은듯
                }

            }

        }
        else if (animInfo.IsName("kick")) // d
        {

            if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S)) // 점프 애니메이션 실행중에 다른 키 눌리면
            {
                rend[1].enabled = true;
                //Invoke("Remove2", 1f);
                StartCoroutine(MakeTransparent2());
                //Invoke("tReset2", 1f);
                /*
                if (t2 > 0.8f)
                {
                    Remove2();
                }
                */

                //StartCoroutine(Motion2());
            }
        }
        else if (animInfo.IsName("pass")) // a
        {

            if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.D)) // 점프 애니메이션 실행중에 다른 키 눌리면
            {
                rend[2].enabled = true;
                //Invoke("Remove3", 1f);
                StartCoroutine(MakeTransparent3());
                //Invoke("tReset3", 1f);
                /*
                if (t3 > 0.8f)
                {
                    Remove3();
                }
                */
                //StartCoroutine(Motion3());
            }
        }
        else if (animInfo.IsName("slide")) // s
        {

            if (animInfo.normalizedTime < 1 * 1.54 / 2.2)
            {


                if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) // 점프 애니메이션 실행중에 다른 키 눌리면
                {
                    rend[3].enabled = true;
                    //Invoke("Remove4", 1f);
                    StartCoroutine(MakeTransparent4());
                    /*Invoke("tReset4", 1f);
                    if (t4 > 0.8f)
                    {
                        Remove4();
                    }
                    */
                    //StartCoroutine(Motion4());

                }
            }
        }

    }

    void Remove1()
    {
        //a = rend[0].color.a;
        //a = 1;
        //rend[0].color = new Color(rend[0].color.r, rend[0].color.g, rend[0].color.b, 1);
        rend[0].enabled = false;


    }
    void Remove2()
    {
        rend[1].enabled = false;
    }
    void Remove3()
    {
        rend[2].enabled = false;
    }
    void Remove4()
    {
        rend[3].enabled = false;
    }
    /*
    IEnumerator Motion() // 코루틴 활용 잔상이미지 효과
    {
        while (t < 1)
        {
            
            //if (i == 0) // 퍼센트로 줄이면 굳이 둘이 나눌 필요 없자나?
            //{
            //    afterImage[0].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.5f, t), Mathf.Lerp(Scale.y, 0.5f, t), Scale.z);
            //}
            //else
            //{
            //   afterImage[i].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f, t), Mathf.Lerp(Scale.y, 0.2f, t), Scale.z);
            //}
            
            Scale = afterImage[i].transform.localScale;
            afterImage[i].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f*Scale.x, t), Mathf.Lerp(Scale.y, 0.2f*Scale.y, t), Scale.z);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator Motion1() // 코루틴 활용 잔상이미지 효과
    {
        while (t < 1)
        {
            
            Scale = afterImage[0].transform.localScale;
            afterImage[0].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f * Scale.x, t), Mathf.Lerp(Scale.y, 0.2f * Scale.y, t), Scale.z);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator Motion2() // 코루틴 활용 잔상이미지 효과
    {
        while (t < 1)
        {

            Scale = afterImage[1].transform.localScale;
            afterImage[1].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f * Scale.x, t), Mathf.Lerp(Scale.y, 0.2f * Scale.y, t), Scale.z);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator Motion3() // 코루틴 활용 잔상이미지 효과
    {
        while (t < 1)
        {

            Scale = afterImage[2].transform.localScale;
            afterImage[2].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f * Scale.x, t), Mathf.Lerp(Scale.y, 0.2f * Scale.y, t), Scale.z);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator Motion4() // 코루틴 활용 잔상이미지 효과
    {
        while (t < 1)
        {

            Scale = afterImage[3].transform.localScale;
            afterImage[3].transform.localScale = new Vector3(Mathf.Lerp(Scale.x, 0.2f * Scale.x, t), Mathf.Lerp(Scale.y, 0.2f * Scale.y, t), Scale.z);

            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    */
    IEnumerator MakeTransparent1()
    {
        rend[0].color = new Color(rend[0].color.r, rend[0].color.g, rend[0].color.b, 1);
        while (t1 < 1)
        {


            rend[0].color = new Color(rend[0].color.r, rend[0].color.g, rend[0].color.b, Mathf.Lerp(1, 0, t1));
            t1 += 1 * Time.deltaTime * asd;
            yield return new WaitForSeconds(0.01f);
            //Debug.Log(t1);

        }
        Remove1();
        Invoke("tReset1", 1f);


    }
    IEnumerator MakeTransparent2()
    {
        rend[1].color = new Color(rend[1].color.r, rend[1].color.g, rend[1].color.b, 1);
        while (t2 < 1)
        {


            rend[1].color = new Color(rend[1].color.r, rend[1].color.g, rend[1].color.b, Mathf.Lerp(1, 0, t2));
            t2 += 1 * Time.deltaTime;
            //t2 += 0.005f;
            yield return new WaitForSeconds(0.01f);


        }
        Remove2();
        Invoke("tReset2", 1f);
    }
    IEnumerator MakeTransparent3()
    {
        rend[2].color = new Color(rend[2].color.r, rend[2].color.g, rend[2].color.b, 1);
        while (t3 < 1)
        {


            rend[2].color = new Color(rend[2].color.r, rend[2].color.g, rend[2].color.b, Mathf.Lerp(1, 0, t3));
            t3 += 1 * Time.deltaTime;
            //t3 += 0.005f;
            yield return new WaitForSeconds(0.01f);


        }
        Remove3();
        Invoke("tReset3", 1f);
    }
    IEnumerator MakeTransparent4()
    {
        rend[3].color = new Color(rend[3].color.r, rend[3].color.g, rend[3].color.b, 1);
        while (t4 < 1)
        {


            rend[3].color = new Color(rend[3].color.r, rend[3].color.g, rend[3].color.b, Mathf.Lerp(1, 0, t4));
            t4 += 1 * Time.deltaTime;
            //t4 += 0.005f;
            yield return new WaitForSeconds(0.01f);


        }
        Remove4();
        Invoke("tReset4", 1f);
    }
    void tReset1()
    {
        t1 = 0;
    }
    void tReset2()
    {
        t2 = 0;
    }
    void tReset3()
    {
        t3 = 0;
    }
    void tReset4()
    {
        t4 = 0;
    }
}

