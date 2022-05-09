using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionCapture : MonoBehaviour {

    public SpriteRenderer[] SpriteList;
    public Animator animator;
    AnimatorStateInfo animInfo;


    
    // Use this for initialization
    void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        animInfo = animator.GetCurrentAnimatorStateInfo(0); // 애니메이터 현재 상태 받아옴


        if (animInfo.IsName("jump"))
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                SpriteList = transform.GetComponentsInChildren<SpriteRenderer>();

            }
        }
    }
}
