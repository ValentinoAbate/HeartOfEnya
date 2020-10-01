using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Reflection;
using System;


public abstract class TextEffect : ScriptableObject
{
    [HideInInspector]
    public int start;
    [HideInInspector]
    public int length;

    protected bool isValid;

    [System.Serializable]
    public struct CustomColor
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public Color color;
    }

    /*public static TextEffect CreateEffect(TextMeshProUGUI text, string effectType, string arguments)
    {
        if(effectType.Trim().ToLower() == "color")
        {
            ColorText colorText = new ColorText(text, arguments);
            return colorText.isValid ? colorText : null;
        }
        else
        {
            return null;
        }
    }*/

    public virtual bool ParseFromArgs(string arguments) { return true; }

    /// <summary>
    /// Initialization behavior for this effect
    /// </summary>
    /// <param name="text">The text to apply this effect to</param>
    public virtual void Apply(TextMeshProUGUI text) { }

    /// <summary>
    /// Update behavior for this effect
    /// </summary>
    /// <param name="text">The text to apply this effect to</param>
    public virtual void Run(TextMeshProUGUI text) { }

    /// <summary>
    /// Create a deep copy of this effect
    /// </summary>
    public virtual TextEffect Copy()
    {
        //TextEffect copy = (TextEffect)Activator.CreateInstance(this.GetType());
        TextEffect copy = (TextEffect)CreateInstance(this.GetType().Name);
        copy.name = this.name;
        copy.start = this.start;
        copy.length = this.length;
        copy.isValid = this.isValid;

        return copy;
    }
}
