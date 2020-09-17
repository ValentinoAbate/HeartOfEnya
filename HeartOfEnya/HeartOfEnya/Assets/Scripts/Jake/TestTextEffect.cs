using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestTextEffect : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public TextEffect effect;

    public void StartEffect(TextEffect toStart, TextMeshProUGUI text)
    {
        textBox.ForceMeshUpdate();

        effect = toStart;
        textBox = text;
        effect.Apply(text);
    }

    // Update is called once per frame
    void Update()
    {
        effect.Run(textBox);
    }
}
