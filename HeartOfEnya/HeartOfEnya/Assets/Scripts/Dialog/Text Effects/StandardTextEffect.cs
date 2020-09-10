using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;


/// <summary>
/// Text effect for passthrough of non-custom text effect tags (bold, italics, color)
/// </summary>
public class StandardTextEffect : TextEffect
{
    public string startTag;// The complete start tag, including braces
    public string endTag;// the complete end tag, including braces

    private static readonly Regex colorRegex = new Regex(@"<color=(.*)>");

    public void SetValues(string parsingTag, string startTag, int start, List<CustomColor> customColors)
    {
        this.parsingTag = parsingTag.Split('=')[0];
        this.startTag = startTag;

        Match colorMatch = colorRegex.Match(startTag);
        foreach (CustomColor customColor in customColors)
        {
            if(startTag.Contains("<color=" + customColor.name + ">"))
            {
                this.startTag = "<#" + ColorUtility.ToHtmlStringRGB(customColor.color) + ">";
                break;
            }
        }

        this.start = start;
    }
}
