//This script is used to select the correct sprite for the room based on the doors that are present. It also changes the color of the sprite based on the type of room (normal or enter). The script uses a series of if statements to determine which sprite to use based on the boolean values of up, down, left, and right.
////It also uses a switch statement to determine which color to use based on the type of room.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{

    public Sprite spU, spD, spR, spL,
            spUD, spRL, spUR, spUL, spDR, spDL,
            spULD, spRUL, spDRU, spLDR, spUDRL;
    public bool up, down, left, right;
    public int type; // 0: normal, 1: enter
    public Color normalColor, enterColor;
    Color mainColor;
    SpriteRenderer rend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        mainColor = normalColor;
        PickSprite();
        PickColor();
    }
    void PickSprite()
    {
        string doors = (up ? "U" : "") + (down ? "D" : "") + (left ? "L" : "") + (right ? "R" : "");

        switch (doors)
        {
            case "UDLR":
                rend.sprite = spUDRL;
                break;
            case "UDL":
                rend.sprite = spULD;
                break;
            case "UDR":
                rend.sprite = spDRU;
                break;
            case "UD":
                rend.sprite = spUD;
                break;
            case "ULR":
                rend.sprite = spRUL;
                break;
            case "UL":
                rend.sprite = spUL;
                break;
            case "UR":
                rend.sprite = spUR;
                break;
            case "U":
                rend.sprite = spU;
                break;
            case "DLR":
                rend.sprite = spLDR;
                break;
            case "DL":
                rend.sprite = spDL;
                break;
            case "DR":
                rend.sprite = spDR;
                break;
            case "D":
                rend.sprite = spD;
                break;
            case "LR":
                rend.sprite = spRL;
                break;
            case "L":
                rend.sprite = spL;
                break;
            case "R":
                rend.sprite = spR;
                break;
            default:
                Debug.LogError("Invalid door combination");
                break;
        }
    }

    void PickColor()
    {
        mainColor = type switch
        {
            0 => normalColor,
            1 => enterColor,
            _ => throw new System.ArgumentException("Invalid room type")
        };
        rend.color = mainColor;
    }
}