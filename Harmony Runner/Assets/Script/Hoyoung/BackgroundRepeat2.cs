using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경 이동 (노을)

public class BackgroundRepeat2 : MonoBehaviour {

    public float scroll; // 0.003 적당 혹은 조금 더 빠르게 가능 0.0035?

    private Material thisMaterial;

    // Use this for initialization
    void Start()
    {
        thisMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newOffset = thisMaterial.mainTextureOffset;

        newOffset.Set(0, newOffset.y - (scroll * Time.deltaTime));

        thisMaterial.mainTextureOffset = newOffset;
    }
}
