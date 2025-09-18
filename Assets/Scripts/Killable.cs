using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Killable : MonoBehaviour
{

    public enum Team { Player, Enemy };

    [SerializeField] Team team;

    [SerializeField] int life;

    [SerializeField] int baseLife = 10;
    [SerializeField] int damage = 3;

    [SerializeField] float fireRate;
    [SerializeField] float speed = 1;

    [SerializeField] float turnSpeed = 1;


    [SerializeField] public float shootSpeed = 5f;

    [SerializeField] GameObject healEffectPrefab;


    public event Action<Killable> OnDied;

    public event Action<Killable, int> OnDamaged;

    public event Action<Killable, int> OnHealed;

    public event Action<Killable> OnLifeChanged;

    public event Action<Killable, float> OnShootSpeedChanged;

    // Start is called before the first frame update

    void Awake()
    {
        life = baseLife;
        if (healEffectPrefab == null)
        {
            Debug.LogError("Heal Effect Prefab is not assigned in " + gameObject.name);
        }
        StartCoroutine(CheckHeight());
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        life -= damage;
        Debug.Log("Damage taken: " + damage + " by " + gameObject.name);
        OnDamaged?.Invoke(this, damage);
        OnLifeChanged?.Invoke(this);

        if (life <= 0)
        {

            OnDied?.Invoke(this);
            //Die();
        }

        Debug.Log(gameObject.name + " Life: " + life);
    }


    // public void Die()
    // {
    //     if (gameObject.CompareTag("Enemy"))
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    public void FactorIncreaseLife(float factor)
    {
        baseLife = Mathf.CeilToInt(baseLife * factor);
        life = baseLife;
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate = Mathf.Max(0.2f, fireRate - amount); // Disminuir el tiempo entre disparos

    }

    public void IncreaseFactorDamage(float factor)
    {
        damage = Mathf.Min(Mathf.CeilToInt(damage * factor)); // Limitar el daño a un máximo de 100
    }

    public void IncreaseSpeed(float factor)
    {
        speed = Mathf.Min(speed + factor, 5f); 
    }

    



    public float setDamage(int newDamage)
    {
        damage = newDamage;
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public float GetTurnSpeed()
    {
        return turnSpeed;
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetFireRate()
    {
        return fireRate;
    }


    public float GetShootSpeed()
    {
        return shootSpeed;
    }

    public void Heal(int amount)
    {
        life = Mathf.Min(life + amount, baseLife);
        OnHealed?.Invoke(this, amount);
        OnLifeChanged?.Invoke(this);

        Debug.Log(gameObject.name + " Healed: " + amount + " New Life: " + life);

        Instantiate(healEffectPrefab, transform.position, Quaternion.identity);

    }


    public float SetShootSpeed(float newShootSpeed)
    {
        shootSpeed = newShootSpeed;
        OnShootSpeedChanged?.Invoke(this, shootSpeed);
        return shootSpeed;
    }

    public void IncreaseShootSpeed(float amount)
    {
        shootSpeed = Mathf.Min(shootSpeed + amount, 20f); // Limitar la velocidad de disparo a un máximo de 20
        OnShootSpeedChanged?.Invoke(this, shootSpeed);
    }

    public Team GetTeam()
    {
        return team;
    }

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
    }

    public int GetLife()
    {
        return life;
    }

    public int GetBaseLife()
    {
        return baseLife;
    }
    
    IEnumerator CheckHeight()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (transform.position.y < -20f)
            {
                OnDied?.Invoke(this);
                Destroy(gameObject);
                yield break;
            }
            
        }
    }

}
