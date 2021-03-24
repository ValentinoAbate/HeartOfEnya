using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAnimation : MonoBehaviour
{
    RectTransform tf;

    [SerializeField]
    Animator logoanim;
    Animator thisanim;

    [SerializeField]
    RectTransform endobj;

    [SerializeField]
    float scrollSpeed;

    [SerializeField]
    SceneTransitionManager sm;

    bool started = false;
    bool finished = false;
    bool transitioning = false;

    void Start()
    {
        tf = gameObject.GetComponent<RectTransform>();
        sm = FindObjectOfType<SceneTransitionManager>();
        thisanim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(started)
        {
            if (!finished)
            {
                ScrollCredits();
            }
        }
        else if(!finished)
        {
            if (logoanim.GetCurrentAnimatorStateInfo(0).IsName("Logo Startup") &&
                logoanim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                Debug.Log("Logo finished");
                started = true;
                thisanim.SetTrigger("Start");
            }
        }
    }

    void ScrollCredits()
    {
        if (thisanim.GetCurrentAnimatorStateInfo(0).IsName("Credits") &&
            thisanim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            finished = true;
            StartCoroutine(DelayedTransition());
        }
    }

    IEnumerator DelayedTransition()
    {
        yield return new WaitForSeconds(1.5f);
        sm.TransitionScenes("MainMenu");
    }
}
