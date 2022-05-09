using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpVibe : MonoBehaviour {

    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform HpTransform;

    // How long the object should shake for.
    public float shake = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector2 originalPos;
    
    int[] point = {30,-29,28,-27,26,-25,24,-23,22,-21,20,-19,18,-17,16,-15,14,-13,12,-11,10,-9,8,-7,6,-5,4,-3,2,-1,0 ,0,0,0,0};
    int n = 0;
    Vector2 Point;

    void Awake()
    {
        if (HpTransform == null)
        {
            HpTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = HpTransform.localPosition;
        
    }

    void Update()
    {
        if (shake > 0)
        {
            
            if (n < 32)
            {
                
                Point = new Vector2(point[n], 0);
                //Debug.Log(point[n]);
                HpTransform.localPosition = originalPos + Point * shakeAmount; // 랜덤 Random.insideUnitCircle
                n = n + 1;
                //Debug.Log(HpTransform.localPosition);
            }
            else
            {
                n = 0;
                shake = 0f;
            }
            

            shake -= Time.deltaTime * decreaseFactor;
            //HpTransform.localPosition = new Vector3(1130.2f, 1387.52f, 0);
            

        }
        else
        {
            shake = 0f;
            HpTransform.localPosition = originalPos;
        }
    }
}


