using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static Vector2Int MapSize;
    private void Start()
    {
        GetMapSize();
    }

    public Button generateButton;
    public InputField inputX;
    public InputField inputY;
    public Dropdown tileset;

    public void GetMapSize()
    {
        int x = Convert.ToInt32(inputX.text);
        int y = Convert.ToInt32(inputY.text);
        if (x <= 0) x = 1;
        if (y <= 0) y = 1;
        x+=2;
        y+=2;
        MapSize = new Vector2Int(x, y);
    }
}
