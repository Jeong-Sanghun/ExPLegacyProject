using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.IO;

public class NewSoundManager : MonoBehaviour
{
    public RhythmTool rhythmTool;
    public RhythmEventProvider rhythmEvent;
    //public NewMusicClass[] AllMusics;  //길이는 유니티의 inspector창에서 해준다. 좀 불안한 방식이긴 하다.
    public List<NewMusicClass> AllMusics;
    public AudioSource nowMusic;
    public float WholeTime = 0;

    /*foreach(T a in b)라고 하면, b배열의 길이만큼 구문을 실행한다. 여기서 a는 b안의 index가 된다.
      예를들면, AllMusics의 길이가 4이다. 총 노래가 4곡이라는 뜻이다. 그러면 music은 첫 번째 루프
      에서는 AllMusics[0], 두 번쨰 루프에서는 AllMusics[1],..이런식으로 진행된다. 그래서 모든 인덱
      스에 같은 구문을 적용하고자 할때, 지금처럼 초기화를 할 때 사용한다. 지금은 music의 AudioSource
      를 초기화 해주기 위해 사용한다.*/

    void Start()
    {
        rhythmTool.SongLoaded += OnSongLoaded;

        string[] files;

        /*
        if (!Directory.Exists(Application.streamingAssetsPath + "/SoundResources/Music/"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/SoundResources/Music/");
        }
        string path = Application.streamingAssetsPath + "/SoundResources/Music/";
        files = Directory.GetFiles(Application.streamingAssetsPath + "/SoundResources/Music");
        for(int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".meta"))
            {
                files[i].Remove(files[i].Length - 4, 4);//01234 5-3
                files[i].Remove(0, path.Length);
                Debug.Log(files[i]);
                LoadClip(files[i]);
            }
        }*/
    }

    public void MusicStop()
    {
        rhythmTool.Stop();
    }

    void LoadClip(string musicName)
    {
        StartCoroutine(ClipLoader(musicName));
    }

    IEnumerator ClipLoader(string name)
    {
        WWW request = GetAudioFromFile(name);
        yield return request;
        AudioClip clip = request.GetAudioClip(false,false,AudioType.UNKNOWN);
        NewMusicClass dumy = new NewMusicClass(name,clip);
        AllMusics.Add(dumy);
    }

    WWW GetAudioFromFile(string fileName)
    {
        string audioStr = Application.streamingAssetsPath + "/SoundResources/Music/" + fileName;
        WWW request = new WWW(audioStr);
        return request;
    }

    public void MusicStart(string name)//음악 이름 받아와준다
    {
        foreach (NewMusicClass music in AllMusics)
        {
            if (music.name == name)  //이름이 같으면 음악을 켜고, 루프를 탈출한다.
            {
                rhythmTool.audioClip = music.Clip;
                WholeTime = music.Clip.length;
                //rhythmTool.volume = PlayerPrefs.GetFloat("volume");   // 설정에서 저장한 볼륨 불러오기. 환우 수정
                rhythmTool.volume = 1;

                //rhythmTool.Play();
                break;
            }
        }
    }

    public void FastMode()
    {
        rhythmTool.pitch = Time.timeScale;
        //WholeTime *= Time.timeScale;
    }

    private void OnSongLoaded()
    {
        rhythmTool.Play();
        //이부분에 모든 곳의 gamestart를 켜야돼.
    }

    /*
    public void MusicStart(string name)//음악 이름 받아와준다
    {
        if (name == "tick")
        {
            AllMusics[2].Source.Play();
            return;
        }
        foreach (MusicClass music in AllMusics)
        {
            if (music.name == name)  //이름이 같으면 음악을 켜고, 루프를 탈출한다.
            {

                rhythmTool.audioClip = music.Clip;
                rhythmTool.volume = music.volume;
                rhythmTool.SongLoaded += OnSongLoaded;
                break;
            }
        }
    }*/
}
