using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Judgement : HpBarScript{

    //판정, 체력, 콤보를 쓰는 스크립트. PassScript에서

    public GameObject TextObject;
    public Text ComboText;
    public CheckBBoom checkBBoom;
    public GameManager gameManager;
    public GameOverManager gameoverManager;
    public NewNoteMaker noteMaker;
    public ScoreBoard scoreBoard;

    public Text[] Scores; //score topscore accuracy
    public float Percentage = 0;
    //public ComboNumber[] NumberSprite; //검정 초록 파랑 빨강 요거는 4개다
    //public Queue<int>[] NumberQ = new Queue<int>[4];
    //bool DigitChanged = false;
    int combo = 0;
    int missCombo = 0;
    float score = 0;
    float baseScore = 0;
    float opacity = 1f;
    float Accuracy = 0;
    float LatestAccuracy = 0;
    int NoteIndex = 0;
    int LongestCombo = 0;

    public float ScoreRate = 100;

    HpVibe hpVibe; // 체력바 진동
    //Cam cam; // 카메라 진동
    CamShake camShake;

    public float RandomRate;
    public float RunnersRate;
    public float DarkRate;
    public float FastRate;
    public float PerfectRate;
    public float RHighRate;

    public bool RHighMode;
    public bool PerfectMode;

    public int TopScore;

    //충돌
    Spriter2UnityDX.EntityRenderer entityRenderer;
    public GameObject shoe1;
    public GameObject shoe2;

    public bool isCollide;
    public int point; // 다른 스크립트에서 퍼펙트,굿,미스 체크하려고..

    public void ChangeScoreRate(bool random, bool runners, bool dark, bool fast, bool perfect, bool rhigh)
    {
        ScoreRate = 100;
        if (random)
            ScoreRate *= RandomRate;
        if (runners)
            ScoreRate *= RunnersRate;
        if (dark)
            ScoreRate *= DarkRate;
        if (fast)
            ScoreRate *= FastRate;
        if (perfect)
            ScoreRate *= PerfectRate;
        if (rhigh)
            ScoreRate *= RHighRate;
        PerfectMode = perfect;
        RHighMode = rhigh;
    }
    public void GameStart()
    {
        MobyStart();
        CautionBar.SetActive(false);
        DangerBar.SetActive(false);
        scoreBoard = new ScoreBoard();
        LongestCombo = 0;
        baseScore = 0;
        /*
        mySprites[0] = OneNumberObject.GetComponent<SpriteRenderer>();
        mySprites[1] = TenNumberObject.GetComponent<SpriteRenderer>();
        mySprites[2] = HundreadNumberObject.GetComponent<SpriteRenderer>();
        mySprites[3] = ThousandNumberObject.GetComponent<SpriteRenderer>();
        */
        //검정 초록 파랑 빨강
        //검정 먼저 1개, (그 1개 instantiate, 그 1개의 차일드 10개 배열 가져오기.)이거 4번
        //wholenumber 1 2 3 4 검정 초록 파랑 빨강
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        noteMaker = GetComponent<NewNoteMaker>();

        hpVibe = GameObject.Find("HpBar").GetComponent<HpVibe>(); // 체력바 진동
        //cam = GameObject.Find("Main Camera").GetComponent<Cam>(); // 카메라 진동
        camShake = GameObject.Find("Main Camera").GetComponent<CamShake>(); // 카메라 진동


        //충돌_색
        entityRenderer = GameObject.Find("entity_000").GetComponent<Spriter2UnityDX.EntityRenderer>();
        if (TopScore == 0)
        {
            Scores[1].text = "00000";
        }
        else
        {
            Scores[1].text = TopScore.ToString();
        }
      

        point = 3;
    }
    //검정 초록 파랑 빨강



    void OnCombo()
    {
       
        ComboText.text = combo.ToString();
        StartCoroutine(ComboColorChange());
        Scores[0].text = score.ToString("N0");

        Accuracy = (Accuracy * NoteIndex + LatestAccuracy) / ++NoteIndex;
        Scores[2].text = Accuracy.ToString("N1");


    }
    IEnumerator ComboColorChange()
    {
        opacity = 1;
        while (opacity > 0)
        {
            opacity -= 0.05f;
            if (combo < 30)
            {
                ComboText.color = new Color(0, 0, 0, opacity);
            }
            else if (combo < 100)
            {
                ComboText.color = new Color(0, 1, 0, opacity);
            }
            else if (combo < 200)
            {
                ComboText.color = new Color(0, 0, 1, opacity);
            }
            else
            {
                ComboText.color = new Color(1, 0, 0, opacity);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void Perfect(float delta)
    {   
        checkBBoom.CheckerChange();
        combo++;
        if (combo > LongestCombo)
        {
            LongestCombo = combo;
        }
        scoreBoard.PerfectHit++;
        float scoreRate = ScoreRate;
        if (HP < 10)
        {
            HPchange((1.0f+(float)combo)/42f);
        }
         float a = (scoreRate *(1 + Mathf.Log10(combo)) * (HP / 10.0f));
        float b = (100.0f * (1 + Mathf.Log10(combo)) * (HP / 10.0f));
        baseScore += b;
        score += a;

        missCombo = 0;
        if (delta != -1)
        {
            if (RHighMode)
            {
                noteMaker.OnRHigh(false);
            }
            delta = Mathf.Abs(delta);

            LatestAccuracy = 10 * (10 - Mathf.FloorToInt(delta / 1.2f));
           // Debug.Log(Mathf.FloorToInt(delta / 12f));
        }
        else
        {
            LatestAccuracy = 100;
        }
        OnCombo();
        //12프레임이야. 만약 8프레임이라면, 7.~~겠지.

        point = 2;
        Invoke("Reset", 0.2f);
        //StartCoroutine(ComboChange());
    }

    public void Miss(float delta)
    {
        missCombo++;
        combo = 0;
        scoreBoard.MissHit++;
        //HPchange(-0.75f*(0.3f + 0.2f * missCombo));

        Debug.Log(delta);
        if (delta != -1)
        {
            delta = Mathf.Abs(delta);
            LatestAccuracy = 10 * (10 - Mathf.FloorToInt(delta / 1.2f));
            Debug.Log(Mathf.FloorToInt(delta / 1.2f));
        }
        else
        {
            LatestAccuracy = 0;
        }
        OnCombo();
        if (RHighMode)
        {
            noteMaker.OnRHigh(true);
        }


        // StartCoroutine(ComboChange());

        // 체력바 진동
        hpVibe.shake = 1;
        // 충돌 시 투명
        isCollide = true;
        StartCoroutine("Collide");

        point = 0;
        Invoke("Reset", 0.2f);

        //cam.shakeDuration = 1; // 카메라 진동
        camShake.shakeDurationValue = 1;
        if (HP <= 0 || PerfectMode)
        {
            gameManager.Gameover();
        }
    }


    public void Good(float delta)
    {
        combo++;
        scoreBoard.GoodHit++;
        if (combo > LongestCombo)
        {
            LongestCombo = combo;
        }
        if (HP < 10)
        {
            HPchange((1.0f + (float)combo) / 42f);
        }
        float scoreRate = ScoreRate;
        /*
        if (RHighMode)
        {
            float rate = 1f + (combo / 20) * 0.01f;
            scoreRate *= rate;
        }*/
        float a = (scoreRate/2 * (1 + Mathf.Log10(combo)) * (HP / 10));
        float b = (50.0f * (1 + Mathf.Log10(combo)) * (HP / 10));
        baseScore += b;
        score += a;
        if (delta != -1)
        {
            if (RHighMode)
            {
                noteMaker.OnRHigh(false);
            }
            delta = Mathf.Abs(delta);

            LatestAccuracy = 10 *(10- Mathf.FloorToInt(delta / 1.2f));

            //Debug.Log(Mathf.FloorToInt(delta / 12f));
     
        }
        missCombo = 0;
        OnCombo();

        point = 1;
        Invoke("Reset", 0.2f);
        if (PerfectMode)
        {
            gameManager.Gameover();
        }
        //StartCoroutine(ComboChange());
    }

    public void GameClear()
    {
        if(Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        scoreBoard.BaseScore = (int)(score / (ScoreRate/100.0f));
        scoreBoard.TotalScore = (int)score;
        scoreBoard.Multiplier = ScoreRate / 100.0f;
        scoreBoard.Accuracy = Accuracy;
        scoreBoard.LongestCombo = LongestCombo;
        scoreBoard.LongestComboPer = 100*LongestCombo / scoreBoard.WholeCombo;
        scoreBoard.SetTime();
        soundManager.MusicStop();
        LevelLoader.inst.ClearSceneLoad(scoreBoard);
    }

    

    // 충돌 시 투명
    IEnumerator Collide()
    {
        int countTime = 0;

        while (countTime < 10)
        {
            if (countTime % 2 == 0)
            {
                entityRenderer.Color = new Color(1, 1, 1, 0.25f);
                shoe1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
                shoe2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
            }

            else
            {
                entityRenderer.Color = new Color(1, 1, 1, 0.5f);
                shoe1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
                shoe2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
            }


            yield return new WaitForSeconds(0.2f);

            countTime++;
        }

        entityRenderer.Color = new Color(1, 1, 1, 1);
        shoe1.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);
        shoe2.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 1, 1);

        isCollide = false;

        yield return null;

    }

    void Reset()
    {
        point = 3; // default 값을 3으로 해놓고 틀리면 0으로
    }

    public void ActionAdd(bool hitted)
    {
        if (hitted)
        {
            scoreBoard.Action++;
        }
        scoreBoard.WholeAction++;

    }
    public void WholeComboAdd(int nums)
    {
        scoreBoard.WholeCombo += nums;
    }

    public void SetMusicName(string name, string level, int levelInt)
    {
        scoreBoard.MusicName = name;
        scoreBoard.Level = level;
        scoreBoard.LevelInt = levelInt;
    }
    
    public void RouteAdd(int index)
    {
        if(index != -1)
        {
            scoreBoard.SwitchedIndex.Add(index);
        }
        scoreBoard.SwitchSum++;
    }

}

