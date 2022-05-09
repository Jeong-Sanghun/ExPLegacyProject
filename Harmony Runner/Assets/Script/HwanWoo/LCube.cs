using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCube : Gallery {      // 갤러리를 상속받아서 같이 쓰도록 한다.

    void OnMouseEnter()
    {
        mouseon = true;
        Debug.Log("성공적");
    }

    void OnMouseOver()
    {
        if (mouseon && delay)
        {
            StartCoroutine(Back());
        }
    }

    void OnMouseExit()
    {
        mouseon = false;
        StopCoroutine(Back());
    }

    IEnumerator Back()
    {
        delay = false;
        index--;
        if (index == -1)
            index = 5;
        Debug.Log(index);
        yield return new WaitForSeconds(1.0f);
        delay = true;
        //Book = Page[index];   // 오른쪽을 누를때마다 한페이지씩 넘기기
        Debug.Log("1초지남");
    }
}
