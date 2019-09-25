﻿using System.Collections;
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
        private static readonly Regex lineRegex = new Regex(@"\s*(\w)\s*(?:\((\w*)\))?\s*:\s*(.*)");

        public float scrollDelay;
        public Canvas dialogCanvas;
        public GameObject dialogBoxPrefab;
        /// The buttons that let the user choose an option
        public List<Button> optionButtons;
        /// The characters currently in the scene
        public List<Character> characters;
        // The last character that spoke
        private Character lastSpeaker;
        // The currently active dialogbox
        private DialogBox dialogBox;

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
            // Correct the formatting
            line.text = CorrectFormatting(line.text);
            // Split speaker name, expression, and line text
            var match = lineRegex.Match(line.text);
            if(!match.Success)
            {
                Debug.LogError("Improperly Formatted Dialog. No \":\" character found to separate speaker and line.");
                yield break;
            }
            string speaker = match.Groups[1].Value.ToLower();
            string expr = match.Groups[2].Value.ToLower();
            string text = match.Groups[3].Value;

            // Find speaking character
            var character = characters.Find((c) => c.Name.ToLower() == speaker);
            if(character == default)
            {
                Debug.LogWarning("Character " + speaker + "not found in scene. Skipping line.");
                yield break;
            }
            // Set the character's expression if necessary
            if (!string.IsNullOrWhiteSpace(expr))
                character.Expression = expr;
            // Spawn a new dialogBox if necessary
            if(character != lastSpeaker || dialogBox == null)
            {
                dialogBox = Instantiate(dialogBoxPrefab, Camera.main.WorldToScreenPoint(character.DialogSpawnPoint), 
                                        Quaternion.identity, dialogCanvas.transform).GetComponent<DialogBox>();
                lastSpeaker = character;
            }
            // Set the dialogBox's portrait
            dialogBox.portrait.sprite = character.Portrait;
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
        /// <summary>
        /// Correct formatting issues introduced by Google Docs, etc.
        /// </summary>
        public string CorrectFormatting(string line)
        {
            // Correct google doc formatting
            return line.Replace("…", "...").Replace('“', '"').Replace('”', '"');
        }
    }
}
