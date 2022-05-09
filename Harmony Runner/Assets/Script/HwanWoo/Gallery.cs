using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gallery : MonoBehaviour
{
    public GameObject[] Page;
    public GameObject Book;
    static public bool mouseon = false;
    static public bool delay = true;
    static public int index = 0;

    void OnMouseEnter()
    {   
        mouseon = true;
        Debug.Log("성공적");
    }

    void OnMouseOver()
    {
        if (mouseon && delay)
        {
            StartCoroutine(Next());
        }
    }

    void OnMouseExit()
    {
        mouseon = false;
        StopCoroutine(Next());
    }

    IEnumerator Next()
    {
        delay = false;
        index++;
        if (index == 6)
            index = 0;
        Debug.Log(index);
        yield return new WaitForSeconds(1.0f);
        delay = true;
        //Book = Page[index];   // 오른쪽을 누를때마다 한페이지씩 넘기기
        Debug.Log("1초지남");
    }
}
