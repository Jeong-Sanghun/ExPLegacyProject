using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarScript : MonoBehaviour {

    public float HP = 10;
    public GameObject SafeBar; //초록색
    public GameObject CautionBar; // 주황색
    public GameObject DangerBar; //빨강색
    public bool GameOver = false;
    public float LerpT = 0;
    public bool InMotion = false;
    public float LastHP = 0;

    public GameObject Moby;
    public GameObject Shadow;
    public GameObject Mask;
    public NewSoundManager soundManager;
    //아니지 이걸 상속시킬까. 그러자.
    

    public void MobyStart()
    {
        StartCoroutine(MobyGo());
    }

    IEnumerator MobyGo()
    {
        while(soundManager.WholeTime == 0)
        {
            yield return new WaitForFixedUpdate();
        }
        //float MobySpeed = 9.42f / soundManager.WholeTime;   //이건 프레임안하고 그대로 간다.
        Vector2 End = new Vector2(-889.44f, -1053.65f);
        Vector2 Start = new Vector2(-898.75f, -1053.65f);
        Moby.transform.localPosition = Start;
        //-898.74
        //-889.44
        //Moby.GetComponent<Rigidbody2D>().velocity = new Vector2(MobySpeed, 0);
        float LerpT = 0;

        while (!GameOver)
        {
            Moby.transform.localPosition = Vector2.Lerp(Start, End, LerpT);
            LerpT = soundManager.nowMusic.time / soundManager.WholeTime;
            yield return new WaitForSeconds(1f);
            if (LerpT == 1)
                break;
        }
    }

    IEnumerator HPlerp(float delta)
    {

        if (LastHP != 10)
        {
            InMotion = true;
            Vector2[] VectorDumy = new Vector2[3];
            //shadow position = 0
            //mask position = 1;
            //mask scale = 2;
            VectorDumy[0] = Shadow.transform.localPosition;
            VectorDumy[1] = Mask.transform.localPosition;
            VectorDumy[2] = Mask.transform.localScale;
            Vector2 shadow = new Vector2(-0.1f * HP, 0);
            Vector2 mask = new Vector2(0.7519548f * HP/10f, 1);
            while (LerpT < 1)
            {
                if (!InMotion)
                {
                    VectorDumy[0] = Shadow.transform.localPosition;
                    VectorDumy[2] = Mask.transform.localScale;
                    shadow = new Vector2(-0.1f * HP, 0);
                    mask = new Vector2(0.7519548f * HP / 10f, 1);
                    LerpT = 0;
                }
                Shadow.transform.localPosition = Vector2.Lerp(VectorDumy[0], shadow, LerpT);
                //float ShadowPosition = Shadow.transform.localPosition.x;
                Mask.transform.localScale = Vector2.Lerp(VectorDumy[2], mask, LerpT);
                LerpT += 0.04f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        InMotion = false;
      
       
    }

    public void HPchange(float delta)
    {
        //모비의 포지션은 0이다. 로컬포지션으로 한다. 귀찮으니까 ㅎㅎ


        LastHP = HP;
        HP += delta;
        if (HP < 0)
        {
            GameOver = true;
            return;
        }
        if (HP > 10)
        {
            HP = 10;
        }
        else if(!InMotion)
        {
            LerpT = 0;
            StartCoroutine(HPlerp(delta));
        }
        else if(InMotion)
        {
            Debug.Log("인모션중에 꺼지기");
            InMotion = false;
            //모션이 진행중일 때 이걸 꺼주면 모션이 이어서 진행된다.
        }

        if (HP <= 10.0f && HP >= 6.0f)
        {
            if (!SafeBar.activeSelf)
            {
                SafeBar.SetActive(true);
            }
            if (CautionBar.activeSelf)
            {
                CautionBar.SetActive(false);
            }
            if (DangerBar.activeSelf)
            {
                DangerBar.SetActive(false);
            }

        }
        else if(HP < 6.0f && HP >= 3.0f)
        {
            if (SafeBar.activeSelf)
            {
                SafeBar.SetActive(false);
            }
            if (!CautionBar.activeSelf)
            {
                CautionBar.SetActive(true);
            }
            if (DangerBar.activeSelf)
            {
                DangerBar.SetActive(false);
            }
        }
        else if(HP < 3.0f && HP > 0f)
        {
            if (SafeBar.activeSelf)
            {
                SafeBar.SetActive(false);
            }
            if (CautionBar.activeSelf)
            {
                CautionBar.SetActive(false);
            }
            if (!DangerBar.activeSelf)
            {
                DangerBar.SetActive(true);
            }
        }
    }
    
}
