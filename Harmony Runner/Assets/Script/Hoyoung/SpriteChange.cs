using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChange : MonoBehaviour {

    //public Sprite CurrentSprite;
    //public Sprite NextSprite;
    private SpriteRenderer upSpriteRenderer;
    public GameObject Up;
    private SpriteRenderer downSpriteRenderer;
    public GameObject Down;
    private SpriteRenderer leftSpriteRenderer;
    public GameObject Left;
    private SpriteRenderer rightSpriteRenderer;
    public GameObject Right;

    public Sprite CurrentUpSprite;
    public Sprite NextUpSprite;
    public Sprite CurrentDownSprite;
    public Sprite NextDownSprite;
    public Sprite CurrentLeftSprite;
    public Sprite NextLeftSprite;
    public Sprite CurrentRightSprite;
    public Sprite NextRightSprite;


    // Use this for initialization
    void Start () {
        upSpriteRenderer = Up.GetComponent<SpriteRenderer>();
        upSpriteRenderer.sprite = CurrentUpSprite;
        downSpriteRenderer = Down.GetComponent<SpriteRenderer>();
        downSpriteRenderer.sprite = CurrentDownSprite;
        leftSpriteRenderer = Left.GetComponent<SpriteRenderer>();
        leftSpriteRenderer.sprite = CurrentLeftSprite;
        rightSpriteRenderer = Right.GetComponent<SpriteRenderer>();
        rightSpriteRenderer.sprite = CurrentRightSprite;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            upSpriteRenderer.sprite = NextUpSprite;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            downSpriteRenderer.sprite = NextDownSprite;
            
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rightSpriteRenderer.sprite = NextRightSprite;
        }
        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            leftSpriteRenderer.sprite = NextLeftSprite;
        }
	}
}
