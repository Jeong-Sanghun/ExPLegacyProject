using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicPickManager : MonoBehaviour {

    public bool RandomMode { get; set; }
    public bool RunnersMode { get; set; }
    public bool DarkMode { get; set; }
    public bool FastMode { get; set; }
    public bool PerfectMode { get; set; }

    public string MusicName { get; set; }
    public GameObject ButtonPrefab;
    public GameObject ScrollContent;
    public GameObject[] ButtonObjectArray;
    public Button[] ButtonScriptArray;
    string NowMusic;
    List<string> MusicNames;
    float ContentHeight;

    public Scrollbar Scroll;

    public NewNoteJson[] JsonNote;
    public TextAsset[] JsonTextAssets;
    public string[] JsonRawString;

    public NewSoundManager soundManager;
    public LevelLoader levelLoader;

    public void StartButton()
    {

    }

    public void LoadMusic()
    {
        int musicCount;
        MusicNames = new List<string>();
        JsonTextAssets = Resources.LoadAll<TextAsset>("ChartResources/NotesJson/");
        JsonRawString = new string[JsonTextAssets.Length];
        JsonNote = new NewNoteJson[JsonTextAssets.Length];
        
        for(int i = 0; i < JsonTextAssets.Length; i++)
        {
            string name;
            JsonRawString[i] = JsonTextAssets[i].text;
            JsonNote[i] = JsonUtility.FromJson<NewNoteJson>(JsonRawString[i]);
            name = JsonNote[i].MusicName;
            if (!MusicNames.Contains(name))
            {
                MusicNames.Add(name);
            }
        }

        musicCount = MusicNames.Count;
        ButtonObjectArray = new GameObject[musicCount];
        ButtonScriptArray = new Button[musicCount];
        RectTransform scrollRect = ScrollContent.GetComponent<RectTransform>();
        ContentHeight = 1110 + 370 * (musicCount-1);
        scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, ContentHeight);
        //버튼의 시작위치는 -555이다. 중앙이고, 앵커는 맨 위로 박혀있어야한다.

        for(int i = 0; i < MusicNames.Count; i++)
        {
            RectTransform rect;
            ButtonObjectArray[i] = Instantiate(ButtonPrefab, ScrollContent.transform, false);
            ButtonScriptArray[i] = ButtonObjectArray[i].GetComponent<Button>();
            rect = ButtonObjectArray[i].GetComponent<RectTransform>();
           
            rect.anchoredPosition = new Vector2(0, -555-370*i);

            ButtonScriptArray[i].onClick.AddListener(delegate { OnButton(MusicNames[i],i); } );
        }
    }

    public void OnButton(string name, int index)
    {
        soundManager.MusicStart(name);

    }


}
