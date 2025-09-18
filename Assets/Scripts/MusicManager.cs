using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Clips")]
    [SerializeField] AudioClip fightLoop;
    [SerializeField] AudioClip loseClip;


    [Header("Crossfade")]
    [SerializeField] float fadeTime = 1.0f;

    AudioSource a, b;
    AudioSource active, idle;

    void Awake()
    {
        var sources = GetComponents<AudioSource>();
        if (sources.Length < 2)
        {
            Debug.LogError("MusicManager necesita 2 AudioSource en el mismo GO.");
            enabled = false; return;
        }
        a = sources[0]; b = sources[1];
        a.loop = b.loop = true;
        a.playOnAwake = b.playOnAwake = false;
        a.spatialBlend = b.spatialBlend = 0f;

        active = a; idle = b;
    }


     public void PlayFight()
    {
        PlayLoop(fightLoop);
    }

    public void PlayLose()
    {

        idle.loop = false;
        CrossfadeTo(loseClip);
    
    }

    void PlayLoop(AudioClip clip, bool instant=false)
    {
        idle.loop = true;
        if (instant)
        {
            active.Stop();
            active.clip = clip;
            active.volume = 1f;
            active.Play();
        }
        else
        {
            CrossfadeTo(clip);
        }
    }

    void CrossfadeTo(AudioClip next)
    {
        idle.clip = next;
        idle.time = 0f;
        idle.volume = 0f;
        idle.Play();
        StopAllCoroutines();
        StartCoroutine(FadeCo());
        Swap(); 
    }

    System.Collections.IEnumerator FadeCo()
    {
        float t = 0f;
        var srcUp = active;     
        var srcDown = idle;     

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            srcUp.volume = k;
            srcDown.volume = 1f - k;
            yield return null;
        }
        srcUp.volume = 1f;
        srcDown.volume = 0f;
        srcDown.Stop();
    }

    void Swap()
    {
        var tmp = active;
        active = idle;
        idle = tmp;
    }
    
}
