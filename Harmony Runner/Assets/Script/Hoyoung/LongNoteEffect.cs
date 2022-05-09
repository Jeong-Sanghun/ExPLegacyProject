using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteEffect : MonoBehaviour
{

    //롱노트 파티클 이펙트
    //노트 칠 때 이펙트로 퍼펙/굿/미스 판단
    //색깔이나 파티클 종류 다르게 해서 눈에 확 보이게 해야함

    //public ParticleSystem lightA; //퍼펙트 이펙트
    //public ParticleSystem lightB; // 굿 이펙트
    //public ParticleSystem lightC; // 미스 이펙트
    SpriteRenderer renderA; // 스크립트 달린 오브젝트의 스프라이트 렌더러 (노트에 달림)
    public GameObject PerfectObject; // 퍼펙트_이펙트_오브젝트 - Light
    public GameObject GoodObject; // 굿_이펙트_오브젝트 - Star_A
    public GameObject MissObject; // 미스_이펙트_오브젝트 - Heart
    public ParticleSystem ParfectEffect; // 퍼펙트_이펙트
    public ParticleSystem GoodEffect; // 굿_이펙트
    public ParticleSystem MissEffect; // 미스_이펙트

    Judgement judgement; // 저지먼트 스크립트 좀 사용

    private void Awake()
    {
        //프리팹?에 안 들어가서 이렇게 넣음
        PerfectObject = GameObject.Find("Light"); // 퍼펙 - 원 
        ParfectEffect = PerfectObject.GetComponent<ParticleSystem>();
        GoodObject = GameObject.Find("Star_A"); // 굿 - 스타
        GoodEffect = GoodObject.GetComponent<ParticleSystem>();
        MissObject = GameObject.Find("Heart");  // 미스 - 하트
        MissEffect = MissObject.GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        renderA = GetComponent<SpriteRenderer>();

        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>(); // 노트매니저의 저지먼트 사용


    }

    // Update is called once per frame
    void Update()
    {
        EffectA();

    }


    void EffectA()
    {

        if (judgement.point == 2)
        {
            //퍼펙트
            if (transform.position.x < 1.3)
            {

                if (ParfectEffect.isPlaying == true) // 실행중이면 스탑
                {

                    ParfectEffect.Stop();

                }
                else // 아니면 실행
                {
                    //Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장
                    //Debug.Log(vec);
                    //aaa.transform.position = vec;

                    ParfectEffect.transform.position = new Vector3(-0.02f, -4, 0); // 롱노트 퍼펙은 판정선에 위치
                    if (transform.GetChild(1).transform.position.x < 1.3) // 롱노트 꼬리가 지나가면 끝나게!
                    {
                        //Debug.Log(transform.GetChild(1));
                        //Debug.Log(transform.GetChild(1).transform.position.x);
                        ParfectEffect.Stop();
                    }
                    else
                    {

                        ParfectEffect.Play();
                    }


                    //Debug.Log("롱노트 퍼펙트 판정");
                }
            }

        }
        else if (judgement.point == 1)
        {
            //굿 - 시작할 때만 판정 중간에는 퍼펙/미스
            //거의 단노트 판정과 같기 때문에 그대로 유지해도 괜찮쓰
            if (transform.position.x < 1.3)
            {
                if (GoodEffect.isPlaying == true)
                {
                    GoodEffect.Stop();
                }
                else
                {
                    Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장
                                                      //Debug.Log(vec);
                    GoodEffect.transform.position = vec;
                    GoodEffect.Play();
                    //Debug.Log("롱노트Goood");
                }
            }

        }
        else
        {
            //미스
            //미스는 어떻게 하지? 일단 퍼펙트와 성질이 같음, 굿 과 달리 계속 판정되는거
            //하지만 퍼펙트는 어느정도 타협이 가능한데 미스는 틀리는 부분이 판정선에서 훨씬 멀수도 있음
            //그렇게 따지면 그 위치에서 이펙트가 나오는게 현실적
            //처음은 그런데 뒤쪽은 판정선에서 나와도 상관은 없음 오히려 보이기에는 괜찮을수있음 (i think..)
            //흠... 수정 좀 하자..
            if (transform.position.x < 2) // 이거 거리도.. 좀 조절해야할거 같은데.. 나머진 괜찮은데 미스는...
            {
                if (MissEffect.isPlaying == true)
                {
                    MissEffect.Stop();
                }
                else
                {
                    MissEffect.transform.position = new Vector3(-0.02f, -4, 0);
                    if (transform.GetChild(1).transform.position.x < 1.3) // 롱노트 꼬리가 지나가면 끝나게!
                    {

                        MissEffect.Stop();
                    }
                    else
                    {
                        //Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장
                        //Debug.Log(vec);
                        //ccc.transform.position = vec;
                        MissEffect.Play();
                        //Debug.Log("롱노트miss");
                    }
                }
            }

        }

    }
}