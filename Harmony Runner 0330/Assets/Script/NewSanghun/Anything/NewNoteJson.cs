using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewNoteJson
{
    public int routes;
    public float NoteVelocity;
    public string MusicName;
    public string Level;
    public int IntLevel;
    public int Chapter;
    public float BPM;
    public List<int> RoutingIndexFirst;
    public List<int> RoutingIndexSecond;
    public List<int> ActionIndexFirst;
    public List<int> ActionIndexSecond;
    //분기 했을 때 얼마나 건너뛰어야할 지 알려준다.
    public List<NewNoteWrapper> NoteList;
    //요거 래퍼로 바꿔야한다 190201
    public List<MouseNoteWrapper> ActionList;
    //액션노트 쳐야되니까 이렇게 한다.

}
