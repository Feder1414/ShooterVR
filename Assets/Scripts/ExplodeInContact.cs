using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeInContact : MonoBehaviour
{
    // Start is called before the first frame update

    public ParticleSystem particleSystem;

    private Killable mKillable;

    void Start()
    {
        mKillable = GetComponent<Killable>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerKillable = collision.gameObject.GetComponent<Killable>();
            if (playerKillable != null)
            {
                Debug.Log("Damaging player " + collision.gameObject.name);
                playerKillable.TakeDamage(mKillable.GetDamage());
            }
            Explode();
        }
    }

    void Explode()
    {
        var explosion = Instantiate(particleSystem, transform.position, Quaternion.identity);
        explosion.Play();
        Destroy(gameObject);
    }

}


