using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net.NetworkInformation;

public static class Utils
{
    public static string GetMacAddress()
    {
        string physicalAddress = "";

        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface adapter in nice)
        {
            if (adapter.Description == "en0")
            {
                physicalAddress = adapter.GetPhysicalAddress().ToString();
                break;
            }
            else
            {
                physicalAddress = adapter.GetPhysicalAddress().ToString();

                if (physicalAddress != "")
                {
                    break;
                }

                ;
            }
        }

        return physicalAddress;
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void ScrollToTop(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
}