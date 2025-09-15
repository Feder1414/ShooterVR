using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class RockAEO : MonoBehaviour
{
    [SerializeField] float radius = 0.5f;
    [SerializeField] VisualEffect vfxPrefab;
    [SerializeField] string vfxRadiusProp = "Radius";
    [SerializeField] string onPlayEvent = "OnPlay"; // coincide con el Initial Event Name

    [SerializeField] Vector2 pitchRange = new Vector2(0.8f, 1.2f);
    [SerializeField] float volume = 0.8f;

    [SerializeField] string lifetimeProp = "effectLifeTime"; // opcional, por si quieres leerlo/ajustarlo
    [SerializeField] string vfxOnPlayEvent = "OnPlay";

    [SerializeField] string vfxMeteorRate = "ate";

    [SerializeField] AudioClip meteorRainSound;

    [SerializeField] float lifeTime = 2f;

    public float tickRate = 1.8f;

    [SerializeField] float coolDown = 5f;

    private float lastTimeUsedPower = -Mathf.Infinity;

    [SerializeField] float damageFactor = 5f;

    public Transform playerPosition;

    private Killable playerKillable;


    // Start is called before the first frame update

    void Awake()
    {
        playerKillable = GetComponent<Killable>();
        if (playerKillable == null)
        {
            Debug.LogError("No Killable component found on " + gameObject.name);
        }

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerMeteorRain()
    {

        StartCoroutine(MeteorRain());
    }

    IEnumerator MeteorRain()
    {
        if (Time.time - lastTimeUsedPower < coolDown) yield break;

        lastTimeUsedPower = Time.time;

        if (vfxPrefab != null)
        {
            VisualEffect vfx = Instantiate(vfxPrefab, new Vector3(playerPosition.position.x, 0, playerPosition.position.z), Quaternion.identity);
            if (vfx.HasFloat(vfxRadiusProp))
                vfx.SetFloat(vfxRadiusProp, radius);

            if (vfx.HasFloat(lifetimeProp))
                vfx.SetFloat(lifetimeProp, lifeTime);

            if (!string.IsNullOrEmpty(vfxOnPlayEvent))
                vfx.SendEvent(vfxOnPlayEvent);
            else
                vfx.Play();

            StartCoroutine(sfxMeteorRain());
            Destroy(vfx.gameObject, lifeTime + 1f); // destroy after effect finished
        }
        else
        {
            Debug.LogError("VFX Prefab is not assigned in " + gameObject.name);
        }
        Collider[] col = Physics.OverlapSphere(transform.position, radius);



        float t = 0f;
        while (t < lifeTime)
        {
            foreach (Collider c in col)
            {
                if (c == null) continue;
                if (c.TryGetComponent<Killable>(out Killable killableEnemy) && killableEnemy.GetTeam() == Killable.Team.Enemy)
                {
                    killableEnemy.TakeDamage(Mathf.RoundToInt(damageFactor * playerKillable.GetDamage()));
                }


            }
            yield return new WaitForSeconds(tickRate);
            t += tickRate;


        }



    }


    IEnumerator sfxMeteorRain()
    {
        if (meteorRainSound == null)
        {
            Debug.LogError("AudioSource or AudioClip not assigned in " + gameObject.name);
            yield break;
        }

        var frequency = vfxPrefab.HasFloat(vfxMeteorRate) ? vfxPrefab.GetFloat(vfxMeteorRate) : 1f;

        var tickRateSound = 1f / frequency;

        float t = 0f;
        while (t < lifeTime)
        {
            PlayOneShot3D(transform.position);
            yield return new WaitForSeconds(tickRateSound);
            t += tickRateSound;
        }

    }


    void PlayOneShot3D(Vector3 pos)
    {
        if (meteorRainSound == null) return;

        var go = new GameObject("SFX_Meteor");
        go.transform.position = pos;

        var src = go.AddComponent<AudioSource>();
        src.clip = meteorRainSound;
        src.volume = volume;
        src.pitch = Random.Range(pitchRange.x, pitchRange.y);
        src.spatialBlend = 1f;          // 3D
        src.rolloffMode = AudioRolloffMode.Linear;
        src.maxDistance = 30f;

        src.Play();
        Destroy(go, meteorRainSound.length / Mathf.Max(0.01f, src.pitch) + 0.1f);
    }

    public void IncreaseRadius(float amount)
    {
        radius = Mathf.Max(radius + amount, 20f);
    }

    public void DecreaseCooldown(float amount)
    {
        coolDown = Mathf.Max(coolDown - amount, 1f);
    }   

    public void IncreaseLifeTime(float amount)
    {
        lifeTime = Mathf.Max(lifeTime + amount, 10f);
    }

}
