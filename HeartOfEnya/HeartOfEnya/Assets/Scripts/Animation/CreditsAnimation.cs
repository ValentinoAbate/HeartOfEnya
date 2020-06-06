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

    bool started = false;
    bool finished = false;

    void Start()
    {
        tf = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            if(!finished)
            {
                ScrollCredits();
            }
            else
            {
                Debug.Log("Credits finished");
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
        float speed = (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) ? scrollSpeed * 2 : scrollSpeed;
        tf.anchoredPosition = new Vector2(tf.anchoredPosition.x,
                                          tf.anchoredPosition.y + Time.deltaTime * speed);
        if (endobj.TransformPoint(endobj.localPosition).y > 0) finished = true;
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
