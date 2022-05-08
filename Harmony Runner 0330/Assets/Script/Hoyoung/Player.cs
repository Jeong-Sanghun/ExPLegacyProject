using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //키 동시에 누르는 경우 없으니까
    //트리거 동시에 켜지면 하나 막아버리자
    // 순서대로 실행 되서.. 우선순위가 상>하>좌>우 가 되어버림;;
    // 먼저 누른게 우선 되게 하려 했는데



    //점프
    [Range(0, 10)]
    public float jumpPower;
    Rigidbody rigdbody;
    bool isJumping;

    //int count = 1;

    //모션
    private Animator anim;


    //HpVibe hpVibe; // 체력바 진동

    //충돌
    Spriter2UnityDX.EntityRenderer entityRenderer;
    public GameObject shoe1;
    public GameObject shoe2;

    public bool isCollide;

    Judgement judgement;


    /*
    private SpriteRenderer upSpriteRenderer; //
    public GameObject Up; //
    public Sprite CurrentUpSprite; //
    public Sprite NextUpSprite; //
    */

    void Jump() // 애니메이터 아님
    {
        if (!isJumping)
            return;
        if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
        {
            rigdbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }


        isJumping = false;
    }

    private void Awake()
    {
        rigdbody = GetComponent<Rigidbody>();

        //모션
        anim = GetComponent<Animator>();
        //anim = GameObject.Find("entity_000").GetComponent<Animator>(); // entity_000 의 애니메이터 받아옴

        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>();
    }

    // Use this for initialization
    void Start()
    {

        //충돌_색
        entityRenderer = GameObject.Find("entity_000").GetComponent<Spriter2UnityDX.EntityRenderer>();


        //hpVibe = GameObject.Find("HpBar").GetComponent<HpVibe>(); // 체력바 진동

        //upSpriteRenderer = Up.GetComponent<SpriteRenderer>();  //
        //upSpriteRenderer.sprite = CurrentUpSprite; //


    }

    // Update is called once per frame
    void Update()
    {
        // 이거 때문에 위 아래 진동 생김.. 수정해야해 -> 높이 제한 더 높임
        // 약간 더 수정해야할거 같은데;;
        // 점프할때 캔슬 바로 되게 하려면 이거 수정해야함!
        if (transform.position.y > -2) // 어느정도 위에서 다른 버튼을 누르면
        {
            if (Input.GetKey(KeyCode.S))
            {
                rigdbody.AddForce(Vector3.down * 2, ForceMode.Impulse);

            }
            else if (Input.GetKey(KeyCode.A))
            {
                rigdbody.AddForce(Vector3.down * 1.5f, ForceMode.Impulse);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rigdbody.AddForce(Vector3.down * 1, ForceMode.Impulse);
            }
            // 더 이상 올라가지 않고, 내려오는 속도 빠르게

        }
        else
        {
            if (transform.localPosition.y < -10)
            {
                transform.localPosition = new Vector3(0, -5f, -0.5f);
                rigdbody.velocity = Vector3.zero;
            }
            if (Input.GetKey(KeyCode.W))
            {
                
                //anim.SetBool("isJump", true);
                if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                {
                    anim.SetTrigger("Jump");
                }



                // upSpriteRenderer.sprite = NextUpSprite; //

                // 점프모션 자연스럽게 하기위함 , point 는 점프 연속으로 눌릴 때 사용
                // 키 연속으로 눌릴 때 애니메이션하고 점프하고 싱크 안맞던 문제 해결
                if (transform.position.y <= -3) // 땅에 붙어있을 때 
                {
                    if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                    {
                        anim.SetInteger("point", 2); // 기준인 1보다 크기 때문에 점프 연속
                    }


                    //Debug.Log("count");
                }
                else
                {
                    anim.SetInteger("point", 0); // 땅에서 멀리 떨어져있을때 기준인 1보다 작으므로 점프 연속 x
                }

                //anim.enabled = false; 이건 아예 전체 애니메이터 정지..
                //anim.Play("run"); // run 실행
                //anim.speed = 2; // speed 조절 이것도 전체 애니메이터


                if (transform.position.y <= -3)  //GameObject.Find("entity_000").GetComponent<Transform>().position.y <= 1
                {
                    if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                        isJumping = true;

                }
                else
                {
                    isJumping = false;

                }

                // 모션캔슬 부분 (점프 -> 다른거)

            }
            else // 위 키 안눌리면
            {
                anim.ResetTrigger("Jump"); // 점프 트리거 리셋

                if (Input.GetKey(KeyCode.S)) // 아래
                {
                    if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                    {
                        anim.SetTrigger("Slide");
                    }



                }
                else // 아래키 안 눌리면
                {
                    anim.ResetTrigger("Slide");

                    if (Input.GetKey(KeyCode.A)) // 왼쪽 키 눌리면
                    {
                        if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                        {
                            anim.SetTrigger("Pass");
                        }

                    }
                    else // 왼쪽 키도 아니면
                    {
                        anim.ResetTrigger("Pass");
                        if (Input.GetKey(KeyCode.D)) // 오른쪽 키 눌릴때
                        {
                            if (judgement.point == 1 || judgement.point == 2 || judgement.point == 3)
                            {
                                anim.SetTrigger("Kick");
                            }


                        }
                        else // 왼쪽 키 안눌릴때
                        {
                            anim.ResetTrigger("Kick");
                        }


                    }
                }
            }
        }





    }


    private void FixedUpdate()
    {
        Jump();
    }




    //충돌
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("충돌");

        if (collision.gameObject.tag.Equals("Obstacle"))
        {
            isCollide = true;
            //StartCoroutine("Collide");

            //hpVibe.shake = 1; // 체력바


            //entityRenderer.Color = new Color(1, 1, 1, 0.5f);  // 전체 색 같이 변함(투명화)


            shoe1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1); // 신발색 흰색으로 변하지 않게 따로 설정
            shoe2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
        }

    }


    private void OnCollisionExit(Collision collision)
    {
        //entityRenderer.Color = new Color(1, 1, 1, 1);
        shoe1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
        shoe2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
    }

}
    


    



