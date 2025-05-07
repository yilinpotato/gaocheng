using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseClickDetector : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite hoverSprite;
    public Sprite clickSprite;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = idleSprite;
    }

    void OnMouseEnter()
    {
        sr.sprite = hoverSprite;
        Debug.Log("鼠标进入");
    }

    void OnMouseExit()
    {
        sr.sprite = idleSprite;
        Debug.Log("鼠标离开");
    }

    void OnMouseDown()
    {
        sr.sprite = clickSprite;
        Debug.Log("鼠标点击");
    }
}