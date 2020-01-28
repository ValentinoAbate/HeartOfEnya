using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionVfxWaitForSeconds : ActionVfx
{
    public float secondsToNext;
    public float totalDelay;

    public override Coroutine Play()
    {
        return StartCoroutine(PlayCr());
    }

    private IEnumerator PlayCr()
    {
        yield return new WaitForSeconds(secondsToNext);
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(totalDelay - secondsToNext);
        Destroy(gameObject);
    }
}
