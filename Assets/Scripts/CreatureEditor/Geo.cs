using System;
using System.Collections.Generic;
using UnityEngine;

public class Geo
{
    public static Dictionary<GeoTypes, string> GeoDescDict = new Dictionary<GeoTypes, string>
    {
        {GeoTypes.Circle, "Vision"},
        {GeoTypes.Square, "Speed"},
        {GeoTypes.Triangle, "Poisonous"},
        {GeoTypes.Hexagon, "Life"},
    };

    public static Dictionary<GeoTypes, string> GeoColorDict = new Dictionary<GeoTypes, string>
    {
        {GeoTypes.Circle, "#FF1900"},
        {GeoTypes.Square, "#004FFF"},
        {GeoTypes.Triangle, "#FFEB00"},
        {GeoTypes.Hexagon, "#51FF00"},
    };
}

public enum GeoTypes
{
    Circle = 0,
    Square = 1,
    Triangle = 2,
    Hexagon = 3,
}