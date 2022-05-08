using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   //요게 있으면 inspector창에서 MusicClass의 요소를 건들 수 있다.
public class NewMusicClass {
    public string name;     //곡의 이름
    public float volume;    //곡의 볼륨
    public float BPM;       //곡의 빠르기,Beat per minute

    public AudioClip Clip;     //AudioClip클래스형 변수를 선언해준다.

    public NewMusicClass(string Name, AudioClip clip)
    {
        name = Name;
        Clip = clip;
        volume = 1;
        BPM = 0;
    }

    /*왜 클립하고 source하고 따로냐면, 클립은 파일 단 하나만 가져오고
      다른 함수가 없는데 source는 play, stop기능이달려있다*/
}
