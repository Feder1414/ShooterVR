using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] LineRenderer aimLine;
    [SerializeField] Material aimLineMaterial;
    [SerializeField] float aimDuration;
    [SerializeField] Transform firePoint;

    [SerializeField] GameObject bulletPrefab;

    private GameObject player;

    private Killable killable;
    [SerializeField] AnimationCurve lockCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] float swayFreq;
    [SerializeField] float maxSwayAngle;


    void Awake()
    {
        killable = GetComponent<Killable>();
        if (killable == null)
        {
            Debug.LogError("Killable component not found");
        }
        killable.OnDied += _ => OnDie();

        player = GameObject.FindGameObjectWithTag("MainCamera");

        if (player == null)
        {
            Debug.LogError("Player not found");
        }

        if (aimLine == null)
        {
            Debug.LogError("Aim Line Renderer not assigned");
        }
        else
        {
            aimLine.material = aimLineMaterial;
        }

    }
    void Start()
    {


    }

    void OnEnable()
    {
        StartCoroutine(AimAndShoot());
    }

    void OnDisable()
    {
        StopCoroutine(AimAndShoot());
    }

    // Update is called once per frame
    void Update()
    {


    }


    IEnumerator AimAndShoot()
    {
        Debug.Log($"[{name}] Starting AimAndShoot coroutine");
        while (true)
        {


            aimLine.enabled = true;

            float elapsedTime = 0f;
            float t = 0f;
            while (elapsedTime < aimDuration)
            {

                t = elapsedTime / aimDuration;
                float u = Mathf.Clamp01(t);
                float lockFactor = lockCurve.Evaluate(u);
                float sway = (1f - lockFactor) * 0.5f;

                Vector3 idealDir = (player.transform.position - transform.position).normalized;

                Quaternion q = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(idealDir, Vector3.up), lockFactor);

                float a = maxSwayAngle * sway;
                float tNow = Time.time * swayFreq;
                Quaternion wobble =
                    Quaternion.AngleAxis(Mathf.Sin(tNow) * a, firePoint.up) *
                    Quaternion.AngleAxis(Mathf.Cos(tNow) * a, firePoint.right);

                Vector3 finalDir = q * wobble * Vector3.forward;



                aimLine.SetPosition(0, Vector3.zero);
                aimLine.SetPosition(1, Vector3.forward * 100f);

                //firePoint.rotation = Quaternion.LookRotation(finalDir, Vector3.up);
                transform.rotation = Quaternion.LookRotation(finalDir, Vector3.up);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            aimLine.enabled = false;
            Vector3 shotDir = (player.transform.position - firePoint.position).normalized;

            // Fire the bullet
            var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shotDir));
            var bulletComponent = bullet.GetComponent<Bullet>();
            var bulletKillable = bullet.GetComponent<Killable>();
            bulletKillable.SetShootSpeed(killable.GetShootSpeed());
            bulletComponent.setOrigin(gameObject);
            bulletKillable.SetTeam(killable.GetTeam());

            yield return new WaitForSeconds(killable.GetFireRate());
        }

    }
    
    void OnDie()
    {
        Destroy(gameObject);
     
    }

}
