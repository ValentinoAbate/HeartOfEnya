using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTextShake", menuName = "TextEffect/TextShake")]
public class TextShake : TextEffect
{
    public float xShakeRate;
    public float yShakeRate;

    public float xShakeDistance;
    public float yShakeDistance;

    public AnimationCurve xShakeShape;
    public AnimationCurve yShakeShape;

    public override void Apply(TextMeshProUGUI text)
    {
        base.Apply(text);
        Debug.Log($"my name is {name}");
    }

    public override void Run(TextMeshProUGUI text)
    {
        base.Run(text);

        for (int i = start; i < start + length; i++)
        {
            if (i >= text.textInfo.characterCount) break;
            TMP_CharacterInfo character = text.textInfo.characterInfo[i];

            Vector2 offset = new Vector2(
                xShakeDistance * (xShakeShape.Evaluate((Time.time * xShakeRate) % 1) - 0.5f),
                yShakeDistance * (yShakeShape.Evaluate((Time.time * yShakeRate) % 1) - 0.5f));
            //offset = new Vector3(0, 1, 0);
            if (character.character != ' ' && character.character != '\0')
            {
                for (int k = 0; k < 4; k++)
                {
                    text
                        .textInfo
                        .meshInfo[character.materialReferenceIndex]
                        .vertices[character.vertexIndex + k] += (Vector3)offset;
                }
            }
        }

        text.UpdateVertexData();
    }

    public override TextEffect Copy()
    {
        TextShake copy = (TextShake)base.Copy();

        copy.xShakeRate = this.xShakeRate;
        copy.yShakeRate = this.yShakeRate;

        copy.xShakeDistance = this.xShakeDistance;
        copy.yShakeDistance = this.yShakeDistance;

        copy.xShakeShape = xShakeShape;
        copy.yShakeShape = yShakeShape;

        return copy;
    }

    public override bool ParseFromArgs(string arguments)
    {
        string[] split = arguments.Split();
        if(split.Length == 4)// only valid if we get four arguments
        {
            float[] args = new float[4];

            for (int i = 0; i < 4; i++)
            {
                try
                {
                    args[i] = float.Parse(split[i]);
                }
                catch (FormatException e)
                {
                    Debug.LogError("TextShake argument parse error: " + e.Message);
                    return false;
                }
            }

            xShakeRate = args[0];
            yShakeRate = args[1];
            xShakeDistance = args[2];
            yShakeDistance = args[3];

            return true;
        }
        else
        {
            return false;
        }
    }
}
