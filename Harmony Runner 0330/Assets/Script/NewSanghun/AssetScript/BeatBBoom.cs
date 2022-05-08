using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBBoom : MonoBehaviour {

    public RhythmTool rhythmTool;
    public RhythmEventProvider rhythmEvent;
    //gameObject의 size조절, Opaque조절..
    Vector3[] vector;
    float t = 0;
    float BeatLength;
    bool InMotion = false;


    int myFrame = 0;
   //ReadOnlyDictionary<int, Beat> myBeats = new ReadOnlyDictionary<int, Beat>();
   //몇번쨰 비트인지 알려주는건가봐.


    public void Start()
    {
        vector = new Vector3[2];
        vector[0] = new Vector3(1f, 1f, 1f);
        vector[1] = new Vector3(2f, 2f, 2f);
        rhythmEvent.Beat += CheckerChange;
        rhythmEvent.SongEnded += OnMusicEnd;
    }

    void CheckerChange(Beat obj)
    {
        //비트에 맞게 그거 뿜뿜
        t = 0;
        BeatLength = rhythmTool.beatLength;
        if (!InMotion)
        {
            StartCoroutine(Motion());
        }
    }

    IEnumerator Motion()
    {
        InMotion = true;
        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(vector[0], vector[1], t);
            t += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        InMotion = false;

    }

    public void OnMusicEnd()
    {
        rhythmEvent.Beat -= CheckerChange;
    }



}
