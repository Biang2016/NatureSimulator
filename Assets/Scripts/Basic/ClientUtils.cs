using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ClientUtils
{
    public static string GetPlatformAbbr()
    {
        string res = "";
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            {
                res = "osx";
                break;
            }
            case RuntimePlatform.OSXEditor:
            {
                res = "osx";
                break;
            }
            case RuntimePlatform.WindowsPlayer:
            {
                res = "windows";
                break;
            }
            case RuntimePlatform.WindowsEditor:
            {
                res = "windows";
                break;
            }
        }

        return res;
    }

    public static string ReplaceWrapSpace(string src)
    {
        src = src.Replace(" ", "\u00A0");
        return src;
    }

    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    public static void ChangeColor(RawImage image, Color newColor)
    {
        if (!image) return;
        image.color = newColor;
    }

    public static void ChangeColor(Image image, Color newColor)
    {
        if (!image) return;
        image.color = newColor;
    }

    public static void ChangeColor(Renderer rd, Color newColor, float intensity = 1.0f)
    {
        if (!rd) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", newColor);
        mpb.SetColor("_EmissionColor", newColor * intensity);
        rd.SetPropertyBlock(mpb);
    }

    public static void ChangeEmissionColor(Renderer rd, Color newColor, float intensity = 1.0f)
    {
        if (!rd) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetColor("_EmissionColor", newColor * intensity);
        rd.SetPropertyBlock(mpb);
    }

    public static Color ChangeColorToWhite(Color color, float whiteRatio)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, g, b);

        if (max - r < 0.2f && max - g < 0.2f && max - b < 0.2f) //本来就是灰色
        {
            max = max + 0.3f;
            Color res = Color.Lerp(color, new Color(max, max, max, color.a), 1f);
            return res;
        }
        else
        {
            max = max + 0.3f;
            Color res = Color.Lerp(color, new Color(max, max, max, color.a), whiteRatio);
            return res;
        }
    }

    public static Color HSL_2_RGB(float H, float S, float L)
    {
        //H, S and L input range = 0 ÷ 1.0
        //R, G and B output range = 0 ÷ 255
        float R;
        float G;
        float B;
        if (S.Equals(0))
        {
            R = L;
            G = L;
            B = L;
        }
        else
        {
            float var_1 = 0;
            float var_2 = 0;
            if (L < 0.5)
            {
                var_2 = L * (1 + S);
            }
            else
            {
                var_2 = (L + S) - (S * L);
            }

            var_1 = 2 * L - var_2;

            R = Hue_2_RGB(var_1, var_2, H + (1.0f / 3));
            G = Hue_2_RGB(var_1, var_2, H);
            B = Hue_2_RGB(var_1, var_2, H - (1.0f / 3));
        }

        return new Color(R, G, B);
    }

    static float Hue_2_RGB(float v1, float v2, float vH) //Function Hue_2_RGB
    {
        if (vH < 0) vH += 1;
        if (vH > 1) vH -= 1;
        if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
        if ((2 * vH) < 1) return (v2);
        if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2.0f / 3.0f) - vH) * 6);
        return v1;
    }

    /// <summary>
    /// 查找子节点对象
    /// 内部使用“递归算法”
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">查找的子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject goParent, string childName)
    {
        Transform searchTrans = null; //查找结果

        searchTrans = goParent.transform.Find(childName);
        if (searchTrans == null)
        {
            foreach (Transform trans in goParent.transform)
            {
                searchTrans = FindTheChildNode(trans.gameObject, childName);
                if (searchTrans != null)
                {
                    return searchTrans;
                }
            }
        }

        return searchTrans;
    }

    /// <summary>
    /// 给子节点添加父对象
    /// </summary>
    /// <param name="parents">父对象的方位</param>
    /// <param name="child">子对象的方法</param>
    public static void AddChildNodeToParentNode(Transform parents, Transform child)
    {
        child.SetParent(parents, false);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localEulerAngles = Vector3.zero;
    }

    public static IEnumerator UpdateLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return null;
    }
}