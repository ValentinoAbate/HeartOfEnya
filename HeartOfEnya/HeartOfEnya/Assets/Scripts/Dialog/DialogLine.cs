using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class DialogLine
{
    public string CleanText
    {
        get
        {
            return cleanText;
        }
    }

    [SerializeField]
    private List<TextEffect.CustomColor> customColors;

    private struct TagMatch
    {
        public bool open;
        public Match match;

        public TagMatch(bool open, Match match)
        {
            this.open = open;
            this.match = match;
        }
    }

    private struct TagString
    {
        public int index;
        public string tag;

        public TagString(string tag, int index)
        {
            this.tag = tag;
            this.index = index;
        }
    }

    private static readonly Regex openEffectRegex = new Regex(@"<([^\/<>()]*)(\(([^<>]*)\))?>");
    private static readonly Regex closeEffectRegex = new Regex(@"<\/([^<>]*)>");
    //private static readonly Regex colorRegex = new Regex(@"<color=(.*)>");

    private Dictionary<string, TextEffect> effectDictionary;
    private List<TextEffect> customEffects;// a list of the current custom effects to apply to this sentence
    private List<StandardTextEffect> standardEffects;// a list of the current standard effects to apply to this sentence

    private int currentLength = 0;

    private string cleanText;

    public DialogLine(string text, List<TextEffect.CustomColor> customColors, List<TextEffect> effects)
    {
        this.customColors = customColors;

        customEffects = new List<TextEffect>();
        standardEffects = new List<StandardTextEffect>();

        // store the user effects in a dictionary for ease of use
        effectDictionary = new Dictionary<string, TextEffect>();
        effects.ForEach(effect =>
        {
            if (!effectDictionary.ContainsKey(effect.parsingTag)) effectDictionary.Add(effect.parsingTag, effect);
            else Debug.LogError("Effect Parsing Error: multiple custom effects share the same name");
        });

        ParseString(text);
    }

    public string GetFormatted(int length = int.MaxValue)
    {
        currentLength = length;

        if (length == 0) return "";

        string formattedText = cleanText.Substring(0, Mathf.Min(cleanText.Length, length));
        
        List<TagString> tags = new List<TagString>();
        foreach(StandardTextEffect effect in standardEffects)
        {
            if (effect.start < length)
            {
                tags.Add(new TagString(effect.startTag, effect.start));
                Debug.Log($"start: {effect.startTag}");
                tags.Add(new TagString(effect.endTag, Mathf.Min(effect.start + effect.length, length)));
                Debug.Log($"end: {effect.endTag}");
            }
        }

        tags.Sort((a, b) => a.index - b.index);

        for (int i = tags.Count - 1; i >= 0; i--)
        {
            /*Debug.Log(tags[i]);
            Debug.Log(tags[i].index);
            Debug.Log(tags[i].tag);*/
            formattedText = formattedText.Insert(tags[i].index, tags[i].tag);
        }

        return formattedText;
    }

    void ParseString(string toParse)
    {
        // replace custom colors with thier hex codes
        /*Match colorMatch = colorRegex.Match(toParse);
        foreach (CustomColor customColor in customColors)
        {
            toParse = toParse.Replace("<color=" + customColor.name + ">", "<#" + ColorUtility.ToHtmlStringRGB(customColor.color) + ">");
        }*/

        // match all special effects
        List<TagMatch> matches = new List<TagMatch>();
        foreach (Match match in openEffectRegex.Matches(toParse)) matches.Add(new TagMatch(true, match));
        foreach (Match match in closeEffectRegex.Matches(toParse)) matches.Add(new TagMatch(false, match));
        matches.Sort((a, b) => a.match.Index - b.match.Index);

        //  find all special text effects (anything not covered by the TMP richtext parser)
        //  and parse them out of the string, saving them in a dialogsentence

        cleanText = "";

        // assuming for now that there's at least one tag
        int current = 0;
        int removedCount = 0;

        List<TextEffect> openEffects = new List<TextEffect>();
        customEffects.Clear();
        List<StandardTextEffect> openStandardEffects = new List<StandardTextEffect>();
        standardEffects.Clear();

        foreach (TagMatch tag in matches)
        {
            removedCount += tag.match.Value.Length;

            cleanText += toParse.Substring(current, tag.match.Index - current);// add up to the tag
            current = tag.match.Index + tag.match.Length;// advance current past the parsed open tag

            if (tag.open)
            {
                // if the tag has parens, it's a custom effect
                if (tag.match.Groups[3].Captures.Count > 0)
                {
                    // match an effect to the given tag, if applicable
                    if (effectDictionary.ContainsKey(tag.match.Groups[1].Value))
                    {
                        TextEffect createdEffect = effectDictionary[tag.match.Groups[1].Value].Copy();
                        createdEffect.start = current - removedCount;
                        openEffects.Add(createdEffect);
                    }
                    else Debug.LogError("DialogSentence parsing error: unable to find match for open tag \"" + tag.match.Groups[1].Captures[0].Value + "\"");
                }
                else
                {
                    StandardTextEffect createdEffect = (StandardTextEffect)ScriptableObject.CreateInstance("StandardTextEffect");
                    Debug.Log($"match value: {tag.match.Value}");
                    createdEffect.SetValues(tag.match.Groups[1].Value, tag.match.Value, current - removedCount, customColors);
                    openStandardEffects.Add(createdEffect);
                }
            }
            else
            {
                // if the close tag is a custom effect
                if (effectDictionary.ContainsKey(tag.match.Groups[1].Value))
                {
                    // check if this close matches any outstanding open effects
                    // iterate from the end of the list to match the innermost open effect
                    for (int i = openEffects.Count - 1; i >= 0; i--)
                    {
                        if (tag.match.Groups[1].Value == openEffects[i].parsingTag)
                        {
                            openEffects[i].length = current - removedCount - openEffects[i].start;
                            customEffects.Add(openEffects[i]);
                            openEffects.RemoveAt(i);
                            break;
                        }
                    }
                }
                else // if the close tag doesn't match any of our defined tags, pass it through to TMPro instead of removing it
                {
                    // check if this close matches any outstanding open effects
                    // iterate from the end of the list to match the innermost open effect
                    for (int i = openStandardEffects.Count - 1; i >= 0; i--)
                    {
                        if (tag.match.Groups[1].Value == openStandardEffects[i].parsingTag)
                        {
                            openStandardEffects[i].length = current - removedCount - openStandardEffects[i].start;
                            openStandardEffects[i].endTag = tag.match.Value;
                            Debug.Log($"closing tag: {tag.match.Value}");
                            standardEffects.Add(openStandardEffects[i]);
                            openStandardEffects.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        // add any remaining portion of the string
        if (current < toParse.Length)
        {
            cleanText += toParse.Substring(current);
        }

        foreach (StandardTextEffect effect in openStandardEffects)
        {
            effect.length = cleanText.Length - effect.start;
            standardEffects.Add(effect);
        }
    }
    
    public void UpdateEffects(TextMeshProUGUI text)
    {
        foreach(TextEffect effect in customEffects)
        {
            if(effect.start <= currentLength)
            {
                effect.Run(text);
            }
        }
    }

}
