using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MovePoint_UI : MonoBehaviour
{
    public Text text;
    public GridPath gp;

    // Update is called once per frame
    void Update()
    {
        if (gp.movePoint >= 1)
            text.text = $"Move Point: {gp.movePoint}";
        else
            text.text = $"End Turn. Turn begins {gp.turnTimer % 60}";
    }
}
