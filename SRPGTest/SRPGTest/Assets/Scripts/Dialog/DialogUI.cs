using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;
using System.Text.RegularExpressions;

namespace Dialog
{
    public class DialogUI : DialogueUIBehaviour
    {
        private static readonly Regex exprRegex = new Regex(@"\s*(.*)\s*:(.*)");

        public float scrollDelay;
        public Canvas dialogCanvas;
        public GameObject dialogBoxPrefab;
        /// The buttons that let the user choose an option
        public List<Button> optionButtons;


        /// A delegate that we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

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
            var dialogBox = Instantiate(dialogBoxPrefab, dialogCanvas.transform).GetComponent<DialogBox>();
            yield return StartCoroutine(dialogBox.PlayLine(text, scrollDelay));
            Destroy(dialogBox.gameObject);
            
        }

        /// Show a list of options, and wait for the player to make a selection.
        public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                                Yarn.OptionChooser optionChooser)
        {
            // Do a little bit of safety checking
            if (optionsCollection.options.Count > optionButtons.Count)
            {
                Debug.LogWarning("There are more options to present than there are" +
                                 "buttons to present them in. This will cause problems.");
            }

            // Display each option in a button, and make it visible
            int i = 0;
            foreach (var optionString in optionsCollection.options)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].GetComponentInChildren<Text>().text = optionString;
                i++;
            }

            // Record that we're using it
            SetSelectedOption = optionChooser;

            // Wait until the chooser has been used and then removed (see SetOption below)
            yield return new WaitUntil(() => SetSelectedOption == null);

            // Hide all the buttons
            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        /// Called by buttons to make a selection.
        public void SetOption(int selectedOption)
        {

            // Call the delegate to tell the dialogue system that we've
            // selected an option.
            SetSelectedOption(selectedOption);

            // Now remove the delegate so that the loop in RunOptions will exit
            SetSelectedOption = null;
        }
    }
}
