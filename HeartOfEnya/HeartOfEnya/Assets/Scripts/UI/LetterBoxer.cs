using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LetterBoxer : MonoBehaviour
{    
    public enum ReferenceMode { DesignedAspectRatio, OrginalResolution };
  
    public ReferenceMode referenceMode;
    public float x = 16;
    public float y = 9;  
    public float width = 1920;
    public float height = 1080;
    public bool onAwake = true;

    private Camera cam;        

    public void Awake()
    {
        // store reference to the camera
        cam = GetComponent<Camera>();

        // perform sizing if onAwake is set
        if(onAwake)
        {
            PerformSizing();
        }
    }

    private void FixedUpdate()
    {
        PerformSizing();
    }

    private void OnValidate()
    {
        x = Mathf.Max(1, x);
        y = Mathf.Max(1, y);
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);
    }

    // based on logic here from http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html
    public void PerformSizing()
    {
        // calc based on aspect ratio
        float targetRatio = x / y;

        // recalc if using resolution as reference
        if (referenceMode == LetterBoxer.ReferenceMode.OrginalResolution)
        {
            targetRatio = width / height;
        }

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetRatio;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            cam.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = cam.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
