using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortNoteEffect : MonoBehaviour
{

    //단노트 파티클 이펙트
    //노트 칠 때 이펙트로 퍼펙/굿/미스 판단
    //색깔이나 파티클 종류 다르게 해서 눈에 확 보이게 해야함

    //public ParticleSystem lightA; //퍼펙트 이펙트
    //public ParticleSystem lightB; // 굿 이펙트
    //public ParticleSystem lightC; // 미스 이펙트
    SpriteRenderer renderA; // 스크립트 달린 오브젝트의 스프라이트 렌더러 (노트에 달림)

    Judgement judgement; // 저지먼트 스크립트 좀 사용

    //SpriteRenderer renderA; // 스크립트 달린 오브젝트의 스프라이트 렌더러 (노트에 달림)
    public GameObject PerfectObject; // 퍼펙트_이펙트_오브젝트 - Light
    public GameObject GoodObject; // 굿_이펙트_오브젝트 - Star_A
    public GameObject MissObject; // 미스_이펙트_오브젝트 - Heart
    public ParticleSystem ParfectEffect; // 퍼펙트_이펙트
    public ParticleSystem GoodEffect; // 굿_이펙트
    public ParticleSystem MissEffect; // 미스_이펙트

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
            if (renderA.color.a < 1 && renderA.color.a > 0.8) // 노트의 색 변할때(투명해질때) (이거 해야 뒤에 있는거 같이 안변함)
            {
                if (ParfectEffect.isPlaying == true) // 실행중이면 스탑
                {
                    ParfectEffect.Stop();
                }
                else // 아니면 실행
                {
                    Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장 -> 그 자리에서 실행되도록
                    vec.z = -3; //앞으로 약간 이동해야 보임
                    ParfectEffect.transform.position = vec;
                    ParfectEffect.Play();
                    //Debug.Log("Perfect");
                }

            }
        }
        else if (judgement.point == 1)
        {
            //굿
            if (renderA.color.a < 1 && renderA.color.a > 0.8)
            {
                if (GoodEffect.isPlaying == true)
                {
                    GoodEffect.Stop();
                }
                else
                {
                    Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장
                    vec.z = -3; //앞으로 약간 이동해야 보임
                    GoodEffect.transform.position = vec;
                    GoodEffect.Play();
                    
                }

            }
        }
        else
        {
            //미스
            if (renderA.color.a < 1 && renderA.color.a > 0.8)
            {
                if (MissEffect.isPlaying == true)
                {
                    MissEffect.Stop();
                }
                else
                {
                    Vector3 vec = transform.position; // 노트 비활성화 될때 위치값 저장
                    vec.z = -3; //앞으로 약간 이동해야 보임
                    MissEffect.transform.position = vec;
                    MissEffect.Play();
                    

                }

            }
        }

    }
}

