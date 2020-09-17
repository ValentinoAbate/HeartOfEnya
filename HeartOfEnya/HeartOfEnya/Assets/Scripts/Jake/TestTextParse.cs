using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityScript.Steps;
using System.Reflection;
using System;
using System.Security.Cryptography;

public class TestTextParse : MonoBehaviour
{
    public string toParse;

    public TextMeshProUGUI text;

    public List<TextEffect> customEffects;

    [SerializeField]
    private List<TextEffect.CustomColor> customColors;

    /*[System.Serializable]
    private struct CustomColor
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public Color color;
    }*/

    private struct Tag
    {
        public bool open;
        public Match match;

        public Tag(bool open, Match match)
        {
            this.open = open;
            this.match = match;
        }
    }


    private static readonly Regex openEffectRegex = new Regex(@"<([^\/<>()]*)(\(([^<>]*)\))?>");
    private static readonly Regex closeEffectRegex = new Regex(@"(<\/[^<>]*>)");
    private static readonly Regex colorRegex = new Regex(@"<color=(.*)>");

    private Dictionary<string, TextEffect> effectDictionary;

    private string final;


    // Start is called before the first frame update
    void Start()
    {
        DialogLine sentence = new DialogLine(toParse, customColors, customEffects);
        
        text.text = sentence.GetFormatted();

        // store the user effects in a dictionary for ease of use
        /*effectDictionary = new Dictionary<string, TextEffect>();
        customEffects.ForEach(effect =>
        {
            if (!effectDictionary.ContainsKey(effect.parsingTag)) effectDictionary.Add(effect.parsingTag, effect);
            else Debug.LogError("Effect Parsing Error: multiple custom effects share the same name");
        });

        // replace custom colors with thier hex codes
        Match colorMatch = colorRegex.Match(toParse);
        foreach(CustomColor customColor in customColors)
        {
            toParse = toParse.Replace("<color=" + customColor.name + ">", "<#" + ColorUtility.ToHtmlStringRGB(customColor.color) + ">");
        }

        // match all special effects
        List<Tag> matches = new List<Tag>();
        foreach (Match match in openEffectRegex.Matches(toParse)) matches.Add(new Tag(true, match));
        foreach (Match match in closeEffectRegex.Matches(toParse)) matches.Add(new Tag(false, match));
        matches.Sort((a, b) => a.match.Index - b.match.Index);

        //  find all special text effects (anything not covered by the TMP richtext parser)
        //  and parse them out of the string, saving them in a dialogsentence

        final = "";

        // assuming for now that there's at least one tag
        int current = 0;
        int removedCount = 0;

        List<TextEffect> openEffects = new List<TextEffect>();
        List<TextEffect> closedEffects = new List<TextEffect>();
        List<StandardTextEffect> openStandardEffects = new List<StandardTextEffect>();
        List<StandardTextEffect> closedStandardEffects = new List<StandardTextEffect>();

        foreach(Tag tag in matches)
        {
            removedCount += tag.match.Value.Length;

            final += toParse.Substring(current, tag.match.Index - current);// add up to the tag
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
                    StandardTextEffect createdEffect = (StandardTextEffect) ScriptableObject.CreateInstance("StandardTextEffect");
                    createdEffect.SetValues(tag.match.Groups[2].Value, tag.match.Value, current - removedCount);
                    Debug.Log(createdEffect.startTag);
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
                            closedEffects.Add(openEffects[i]);
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
                            closedStandardEffects.Add(openStandardEffects[i]);
                            openStandardEffects.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        // add any remaining portion of the string
        if(current < toParse.Length)
        {
            final += toParse.Substring(current);
        }

        foreach (StandardTextEffect effect in openStandardEffects)
        {
            effect.length = final.Length - effect.start;
            closedStandardEffects.Add(effect);
        }

        text.text = final;*/
    }
    int test = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            test++;
        }

        DialogLine sentence = new DialogLine(toParse, customColors, customEffects);

        text.text = sentence.GetFormatted(test);
    }
}
