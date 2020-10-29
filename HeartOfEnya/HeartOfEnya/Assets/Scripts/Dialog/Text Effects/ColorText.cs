using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System;
using FMOD;

public class ColorText : TextEffect
{
    public static Dictionary<string, Color> ColorNames = new Dictionary<string, Color>
    {
        { "red",        Color.red},
        { "blue",       Color.blue},
        { "green",      Color.green},
        { "yellow",     Color.yellow},
        { "cyan",       Color.cyan},
        { "magenta",    Color.magenta},
        { "white",      Color.white},
        { "black",      Color.black},
        { "gray",       Color.grey},
        { "testcolor",           new Color(100, 200, 50)}
    };

    private Color color;

    public ColorText(TextMeshProUGUI text, string arguments)
    {
        //this.color = color;
        ParseArgumentsToColor(arguments);
    }

    public override bool ParseFromArgs(string arguments)
    {
        throw new NotImplementedException();
    }

    /*public override void Apply()
    {
        base.Apply();
        text.color = this.color;

    }*/

    private Color? ParseArgumentsToColor(string arguments)
    {
        string[] splitArgs = arguments.Split(',');
        
        if(splitArgs.Length == 1)// could be a name or a hex code
        {
            if(splitArgs[0].Trim()[0] == '#')// it's a hex code
            {
                if (ColorUtility.TryParseHtmlString(splitArgs[0], out Color result))
                {
                    return result;
                }
            }
            else// it's a name
            {
                // match to a dictionary
                if(ColorNames.ContainsKey(splitArgs[0]))
                {
                    return ColorNames[splitArgs[0].ToLower()];
                }
            }
        }
        else if(splitArgs.Length == 3)// exactly three means an rgb code
        {
            if( int.TryParse(splitArgs[0], out int r) &&
                int.TryParse(splitArgs[0], out int g) &&
                int.TryParse(splitArgs[0], out int b))
            {
                return new Color(r/255, g/255, b/255);
            }
        }

        return null;
    }

}
