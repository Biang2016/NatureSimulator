using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeoInfo
{
    public GeoTypes GeoType;
    public Vector2 Position;
    public Quaternion Rotation;
    public Color Color;
    public Vector2 Size;
    public int SortingOrder;
    public List<string> Diet = new List<string>();
}