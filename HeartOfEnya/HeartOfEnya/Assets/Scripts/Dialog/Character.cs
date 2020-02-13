using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public const string defaultExpression = "normal";
    public static readonly Color dimColor = Color.gray;
    public string Name { get => characterName; }
    [SerializeField] private string characterName;
    public Vector2 DialogSpawnPoint => dialogSpawnPoint.position;
    [SerializeField] private Transform dialogSpawnPoint;
    [SerializeField] private CharacterData data;
    public string Expression
    {
        get => expression;
        set
        {
            if (data.portraits.ContainsKey(value))
            {
                expression = value;
                Portrait = data.portraits[value];
            }                
            else
                Debug.LogError(value + " is not a valid expression for " + Name);
        }
    }
    private string expression;
    public Sprite Portrait { get; private set; }
    public AudioClip TextBlip { get => data.textScrollSfx; }

    private void Awake()
    {
        Expression = defaultExpression;
    }

    void OnMouseDown()
    {
    	Debug.Log("You've clicked on: " + Name);
    }
}

    
