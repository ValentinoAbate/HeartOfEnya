using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInfoPanelParty : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image unitImageBg;
    public Image unitImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI hpNumberText;
    public Image hpBarImage;
    public TextMeshProUGUI fpNumberText;
    public Image fpBarImage;
    public TextMeshProUGUI passiveText;
    public TextMeshProUGUI passiveNameText;
    public Image nameplate;

    [Header("Unit Display Background Colors")]
    public Color bapyColor;
    public Color rainaColor;
    public Color soleilColor;
    public Color luaColor;
    [Header("Unit Display Background Images")]
    public Sprite bapySprite;
    public Sprite rainaSprite;
    public Sprite soleilSprite;
    public Sprite luaSprite;
    [Header("Unit Display Nameplate Images")]
    public Sprite bapyNameplate;
    public Sprite rainaNameplate;
    public Sprite soleilNameplate;
    public Sprite luaNameplate;

    private Dictionary<string, Sprite> spriteDict;
    private Dictionary<string, Sprite> nameplateDict;
    private Dictionary<string, Color> colorDict;

    private void Awake()
    {
        spriteDict = new Dictionary<string, Sprite>
        {
            {"bapy", bapySprite },
            {"raina", rainaSprite },
            {"soleil", soleilSprite },
            {"lua", luaSprite },
        }; 
        nameplateDict = new Dictionary<string, Sprite>
        {
            {"bapy", bapyNameplate },
            {"raina", rainaNameplate },
            {"soleil", soleilNameplate },
            {"lua", luaNameplate },
        };
        colorDict = new Dictionary<string, Color>
        {
            {"bapy", bapyColor },
            {"raina", rainaColor },
            {"soleil", soleilColor },
            {"lua", luaColor },
        };
    }

    public void ShowUI(PartyMember p)
    {
        nameText.text = p.DisplayName;
        descriptionText.text = p.description;
        //statusText.text = "status";
        moveNumberText.text = p.Move.ToString();
        hpNumberText.text = p.Hp.ToString();
        hpBarImage.fillAmount = p.Hp / (float)p.maxHp;
        fpNumberText.text = p.Fp.ToString();
        fpBarImage.fillAmount = p.Fp / (float)p.maxFp;
        unitImage.sprite = p.DisplaySprite;
        unitImage.color = p.DisplaySpriteColor;
        //unitImageBg.sprite = spriteDict[p.DisplayName.ToLower()];
        //unitImageBg.color = colorDict[p.DisplayName.ToLower()];
        nameplate.sprite = nameplateDict[p.DisplayName.ToLower()];
        //statusText.text = "Status: " + (p.DeathsDoor ? "Dying." : "Doing fine!");
        if (p.Stunned)
            statusText.text = "Status: " + "Stunned.";
        if (!string.IsNullOrWhiteSpace(p.passiveName))
        {
            passiveNameText.text = p.passiveName + ": ";
            passiveText.text = p.passiveDescription;
        }
        else
        {
            passiveText.text = string.Empty;
            passiveNameText.text = string.Empty;
        }
    }
}
