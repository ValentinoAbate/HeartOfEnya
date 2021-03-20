using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BattleLighting : MonoBehaviour
{
    public enum Time
    {
        Morning = 0,
        Noon = 1,
        Afternoon = 2,
        Evening = 3
    }

    public Lighting[] lighting;

    [SerializeField]
    Lighting current;
    int index;

    // lights
    [SerializeField]
    Light2D global;

    [SerializeField]
    Light2D sunGround;

    [SerializeField]
    Light2D sunUnits;

    // transition fps
    int transitionIntervalsPerSecond = 60;

    private readonly Dictionary<Time, int> lightingPhaseLengths = new Dictionary<Time, int>
    {
        {Time.Morning,   3},
        {Time.Noon, 9},
        {Time.Afternoon,   15},
    };

    private void Start()
    {
        current = lighting[0];
        index = 0;

         //StartCoroutine(TestLighting());
    }

    public bool ReadyToProgress(int turn)
    {
        Time currTime = (Time)index;
        return lightingPhaseLengths.ContainsKey(currTime) && turn > lightingPhaseLengths[currTime];
    }

    // progress lighting to next time of day
    public void ProgressLighting(float timeInSeconds = 5.0f)
    {
        if(index >= lighting.Length - 1)
        {
            Debug.Log("Lighting already at Evening");
            return;
        }
        index++;

        StopAllCoroutines();
        UpdateCurrent();

        StartCoroutine(TransitionOverTime(lighting[index], timeInSeconds));
    }

    // transition to lighting by name
    // using enums
    public void TransitionLightingByName(Time name, float timeInSeconds = 5.0f)
    {
        index = (int)name;

        StopAllCoroutines();
        UpdateCurrent();

        StartCoroutine(TransitionOverTime(lighting[index], timeInSeconds));
        return;
        
    }

    // coroutine that lerps between two lighting settings
    IEnumerator TransitionOverTime(Lighting next, float timeInSeconds)
    {
        Debug.Log("Transitioning lighting...");
        float timePerInterval = 1f / transitionIntervalsPerSecond;
        float timeElapsed = 0f;
        while (timeElapsed < timeInSeconds)
        {
            float ratio = Mathf.SmoothStep(0f, 1f, timeElapsed / timeInSeconds);

            // lerp global lighting
            global.color = Color.Lerp(current.globalCol, next.globalCol, ratio);
            global.intensity = Mathf.Lerp(current.globalIntensity, next.globalIntensity, ratio);

            // lerp sun color
            sunGround.color = Color.Lerp(current.sunCol, next.sunCol, ratio);
            sunUnits.color = sunGround.color;

            // lerp sun intensity
            sunGround.intensity = Mathf.Lerp(current.sunIntensity, next.sunIntensity, ratio);
            sunUnits.intensity = sunGround.intensity * Mathf.Lerp(current.unitSunMult, next.unitSunMult, ratio);

            // lerp sun position
            Vector2 newPos = Vector2.Lerp(current.sunPos, next.sunPos, ratio);
            sunGround.transform.localPosition = sunUnits.transform.localPosition = new Vector3(newPos.x, newPos.y, sunUnits.transform.localPosition.z);

            // lerp sun scale
            Vector2 newScale = Vector2.Lerp(current.sunScale, next.sunScale, ratio);
            sunGround.transform.localScale = sunUnits.transform.localScale = new Vector3(newScale.x, newScale.y, sunGround.transform.localScale.z);

            //lerp sun rotation
            float newRot = Mathf.Lerp(current.sunRot, next.sunRot, ratio);
            sunGround.transform.localEulerAngles = sunUnits.transform.localEulerAngles = new Vector3(0f, 0f, newRot);

            timeElapsed += timePerInterval;
            yield return new WaitForSeconds(timePerInterval);
        }

        global.color = next.globalCol;
        global.intensity = next.globalIntensity;

        sunGround.color = sunUnits.color = next.sunCol;
        sunGround.intensity = next.sunIntensity;
        sunUnits.intensity = next.sunIntensity * next.unitSunMult;

        sunGround.transform.position = sunUnits.transform.position = new Vector3(next.sunPos.x, next.sunPos.y, sunUnits.transform.position.z);
        sunGround.transform.localScale = sunUnits.transform.localScale = new Vector3(next.sunScale.x, next.sunScale.y, sunUnits.transform.localScale.z);
        sunGround.transform.localEulerAngles = sunUnits.transform.localEulerAngles = new Vector3(0f, 0f, next.sunRot);

        current = next;

        Debug.Log("Lighting transition complete! Transitioned to " + next.name);
    }

    public void UpdateCurrent()
    {
        current.globalCol = global.color;
        current.globalIntensity = global.intensity;

        current.sunCol = sunGround.color;
        current.sunIntensity = sunGround.intensity;
        current.sunPos = new Vector2(sunGround.transform.position.x, sunGround.transform.position.y);
        current.sunScale = new Vector2(sunGround.transform.localScale.x, sunGround.transform.localScale.y);

        current.unitSunMult = sunUnits.intensity / sunGround.intensity;
    }



    [System.Serializable]
    public struct Lighting
    {
        public string name;

        public Color globalCol;
        public float globalIntensity;

        public Color sunCol;
        public float sunIntensity;
        public Vector2 sunPos;
        public Vector2 sunScale;
        public float sunRot;

        public float unitSunMult;
    }

    // test function for lighting transitions
    private IEnumerator TestLighting()
    {
        Debug.Log(sunGround);
        Debug.Log(sunUnits);
        Debug.Log(global);
        yield return StartCoroutine(TransitionOverTime(lighting[(int)Time.Morning], 5.0f));
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(TransitionOverTime(lighting[(int)Time.Noon], 5.0f));
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(TransitionOverTime(lighting[(int)Time.Afternoon], 5.0f));
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(TransitionOverTime(lighting[(int)Time.Evening], 5.0f));
    }
}
