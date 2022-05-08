using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JustFormat {

    public string MusicName;
    public int ChapterNum;
    public List<string> LevelStringList;
    public List<int> LevelIntList;

    public JustFormat(string name)
    {
        MusicName = name;
        LevelStringList = new List<string>();
        LevelIntList = new List<int>();
    }
}
