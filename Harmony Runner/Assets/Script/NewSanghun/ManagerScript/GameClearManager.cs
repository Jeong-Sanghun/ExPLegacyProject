using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour {

    public ScoreBoard scoreBoard;
    public Text MusicName;
    public Text Level;

    public Text NoteHit;
    public Text WholeNoteHit;
    public Text Perfect;
    public Text Good;
    public Text Miss;

    public Text LCombo;
    public Text LComboPer;
    public Text Action;
    public Text WholeAction;

    public List<GameObject> SwitchList;
    public GameObject Switch; //switch pos. (925 - 40*index, 51) 0번 차일드를 set true하면 on이 된다.
    public Transform SwitchParents;

    public Text Accuracy;
    public Text BaseScore;
    public Text Multiplier;
    public Text TotalScore;

    public Image Emblem;
    public Image BaseBG;
    public Image RankBG;

    public Image AlbumArt;

    public GameObject Buttons;
   
    public Sprite[] EmblemSprites;  //best s a b c d 
    public Sprite[] BaseSprite;
    public Sprite[] RankSprite;

    public CutSceneJson cutSceneJson;
    public bool cutSceneBool;

    public string NameInput { get; set; }

    public void ClearStart()
    {
        cutSceneBool = false;
        CutSceneFormatLoad();
        if (cutSceneBool)
            return;

        SwitchList = new List<GameObject>();

        MusicName.text = scoreBoard.MusicName;
        Level.text = scoreBoard.LevelInt.ToString() + "  " + scoreBoard.Level;

        Perfect.text = scoreBoard.PerfectHit.ToString();
        Good.text = scoreBoard.GoodHit.ToString();
        Miss.text = scoreBoard.MissHit.ToString();
        NoteHit.text = (scoreBoard.PerfectHit + scoreBoard.GoodHit).ToString();
        WholeNoteHit.text = scoreBoard.WholeCombo.ToString();
        LCombo.text = scoreBoard.LongestCombo.ToString();
        LComboPer.text = scoreBoard.LongestComboPer.ToString("N1") + "%";
        Action.text = scoreBoard.Action.ToString();
        WholeAction.text = scoreBoard.WholeAction.ToString();

        Accuracy.text = scoreBoard.Accuracy.ToString("N1") + "%";
        BaseScore.text = scoreBoard.BaseScore.ToString();
        Multiplier.text = scoreBoard.Multiplier.ToString("N2");
        TotalScore.text = scoreBoard.TotalScore.ToString();

        float actionRate = (float)(scoreBoard.Action + scoreBoard.SwitchedIndex.Count) / (scoreBoard.WholeAction + scoreBoard.SwitchSum);

        if(scoreBoard.WholeAction + scoreBoard.SwitchSum == 0)
        {
            actionRate = 1;
        }
        float accuracy = scoreBoard.Accuracy;

        if (accuracy >= 50)
        {
            scoreBoard.ScoreRank = ScoreBoard.Rank.C;
        }
        else
        {
            scoreBoard.ScoreRank = ScoreBoard.Rank.D;
        }

        if (actionRate == 1 && actionRate == 1 && accuracy == 100)
        {
            scoreBoard.ScoreRank = ScoreBoard.Rank.Best;
        }
        else if(actionRate >= 0.5f)
        {
            if(accuracy>=85)
                scoreBoard.ScoreRank = ScoreBoard.Rank.S;
            else if(accuracy>=75)
                scoreBoard.ScoreRank = ScoreBoard.Rank.A;
            else if(accuracy>=60)
                scoreBoard.ScoreRank = ScoreBoard.Rank.B;
        }
        else
        {
            if (accuracy >= 92)
                scoreBoard.ScoreRank = ScoreBoard.Rank.S;
            else if (accuracy >= 82)
                scoreBoard.ScoreRank = ScoreBoard.Rank.A;
            else if (accuracy >= 70)
                scoreBoard.ScoreRank = ScoreBoard.Rank.B;
        }

       



        Emblem.sprite = EmblemSprites[(int)scoreBoard.ScoreRank];
        Emblem.color = new Color(1, 1, 1, 0);

        RankBG.sprite = RankSprite[(int)scoreBoard.ScoreRank];
        RankBG.color = new Color(1, 1, 1, 0);

        string level = scoreBoard.Level;
        if (level.Contains("Easy"))
        {
            BaseBG.sprite = BaseSprite[0];
        }
        else if (level.Contains("Hard"))
        {
            BaseBG.sprite = BaseSprite[2];
        }
        else
        {
            BaseBG.sprite = BaseSprite[1];
        }

        for(int i = 0; i < scoreBoard.SwitchSum; i++)
        {
            GameObject dumy = Instantiate(Switch, SwitchParents, false);
            SwitchList.Add(dumy);
        }

        foreach(int i in scoreBoard.SwitchedIndex)
        {
            SwitchList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        AlbumArt.transform.SetAsLastSibling(); //1번 시블링으로 돌아와야함.
        AlbumArt.color = new Color(0, 0, 0, 1);
        Sprite spr;
        
        if (File.Exists(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + scoreBoard.MusicName + "/" + scoreBoard.MusicName + ".jpg"))
        {
            Texture2D texture;
            byte[] bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/ChartResources/NotesJson/" + scoreBoard.MusicName + "/" + scoreBoard.MusicName + ".jpg");
            if (bytes != null)
            {
                texture = new Texture2D(1920, 1080, TextureFormat.ARGB32, false);
                texture.LoadImage(bytes);
                spr = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
            }
            else
            {
                spr = null;
            }
        }
        else
        {
            spr = null;
        }
        if (spr != null)
        {
            AlbumArt.sprite = spr;
        }
        Buttons.SetActive(false);
        //여기서 컷씬로드 해줘야한다.
        
        StartCoroutine(Fader());

     
    }

    public void CutSceneFormatLoad()
    {
        string jsonString;
        string musicName = scoreBoard.MusicName;
        int nowChapter = 0;
        int wholeChpater = 0;
        jsonString = File.ReadAllText(CutSceneJson.path);
        cutSceneJson = JsonUtility.FromJson<CutSceneJson>(jsonString);


        Debug.Log(jsonString);
        jsonString = File.ReadAllText(Application.streamingAssetsPath + "/ChartResources/Format/MusicFormat.txt");
        MusicsFormat format = JsonUtility.FromJson<MusicsFormat>(jsonString);
        foreach (JustFormat justFormat in format.MusicList)
        {
            if (justFormat.MusicName == musicName)
            {
                nowChapter = justFormat.ChapterNum;//넘버가 뭔지 알았다.
            }
        }
        Debug.Log(nowChapter + "챕터번호");

        if (nowChapter == 1)
        {
            foreach (string str in cutSceneJson.chapter1List)
            {
                if (str == musicName)
                {
                    Debug.Log("존재");
                    return;
                }
            }
            if (cutSceneJson.chapter1List == null)
                cutSceneJson.chapter1List = new List<string>();
            cutSceneJson.chapter1List.Add(musicName);
            if (!cutSceneJson.chapter1Seen)
            {
                cutSceneJson.chapter1Seen = true;
                CutSceneLoad(1);
                return;
            }
        }
        else if (nowChapter == 2)
        {

            foreach (string str in cutSceneJson.chapter2List)
            {
                if (str == musicName)
                {
                    Debug.Log("존재");
                    return;
                }
            }
            if (cutSceneJson.chapter2List == null)
                cutSceneJson.chapter2List = new List<string>();
            cutSceneJson.chapter2List.Add(musicName);
            if (!cutSceneJson.chapter2Seen)
            {
                cutSceneJson.chapter2Seen = true;
                CutSceneLoad(2);
                return;
            }
        }

        wholeChpater = format.MusicList.Count;
        Debug.Log("여기까지는 하니");
        if (cutSceneJson.chapter1List.Count + cutSceneJson.chapter2List.Count == wholeChpater)
        {
            cutSceneJson.chapter3seen = false;
            CutSceneLoad(3);
        }


    }

    void CutSceneLoad(int chapter)
    {
        cutSceneBool = true;
        string JsonText = JsonUtility.ToJson(cutSceneJson, true);
        StreamWriter myFile = new StreamWriter(CutSceneJson.path);
        myFile.Write(JsonText);
        myFile.Close();
        Debug.Log("로드는 했다");
        LevelLoader.inst.CutSceneLoad(chapter,SpriteChangeManager.Way.MusicEnd);

    }

    IEnumerator Fader()
    {
        float t = 0;
        yield return new WaitForSeconds(0.5f);
        while (t < 1)
        {
            t += 0.01f;
            AlbumArt.color = new Color(t, t, t, 1);
            yield return new WaitForFixedUpdate();
        }
        while (t > 0.75)
        {
            t -= 0.01f;
            AlbumArt.color = new Color(t, t, t, t);
            yield return new WaitForFixedUpdate();
        }
        AlbumArt.transform.SetSiblingIndex(1);
        t = 0;
        yield return new WaitForSeconds(1f);
        while (t < 1)
        {
            t += 0.01f;
            RankBG.color = new Color(1, 1, 1, t);
            Emblem.color = new Color(1, 1, 1, t);
            yield return new WaitForFixedUpdate();
        }
        Buttons.SetActive(true);

    }

    void Save()
    {
        if(NameInput == null)
        {
            scoreBoard.ID = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }
        else
        {
            scoreBoard.ID = NameInput;
        }
        string JsonText = JsonUtility.ToJson(scoreBoard, true);
        if (!Directory.Exists(Application.streamingAssetsPath+"/ChartResources/ScoreBoard/" + scoreBoard.MusicName + "/" + scoreBoard.Level + "/"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath +"/ChartResources/ScoreBoard/" + scoreBoard.MusicName + "/" + scoreBoard.Level + "/");
        }
        StreamWriter myFile = new StreamWriter(Application.streamingAssetsPath+"/ChartResources/ScoreBoard/" + scoreBoard.MusicName + "/" + scoreBoard.Level + "/" + scoreBoard.ClearTime + ".txt");
        myFile.Write(JsonText);
        myFile.Close();
    }

    public void Replay()
    {
        LevelLoader.inst.Replay();
        Save();
    }

    public void ToPick()
    {
        LevelLoader.inst.LoadLevel("MusicPickScene", LevelLoader.SceneType.MusicPick);
        Save();
    }

    public void ToMenu()
    {
        LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
        Save();
    }

   

}
