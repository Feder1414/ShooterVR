using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{


    public Transform shootStart;

    public GameObject bulletPrefab;

    private GameObject player;

    private Killable killable;




    // Start is called before the first frame update

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("MainCamera");
        killable = GetComponent<Killable>();

        if (killable == null)
        {
            Debug.LogError("Killable component not found in " + gameObject.name);
        }

        if (player == null)
        {
            Debug.LogError("Player not found in " + gameObject.name);
        }
    }
    void Start()
    {
        StartCoroutine(Shoot());

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(shootStart.position, (player.transform.position - shootStart.position).normalized * 20f, Color.blue, fireRate);

    }

    IEnumerator Shoot()
    {
        while (true)
        {
            Vector3 target = (player.transform.position - shootStart.position).normalized;

            //GameObject bullet = Instantiate(bulletPrefab, shootStart.position, Quaternion.LookRotation(target, Vector3.up));
            GameObject bullet = Instantiate(bulletPrefab, shootStart.position, Quaternion.LookRotation(target, Vector3.up));
            var bulletComponent = bullet.GetComponent<Bullet>();
            var bulletKillable = bullet.GetComponent<Killable>();
            bulletComponent.setOrigin(gameObject);
            bulletComponent.setDamage(killable.GetDamage());
            bulletKillable.SetTeam(killable.GetTeam());

            Debug.Log($"[{name}] fireRate={killable.GetFireRate()}  t={Time.time}");



            Debug.DrawRay(shootStart.position, target * 20f, Color.red, killable.GetFireRate());

            yield return new WaitForSeconds(killable.GetFireRate());

        }

    }
}

    


