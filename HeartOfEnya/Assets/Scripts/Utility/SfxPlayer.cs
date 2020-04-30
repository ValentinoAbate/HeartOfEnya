using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxPlayer : MonoBehaviour
{
    private AudioSource src;
    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }
    public Coroutine PlayAndDestroy(AudioClip clip)
    {
        return StartCoroutine(PlayAndDestroyCr(clip));
    }

    public Coroutine Play(AudioClip clip)
    {
        return StartCoroutine(PlayCr(clip));
    }

    private IEnumerator PlayCr(AudioClip clip)
    {
        src.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
    }

    private IEnumerator PlayAndDestroyCr(AudioClip clip)
    {
        src.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        Destroy(gameObject);
    }
}
