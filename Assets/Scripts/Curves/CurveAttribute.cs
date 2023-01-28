using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveAttribute : PropertyAttribute
{
    public float PosX, PosY;
    public float RangeX, RangeY;
    public Color Colour;

    public CurveAttribute(float minX, float maxX, float minY, float maxY)
    {
        PosX = minX;
        PosY = minY;
        RangeX = Mathf.Abs(minX) + maxX;
        RangeY = Mathf.Abs(minY) + maxY;
        Colour = Color.green;
    }

    public CurveAttribute(float minX, float maxX, float minY, float maxY, Color colour)
    {
        PosX = minX;
        PosY = minY;
        RangeX = Mathf.Abs(minX) + maxX;
        RangeY = Mathf.Abs(minY) + maxY;
        Colour = colour;
    }
}
