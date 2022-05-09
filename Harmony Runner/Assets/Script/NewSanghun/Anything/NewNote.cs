using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewNote{

    public int NoteTime;
    public char NoteType;
    public float LongNoteLength;
    public GameObject EditNote;
    public static NoteMakerUIManager uiManager;

    public NewNote(int a, char b, float c)
    {
        NoteTime = a;
        NoteType = b;
        LongNoteLength = c;
        EditNote = null;
        uiManager.changeNotes();
    }


    public NewNote(int a, char b, float c, ref GameObject myNote)
    {
        NoteTime = a;
        NoteType = b;
        LongNoteLength = c;
        EditNote = myNote;
        uiManager.changeNotes();
    }
}
