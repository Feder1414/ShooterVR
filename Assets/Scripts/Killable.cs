using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{

    public enum Team { Player, Enemy };

    [SerializeField] Team team;

    [SerializeField] int life = 10;
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

    public void factorIncreaseLife(float factor)
    {
        life = Mathf.FloorToInt(life * factor);
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate = Mathf.Max(0.2f, fireRate - amount); // Disminuir el tiempo entre disparos

    }

    public void IncreaseFactorDamage(float factor)
    {
        damage = Mathf.Min(Mathf.FloorToInt(damage * factor), 500); // Limitar el daño a un máximo de 100
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
        life += amount;
        OnHealed?.Invoke(this, amount);

        Debug.Log(gameObject.name + " Healed: " + amount + " New Life: " + life);

        Instantiate(healEffectPrefab, transform.position, Quaternion.identity);

    }


    public float SetShootSpeed(float newShootSpeed)
    {
        shootSpeed = newShootSpeed;
        OnShootSpeedChanged?.Invoke(this, shootSpeed);
        return shootSpeed;
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
