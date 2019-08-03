using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;
using System.Text.RegularExpressions;

namespace Dialog
{
    public class DialogUI : DialogueUIBehaviour
    {
        private static readonly Regex exprRegex = new Regex(@"\s*[*]\s*:[*]");
        public GameObject dialogBoxPrefab;


        private DialogBox dialogBox;

        // Start is called before the first frame update
        void Start()
        {
            if(dialogBoxPrefab != null)
            {
                var db = dialogBoxPrefab.GetComponent<DialogBox>();
                Debug.Assert(db != null, "Improper dialog box attactched to DialogUI: No DialogBox component found");
            }
            else
                Debug.LogError("No dialog box prefab set in DialogUI on object: " + name);

        }

        public override IEnumerator RunCommand(Command command)
        {
            yield break;
        }

        public override IEnumerator RunLine(Line line)
        {
            var match = exprRegex.Match(line.text);
            if(!match.Success)
            {
                Debug.LogError("Improperly Formatted Dialog. No \":\" character found to separate speaker and line.");
                yield break;
            }
            string speaker = match.Groups[1].Value;
            string text = match.Groups[2].Value;
        }

        public override IEnumerator RunOptions(Options optionsCollection, OptionChooser optionChooser)
        {

            yield break;
        }
    }
}
