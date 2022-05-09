using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditManager : MonoBehaviour {

    public Sprite[] spriteArr;
    public Image spriteRenderer;
    public GameObject textObject;
    int nowIndex;
    int spriteCount;
    float opacity = 0;
    //-2000에서 시작, 2000까지 갔다가 2000넘어가면 다시 -2000

	// Use this for initialization
	void Start () {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        spriteArr = Resources.LoadAll<Sprite>("CutScene/");
        spriteCount = spriteArr.Length;
        StartCoroutine(CreditCor());
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelLoader.inst.LoadLevel("StartScene", LevelLoader.SceneType.Menu);
        }

        textObject.transform.localPosition += Vector3.up * Time.deltaTime * 200;
        if (textObject.transform.localPosition.y > 2000)
        {
            textObject.transform.localPosition = new Vector3(0, -2000, 0);
        }
    }

    IEnumerator CreditCor()
    {
        while (true)
        {
            nowIndex = Random.Range(0, spriteCount - 1);
            yield return new WaitForSeconds(1f);
            while (opacity >= 0)
            {
                opacity -= Time.deltaTime * 2;
                spriteRenderer.color = new Color(1, 1, 1, opacity);
                yield return null;
            }
            opacity = 0;
            spriteRenderer.color = new Color(1, 1, 1, opacity);
            spriteRenderer.sprite = spriteArr[nowIndex];
            while (opacity <= 0.5f)
            {
                opacity += Time.deltaTime * 2;
                spriteRenderer.color = new Color(1, 1, 1, opacity);
                yield return null;
            }
            opacity = 0.5f;
            spriteRenderer.color = new Color(1, 1, 1, opacity);
        }
     
    }
}
