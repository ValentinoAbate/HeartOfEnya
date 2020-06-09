using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsAnimation : MonoBehaviour
{
    RectTransform tf;

    [SerializeField]
    Animator logoanim;

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
    }

    // Update is called once per frame
    void Update()
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
                StartCoroutine(SpeedUp(scrollSpeed));
            }
        }
    }

    void ScrollCredits()
    {
        tf.anchoredPosition = new Vector2(tf.anchoredPosition.x,
                                          tf.anchoredPosition.y + Time.deltaTime * scrollSpeed);
        if (endobj.TransformPoint(endobj.localPosition).y > -3600)
        {
            sm.TransitionScenes("MainMenu");
            finished = true;
        }
    }

    IEnumerator SpeedUp(float topspeed)
    {
        float stepcount = 10;
        for(int i = 0; i < stepcount; i++)
        {
            scrollSpeed = Mathf.SmoothStep(0, topspeed, i / stepcount);
            yield return new WaitForSeconds(1.5f / stepcount);
        }
        scrollSpeed = topspeed;
    }
}
