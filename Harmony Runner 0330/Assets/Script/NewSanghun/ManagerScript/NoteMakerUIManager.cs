using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteMakerUIManager : MonoBehaviour {

    public NewJsonManager jsonManager;
    public GameObject textCanvas;
    public Text BPM;
    public Text Notes;
    public RhythmTool rhythmTool;

    int nowNotes = 0;
    bool textOn = true;

    //다 에디터에서 넣어준다.
    //time은 json에서 관리해준다, 여기서해줄거는 bpm하고 현재 노트 개수.

    public void OnButton()
    {
        textOn = !textOn;
        textCanvas.SetActive(textOn);
    }

    public void uiSetup()
    {   
        StartCoroutine(BPMCoroutine());             
        Notes.text = "0";
        BPM.text = "0";
    }

    public void changeNotes(int quant)
    {
        nowNotes = quant;
        Notes.text = quant.ToString();
    }

    public void changeNotes()
    {
        ++nowNotes;
        Notes.text = nowNotes.ToString();
    }

    public void removeNotes()
    {
        nowNotes--;
        Notes.text = nowNotes.ToString();
    }

    IEnumerator BPMCoroutine()
    {
        while (true)
        {
            BPM.text = rhythmTool.bpm.ToString("N1");
            yield return new WaitForSeconds(1.0f);
        }

    }


}
