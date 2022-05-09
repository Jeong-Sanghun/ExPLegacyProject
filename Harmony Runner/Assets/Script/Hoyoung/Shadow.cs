using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    //그림자
    public GameObject ShadowObj;
    public int speed; // 현재 스피드에서 몇배할지
    Judgement judgement;
    // Use this for initialization
    void Start()
    {
        judgement = GameObject.Find("NoteManager").GetComponent<Judgement>();

    }

    //게임오버 전에 그림자 따라오게
    public void Gameover()
    {

        if (judgement.HP <= 0f)
        {

            if (ShadowObj.transform.position.x >= 0f)
            {
                //게임오버매니저 스크립트에
                // 게임오버 창
                //Debug.Log("게임오버");
            }
            else
            {
                ShadowObj.transform.Translate(0.03f *speed, 0, 0); // 그림자 따라옴 속도조절
            }
        }

    }
}
