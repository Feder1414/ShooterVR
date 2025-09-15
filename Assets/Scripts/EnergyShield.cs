using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;


public class EnergyShield : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float limitTime = 5f;
    [SerializeField] float rechargeTime = 10f;

    private float timer = 0f;

    public bool isActive;

    [SerializeField] Renderer rend;
    MaterialPropertyBlock mbp;
    [SerializeField] Color colorTarget = Color.red;

    Color baseColorInitial = Color.white;




    void Awake()
    {
        isActive = true;
        mbp = new MaterialPropertyBlock();

        var mat = rend.sharedMaterial != null ? rend.sharedMaterial : rend.material;
        if (mat && mat.HasProperty("baseColorFactor"))
            baseColorInitial = mat.GetColor("baseColorFactor");
        else
            Debug.LogWarning("EnergyShield: 'baseColorFactor' no existe en el shader.");

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
        TimerForCooldown();
        UpdateShieldColor();


    }

    void TimerForCooldown()
    {

        if (timer < limitTime && isActive)
        {
            timer += Time.deltaTime;


        }

        if (timer >= limitTime && isActive)
        {
            isActive = false;
            timer = 0f;

        }

        if (!isActive && timer < rechargeTime)
        {
            timer += Time.deltaTime;
        }

        if (timer >= rechargeTime && !isActive)
        {
            isActive = true;
            timer = 0f;
        }

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
        if (!isActive)
        {
            Debug.Log("Shield is down");
            return;
        }

        var killableBullet = bullet.GetComponent<Killable>();

        if (!killableBullet)
        {
            Debug.LogWarning("Bullet has no Killable component");
            return;
        }

        if (killableBullet.GetTeam() == Killable.Team.Player)
            return;


        var bulletComponent = bullet.GetComponent<Bullet>();

        var shooterEnemy = bulletComponent.GetOrigin();
        
        killableBullet.SetTeam(Killable.Team.Player);


        var enemyDirection = (shooterEnemy.transform.position - transform.position).normalized;
        var rbBullet = bullet.GetComponent<Rigidbody>();
        rbBullet.velocity = enemyDirection * rbBullet.velocity.magnitude;

        Debug.Log("Bullet deflected");




    }

    void UpdateShieldColor()
    {


        if (!rend) return;


        float t = isActive ? Mathf.Clamp01(timer / limitTime)
                        : 1f - Mathf.Clamp01(timer / rechargeTime);

        Color baseColor = mbp.GetColor("baseColorFactor");
        Color lerpColor = Color.Lerp(baseColor, colorTarget, t);
        rend.GetPropertyBlock(mbp);
        mbp.SetColor("baseColorFactor", lerpColor);
        rend.SetPropertyBlock(mbp);
    }

    
    
   

    



}
