using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogManager : MonoBehaviour
{

    public Dialog.DialogUI ui;
    public DialogueRunner runner;
    /// <summary>
    /// DialogueManager Singleton
    /// </summary>
    public static DialogManager main;
    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

  
    // Update is called once per frame
    void Update()
    {
        
    }
}
