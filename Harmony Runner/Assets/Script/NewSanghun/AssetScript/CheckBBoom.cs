using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBBoom : MonoBehaviour {


    //gameObject의 size조절, Opaque조절..
    Vector3[] vector;
    float t = 0;
    bool InMotion = false;

    AudioSource audioSource;
    int myFrame = 0;
    //ReadOnlyDictionary<int, Beat> myBeats = new ReadOnlyDictionary<int, Beat>();
    //몇번쨰 비트인지 알려주는건가봐.


    public void Start()
    {
        vector = new Vector3[2];
        vector[0] = new Vector3(1.130042f, 1.130042f, 1.130042f);
        vector[1] = new Vector3(1.32f, 1.32f, 1.32f);
    }

    public void CheckerChange()
    {
        //비트에 맞게 그거 뿜뿜
        if (!InMotion)
        {
            t = 0;
            StartCoroutine(Motion());
        }
    }

    IEnumerator Motion()
    {
        InMotion = true;
        while (t < 1)
        {
            transform.localScale = Vector3.Lerp(vector[0], vector[1], t);
            t += 0.072f;
            yield return new WaitForSeconds(0.01f);
        }
        //t가 1아래일 때.
        while (t >0)
        {
            transform.localScale = Vector3.Lerp(vector[0], vector[1], t);
            t -= 0.072f;
            yield return new WaitForSeconds(0.01f);
        }
        transform.localScale = vector[0];
        InMotion = false;

    }
}
