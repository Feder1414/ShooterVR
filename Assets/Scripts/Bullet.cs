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


    [SerializeField] GameObject hitEffect;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mKillable = GetComponent<Killable>();
        if (mKillable == null)
        {
            Debug.LogError("Killable component not found");
        }

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
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        }
        Destroy(gameObject);
    }

    public void setDamage(int damage)
    {

    }

    public void ChangedShotSpeed(Killable killable, float newShootSpeed)
    {
        rb.velocity = transform.forward * newShootSpeed;
    }
    


}
