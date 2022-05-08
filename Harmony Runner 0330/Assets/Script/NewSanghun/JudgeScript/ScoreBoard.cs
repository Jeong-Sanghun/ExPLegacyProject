using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScoreBoard
{
    public enum Rank
    {
        Best,S,A,B,C,D
    }
    public string ID;

    public string MusicName;
    public string Level;
    public int LevelInt;

    public int PerfectHit;
    public int GoodHit;
    public int MissHit;

    public int WholeCombo;
    public int LongestCombo;
    public float LongestComboPer;
    public int WholeAction;
    public int Action;
    public int SwitchSum;
    public List<int> SwitchedIndex;
    public float Accuracy;
    public int BaseScore;
    public float Multiplier;
    public int TotalScore;

    public Rank ScoreRank;

    /*
    public int Year;
    public int Month;
    public int Date;
    public int Hour;
    public int Minute;
    public int Second;
    */
    public string ClearTime;

    public ScoreBoard()
    {
        SwitchedIndex = new List<int>();
        PerfectHit = 0;
        GoodHit = 0;
        MissHit = 0;

        WholeCombo = 0;
        LongestCombo = 0;
        LongestComboPer = 0;
        Action = 0;
        WholeAction = 0;
        SwitchSum = 0;

        Accuracy = 0;
        BaseScore = 0;
        Multiplier = 1;
        TotalScore = 0;
    }
    public void SetTime()
    {
        ClearTime = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
       // ID = SystemInfo.deviceName;
    }

    

}
