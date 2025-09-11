using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] LineRenderer aimLine;
    [SerializeField] float aimDuration;
    [SerializeField] Transform firePoint;

    [SerializeField] GameObject bulletPrefab;

    private GameObject player;

    private Killable killable;
    [SerializeField] AnimationCurve lockCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] float swayFreq;
    [SerializeField] float maxSwayAngle;


    void Awake()
    {
        killable = GetComponent<Killable>();
        if (killable == null)
        {
            Debug.LogError("Killable component not found");
        }

        player = GameObject.FindGameObjectWithTag("MainCamera");

        if (player == null)
        {
            Debug.LogError("Player not found");
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

                aimLine.SetPosition(0, firePoint.position);
                aimLine.SetPosition(1, firePoint.position + finalDir * 100f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            aimLine.enabled = false;
            Vector3 shotDir = (player.transform.position - transform.position).normalized;

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

}
