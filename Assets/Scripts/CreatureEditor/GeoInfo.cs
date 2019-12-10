using System;
using System.Xml;
using UnityEngine;

public class GeoInfo
{
    public GeoTypes GeoType;
    public Vector2 Position;
    public Quaternion Rotation;
    public Color Color;
    public Vector2 Size;
    public int SortingOrder;

    public GeoInfo Clone()
    {
        GeoInfo newGeoInfo = new GeoInfo();
        newGeoInfo.GeoType = GeoType;
        newGeoInfo.Position = Position;
        newGeoInfo.Rotation = Rotation;
        newGeoInfo.Color = Color;
        newGeoInfo.Size = Size;
        newGeoInfo.SortingOrder = SortingOrder;
        return newGeoInfo;
    }

    public void ExportToXML(XmlElement geoInfosElement)
    {
        geoInfosElement.SetAttribute("GeoType", GeoType.ToString());
        geoInfosElement.SetAttribute("Position", string.Format("{0},{1}", Position.x, Position.y));
        geoInfosElement.SetAttribute("Rotation", string.Format("{0},{1},{2},{3}", Rotation.x, Rotation.y, Rotation.z, Rotation.w));
        geoInfosElement.SetAttribute("Color", string.Format("{0},{1},{2},{3}", Color.r, Color.g, Color.b, Color.a));
        geoInfosElement.SetAttribute("Size", string.Format("{0},{1}", Size.x, Size.y));
        geoInfosElement.SetAttribute("SortingOrder", SortingOrder.ToString());
    }

    public static GeoInfo GenerateGeoInfoFromXML(XmlNode geo_ele)
    {
        GeoInfo geoInfo = new GeoInfo();
        geoInfo.GeoType = (GeoTypes) Enum.Parse(typeof(GeoTypes), geo_ele.Attributes["GeoType"].Value);

        string[] Position_Str = geo_ele.Attributes["Position"].Value.Split(',');
        float[] Position_values = new float[Position_Str.Length];
        for (int i = 0; i < Position_Str.Length; i++)
        {
            Position_values[i] = float.Parse(Position_Str[i]);
        }

        geoInfo.Position = new Vector2(Position_values[0], Position_values[1]);

        string[] Rotation_Str = geo_ele.Attributes["Rotation"].Value.Split(',');
        float[] Rotation_values = new float[Rotation_Str.Length];
        for (int i = 0; i < Rotation_Str.Length; i++)
        {
            Rotation_values[i] = float.Parse(Rotation_Str[i]);
        }

        geoInfo.Rotation = new Quaternion(Rotation_values[0], Rotation_values[1], Rotation_values[2], Rotation_values[3]);

        string[] Color_Str = geo_ele.Attributes["Color"].Value.Split(',');
        float[] Color_values = new float[Color_Str.Length];
        for (int i = 0; i < Color_Str.Length; i++)
        {
            Color_values[i] = float.Parse(Color_Str[i]);
        }

        geoInfo.Color = new Color(Color_values[0], Color_values[1], Color_values[2], Color_values[3]);

        string[] Size_Str = geo_ele.Attributes["Size"].Value.Split(',');
        float[] Size_values = new float[Size_Str.Length];
        for (int i = 0; i < Size_Str.Length; i++)
        {
            Size_values[i] = float.Parse(Size_Str[i]);
        }

        geoInfo.Size = new Vector2(Size_values[0], Size_values[1]);
        geoInfo.SortingOrder = int.Parse(geo_ele.Attributes["SortingOrder"].Value);

        return geoInfo;
    }
}