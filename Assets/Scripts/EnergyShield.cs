using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;


public class EnergyShield : MonoBehaviour
{
    // Start is called before the first frame update




    [SerializeField] GameObject player;



    void Awake()
    {
        if (!player)
        {
            player = FindObjectOfType<PlayerController>().gameObject;
            if (!player)
            {
                Debug.LogError("Player not found in the scene for EnergyShield.");
            }
        }

    }

    void OnEnable()
    {


    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    void TimerForCooldown()
    {

    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Bullet>() != null)
        {
            Debug.Log("Shield hit by bullet");
            BlockShoot(other.gameObject);
        }
    }
    void BlockShoot(GameObject bullet)
    {
       
        var killableBullet = bullet.GetComponent<Killable>();

        if (killableBullet.GetTeam() == Killable.Team.Player)
            return;


        var bulletComponent = bullet.GetComponent<Bullet>();

        var shooterEnemy = bulletComponent.GetOrigin();
        
        killableBullet.SetTeam(Killable.Team.Player);


        var enemyDirection = (shooterEnemy.transform.position - transform.position).normalized;
        var rbBullet = bullet.GetComponent<Rigidbody>();
        
        var playerKillable = player.GetComponent<Killable>();

        killableBullet.setDamage(playerKillable.GetDamage()*3);
        
        rbBullet.velocity = enemyDirection * rbBullet.velocity.magnitude;

        Debug.Log("Bullet deflected");

    }

    
    
   

    



}
