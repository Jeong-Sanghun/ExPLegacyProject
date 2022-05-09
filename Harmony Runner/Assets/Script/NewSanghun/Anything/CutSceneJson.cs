using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutSceneJson {

    public bool introSeen;
    public bool chapter1Seen;
    public bool chapter2Seen;
    public bool chapter3seen;

    public List<string> chapter1List;
    public List<string> chapter2List;

    public static string path;

}
