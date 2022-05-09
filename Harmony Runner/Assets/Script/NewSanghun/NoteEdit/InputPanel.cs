using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour {

    //패널의 컴포넌트로 들어간다.
    //버튼이 눌렸을 떄 패널을 왼쪽으로 밀어주는 스크립트,19.01.10

    public GameObject MovingPanel;
    //패널의 컴포넌트이기 때문에, gameobject가 된다.
    public float MoveLength;
    //사용 패널이 여러개일 수 있기 때문에, 하드코딩 하지않는다.
    bool ToggleIn = true;
    //들어가있으면 true
    bool Moving = false;
    //움직이는 도중에 버튼누르면 안된다아

    // Use this for initialization
    void Start () {
        MovingPanel = gameObject;
        //간단 초기화.19.01.10
	}

    public void OnButton()
    {
        if (!Moving)
        {
            StartCoroutine(moveCoroutine());
            ToggleIn = !ToggleIn;
        }

    }

    //이동은 lerp를 쓴다
    IEnumerator moveCoroutine()
    {
        Vector3 pos;
        Vector3 des;
        float lerp=0;
        Moving = true;
        if (ToggleIn)
        {
            pos = transform.position;
            des = new Vector3(pos.x - MoveLength, pos.y, pos.z);
        }
        else
        {
            pos = transform.position;
            des = new Vector3(pos.x + MoveLength, pos.y, pos.z);
            //나와있으면 length를 더해준다.19.01.10
            //나와있으면 오른쪽으로 가야하니까 더해줘야한다.
        }
        while (lerp<=1)
        {
            lerp += 0.05f;
            MovingPanel.transform.position = Vector3.Lerp(pos, des, lerp);
            yield return new WaitForFixedUpdate();
        }
        Moving = false;
    }
}
