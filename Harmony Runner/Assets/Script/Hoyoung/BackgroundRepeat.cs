using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//배경 반복 이동

public class BackgroundRepeat : MonoBehaviour
{
    [Range(0, 1)]
    public float scrollSpeed;
    //스크롤할 속도를 지정
    private Material thisMaterial;
    //Quad의 Material 데이터를 받아올 객체를 선언

    void Start()
    {

        thisMaterial = GetComponent<Renderer>().material;
        //현재 객체의 Component들을 참조해 Renderer라는 컴포넌트의 Material정보를 받아옴
    }

    void Update()
    {
        Vector2 newOffset = thisMaterial.mainTextureOffset;
        // 새롭게 지정해줄 OffSet 객체를 선언

        newOffset.Set(newOffset.x + (scrollSpeed * Time.deltaTime), 0);
        // X부분에 현재 x값의 속도에 프레임 보정을 해서 더해줌

        thisMaterial.mainTextureOffset = newOffset;
        //그리고 최종적으로 Offset값을 지정
        
    }
}
