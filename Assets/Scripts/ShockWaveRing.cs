using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveRing : MonoBehaviour
{
    public float duration = 0.35f;
    public float finalRadius = 6f;   // escala X/Z
    public AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
    public float fadeStart = 0.6f;   // fracción del tiempo para empezar a desvanecer

    Material _matInstance;
    Color _baseColor;

    void Awake()
    {
        var r = GetComponent<Renderer>();
        if (r) { _matInstance = r.material; _baseColor = _matInstance.color; }
        transform.localScale = Vector3.one * 0.01f;
    }

    public void Play(Vector3 pos, Quaternion rot)
    {
        transform.SetPositionAndRotation(pos, rot);
        StopAllCoroutines();
        StartCoroutine(Co());
    }

    System.Collections.IEnumerator Co()
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);
            float s = Mathf.Lerp(0.01f, finalRadius, curve.Evaluate(a));
            transform.localScale = new Vector3(s, 1f, s);

            if (_matInstance)
            {
                float fade = a < fadeStart ? 1f : 1f - Mathf.InverseLerp(fadeStart, 1f, a);
                _matInstance.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, fade);
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}

