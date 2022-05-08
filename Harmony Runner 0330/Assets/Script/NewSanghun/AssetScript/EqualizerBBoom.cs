using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualizerBBoom : MonoBehaviour {

    public RhythmEventProvider rhythmEvent;
    //low mid high beat quarterbeat all
    //linemask의 x간격은 0.23
    //mask들 사이의 y간격은 0.33
    GameObject[] children;
    Vector2 standard;
    float[] LerpT = new float[6];
    bool[] OnMotion = new bool[6];

    private void Start()
    {
        rhythmEvent.Onset += OnSet;
        rhythmEvent.Beat += OnBeat;
        rhythmEvent.SubBeat += OnQuarterBeat;
        children = new GameObject[6];
        for(int i = 0; i < 6; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            OnMotion[i] = false;
        }
        standard = new Vector2(1, 1);
    }

    void OnSet(OnsetType type,Onset onset)
    {

        if (onset.rank < 4 && onset.strength < 5)
            return;

        switch (type)
        {
            case OnsetType.Low:
                if (!OnMotion[0])
                    StartCoroutine(MotionCoroutine(0, onset.strength));
                break;
            case OnsetType.Mid:
                if (!OnMotion[1])
                    StartCoroutine(MotionCoroutine(1, onset.strength));
                break;
            case OnsetType.High:
                if (!OnMotion[2])
                    StartCoroutine(MotionCoroutine(2, onset.strength));
                break;
            case OnsetType.All:
                if (!OnMotion[3])
                    StartCoroutine(MotionCoroutine(3, onset.strength));
                break;
        }
    }

    IEnumerator MotionCoroutine(int index, float strength)
    {
        //index : low mid high all beat subbeat
        OnMotion[index] = true;
        while (LerpT[index] < 1)
        {
            children[index].transform.localScale = Vector2.Lerp(standard, new Vector2(strength, 1), LerpT[index]);
            LerpT[index] += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        while (LerpT[index] > 0)
        {
            children[index].transform.localScale = Vector2.Lerp(standard, new Vector2(strength, 1), LerpT[index]);
            LerpT[index] -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        LerpT[index] = 0;
        OnMotion[index] = false;
       
    }

    void OnBeat(Beat beat)
    {
        if (!OnMotion[4])
            StartCoroutine(MotionCoroutine(4,12f));
    }
    
    void OnQuarterBeat(Beat beat, int num)
    {
        if (!OnMotion[5])
            StartCoroutine(MotionCoroutine(5, 12f));
    }
}
