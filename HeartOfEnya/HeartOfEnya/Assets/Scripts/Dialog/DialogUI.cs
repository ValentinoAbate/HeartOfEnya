using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

namespace Dialog
{
    public class DialogUI : DialogueUIBehaviour, IPausable
    {
        public enum EndAction
        {
            DoNothing,
            EndScene,
            GoToCampfireScene,
        }

        private static readonly Regex lineRegex = new Regex(@"\s*(\w+)\s*(?:\(([\w\s]+)\))?\s*:\s*(.*)");
        private static readonly Regex commentRegex = new Regex(@"\/\/.*");

        public PauseHandle PauseHandle { get; set; }
        public bool SoloMode { get; private set; } = false;


        [Header("General Config")]
        public DialogueRunner runner;
        public float scrollDelay;
        public float spaceDelay;
        public Canvas dialogCanvas;
        public GameObject dialogBoxPrefab;
        public bool battleMode = false;
        public List<Button> optionButtonsNormal;
        public List<Button> optionButtonsSolo;
        public List<Button> optionButtonsBattle;
        /// The buttons that let the user choose an option
        private List<Button> optionButtons;
        /// The characters currently in the scene
        public List<Character> characters;
        public FMODUnity.StudioEventEmitter Music { get; set; }

        [Header("Fixed Position Fields")]
        [SerializeField] private Transform fixedPosition;
        [SerializeField] private Transform fixedPositionNormal;
        [SerializeField] private Transform fixedPositionSolo;
        public bool useFixedPosition = false;

        [Header("Scene Transition Fields")]
        public EndAction endAction = EndAction.DoNothing;
        public string sceneName = string.Empty;

        // The last character that spoke
        private Character lastSpeaker;
        // The currently active dialogbox
        private DialogBox dialogBox;
        private bool newDialog = true;

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
            PauseHandle = new PauseHandle((pause) => dialogBox?.PauseHandle.SetPauseAll(pause));
        }

        public override IEnumerator RunCommand(Command command)
        {
            var args = command.text.ToLower().Split();
            if(args[0] == "fadein")
            {
                // Find speaking character
                var character = characters.Find((c) => c.Name.ToLower() == args[1]);
                var anim = character.GetComponent<Animator>();
                anim.Play("FadeIn");
                yield return new WaitForSeconds(1.75f);
            }
            else if (args[0] == "fadeout")
            {
                // Find speaking character
                var character = characters.Find((c) => c.Name.ToLower() == args[1]);
                var anim = character.GetComponent<Animator>();
                anim.Play("FadeOut");
                yield return new WaitForSeconds(1.75f);
            }
            else if(args[0] == "cleartb")
            {
                if(dialogBox != null)
                    Destroy(dialogBox.gameObject);
            }
            else if(args[0] == "progresstheme")
            {
                Music.SetParameter("Progress Theme", 1);
            }
            yield break;
        }

        public override IEnumerator RunLine(Line line)
        {
            if(newDialog)
            {
                newDialog = false;
                foreach (var chara in characters)
                    chara.Expression = Character.defaultExpression;
            }

            // Correct the formatting
            line.text = CorrectFormatting(line.text);

            var commentMatch = commentRegex.Match(line.text);
            if (commentMatch.Success)
            {
                Debug.Log("Comment detected: " + line);
                yield break;
            }
            // Split speaker name, expression, and line text
            var match = lineRegex.Match(line.text);
            if(!match.Success)
            {
                Debug.LogError("Improperly Formatted Dialog: " + line.text);
                yield break;
            }
            string speaker = match.Groups[1].Value.ToLower();
            string expr = match.Groups[2].Value.ToLower();
            string text = match.Groups[3].Value;

            // Find speaking character
            var character = characters.Find((c) => c.Name.ToLower() == speaker);
            //characters.Add(character);
            if (character == default)
            {
                Debug.LogWarning("Character \"" + speaker + "\" not found in scene. Skipping line.");
                yield break;
            }
            // Set the character's expression if necessary
            if (!string.IsNullOrWhiteSpace(expr))
                character.Expression = expr;
            // Spawn a new dialogBox if necessary
            if(character != lastSpeaker || dialogBox == null)
            {
                if(dialogBox != null)
                    Destroy(dialogBox.gameObject);
                var dbType = battleMode ? Character.DialogType.Battle : (SoloMode ? Character.DialogType.Solo : Character.DialogType.Normal);
                var dbPrefab = character.GetDialogBoxPrefab(dbType);               
                // Determine if we want to use the character spawn point or the fixed one
                var spawnPoint = !useFixedPosition || character.ignoreFixedPosition
                    ? character.DialogSpawnPoint : (Vector2)fixedPosition.position;
                // Actually spawn the dialog box
                dialogBox = Instantiate(dbPrefab, Camera.main.WorldToScreenPoint(spawnPoint),
                                        Quaternion.identity, dialogCanvas.transform).GetComponent<DialogBox>();
                // Make sure the dialog box renders below willow
                dialogBox.transform.SetAsFirstSibling();
                lastSpeaker = character;
                dialogBox.VoiceEvent = character.VoiceEvent;
            }
            // Set the dialogBox's portrait
            dialogBox.portrait.sprite = character.Portrait;
            yield return StartCoroutine(dialogBox.PlayLine(text, scrollDelay, spaceDelay));
        }

        /// Show a list of options, and wait for the player to make a selection.
        public override IEnumerator RunOptions(Yarn.Options optionsCollection,
                                                Yarn.OptionChooser optionChooser)
        {
            optionButtons = battleMode ? optionButtonsBattle : SoloMode ? optionButtonsSolo : optionButtonsNormal;
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
                optionButtons[i].GetComponentInChildren<Text>().text = CorrectFormatting(optionString);
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

        public override IEnumerator DialogueComplete()
        {
            newDialog = true;
            if (dialogBox != null)
            {
                dialogBox.Stop();
                Destroy(dialogBox.gameObject);
            }
            if (Music != null)
                Music.Stop();
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            if (endAction == EndAction.EndScene)
            {
                // Go to next game phase, if applicable.
                string gamePhase = pData.gamePhase.ToUpper();
                if (pData.InLuaBattle)
                {
                    if (pData.luaBossDefeated)
                        GoToNextGamePhase();
                    else
                        pData.dayNum += 1; //if lua's not defeated, we spend more time in this phase
                }
                else if(gamePhase == PersistentData.gamePhaseAbsoluteZeroBattle)
                {
                    if (pData.absoluteZeroDefeated)
                        GoToNextGamePhase();
                    else
                        pData.dayNum += 1; //if Abs0's not defeated, we spend more time in this phase
                }
                else if(gamePhase == PersistentData.gamePhaseIntro)
                {
                    pData.gamePhase = PersistentData.gamePhaseTut1And2;
                    pData.dayNum = 0;
                }
                else
                {
                    GoToNextGamePhase();
                }
                SceneTransitionManager.main.TransitionScenes(sceneName);
            }
            else if(endAction == EndAction.GoToCampfireScene)
            {
                //assume we're on day 0 of the phase, in which case we should...
                pData.dayNum += 1; //...iterate to day 1...
                SceneTransitionManager.main.TransitionScenes(sceneName); //..and proceed to the battle scene
                //runner.StartCampPartyScene(phaseData);
            }
            yield break;
        }

        private void GoToNextGamePhase()
        {
            var pData = DoNotDestroyOnLoad.Instance.persistentData;
            pData.gamePhase = ((char)(pData.gamePhase[0] + 1)).ToString(); //switch to next phase
            pData.dayNum = 0; //reset the day to 0 to signifiy new phase
        }

        /// Called by buttons to make a selection.
        public void SetOption(int selectedOption)
        {
            if (SetSelectedOption == null)
                return;
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
            line = Regex.Replace(line, @"\[\w*\]" , string.Empty);
            // Correct google doc formatting
            return line.Replace("…", "...").Replace('“', '"').Replace('”', '"');
        }

        public void SetSoloMode(bool solo)
        {
            SoloMode = solo;
            fixedPosition = solo ? fixedPositionSolo : fixedPositionNormal;
        }
    }
}
