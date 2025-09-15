using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update





    public float lifetime = 5f;


    private Killable mKillable;

    private GameObject origin;

    private Rigidbody rb;
    private float deathAt;

    private AudioSource audioSource;


    [SerializeField] GameObject hitEffect;

    [SerializeField] AudioClip hitSound;

    [SerializeField] AudioClip shootSound;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mKillable = GetComponent<Killable>();
        if (mKillable == null)
        {
            Debug.LogError("Killable component not found");
        }
        audioSource = gameObject.AddComponent<AudioSource>();

    }
    void OnEnable()
    {
        deathAt = Time.time + lifetime;
        rb.velocity = transform.forward * mKillable.GetShootSpeed();
        mKillable.OnShootSpeedChanged += ChangedShotSpeed;

    }

    void OnDisable()
    {
        mKillable.OnShootSpeedChanged -= ChangedShotSpeed;
    }

    void Start()
    {
        PlayShootSound();

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > deathAt)
        {
            Destroy(gameObject);
        }

    }

    public void setOrigin(GameObject gameObject)
    {
        origin = gameObject;
    }

    public GameObject GetOrigin()
    {
        return origin;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != origin)
        {
            Killable killable = other.gameObject.GetComponent<Killable>();
            if (killable != null)
            {
                if (killable.GetTeam() != mKillable.GetTeam())
                {
                    killable.TakeDamage(mKillable.GetDamage());
                    //Destroy(gameObject);

                    HandleDestroy(other.gameObject);
                }
            }
            //Destroy(gameObject);

        }








    }

    void HandleDestroy(GameObject other)
    {
        if (hitEffect && !other.CompareTag("Player"))
        {
            var hitEffectInstance = Instantiate(hitEffect, transform.position, Quaternion.identity);
            PlayHitSound();
            Destroy(hitEffectInstance, 2f);
        }
        Destroy(gameObject);
    }

    public void setDamage(int damage)
    {

    }

    void PlayHitSound()
    {
        if (audioSource != null && mKillable != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    public void ChangedShotSpeed(Killable killable, float newShootSpeed)
    {
        rb.velocity = transform.forward * newShootSpeed;
    }

    void PlayShootSound()
    {
        if (audioSource != null && mKillable != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }



}
