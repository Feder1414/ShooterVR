using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject player;

    [SerializeField] float healthIncreaseFactor = 1.05f; // 5% increase

    [SerializeField] float fireRateIncreaseAmount = 0.1f; // Amount to increase fire rate

    [SerializeField] float damageIncreaseFactor = 1.1f; // 10% increase

    [SerializeField] float knockbackIncreaseAmount = 3f; // Amount to increase knockback
    [SerializeField] float shockwaveRadiusIncreaseAmount = 0.25f; // Amount to increase shockwave radius

    [SerializeField] float shockwaveCooldownDecreaseAmount = 0.05f; // Amount to decrease shockwave cooldown

    [SerializeField] float tickRateFireRainDecreaseAmount = 0.1f; // Amount to decrease tick rate of Fire Rain

    [SerializeField] float radiusFireRainIncreaseAmount = 0.5f; // Amount to increase radius of Fire Rain

    [SerializeField] float decreaseCooldownFireRain = 0.5f; // Minimum tick rate for Fire Rain

    [SerializeField] float increaseFireRainLifetime = 0.3f; // Amount to increase lifetime of Fire Rain

    public event System.Action OnbuffApplied;


    private Killable playerKillable;
    private RockAEO playerRockAOE;
    private ShockWave playerShockWave;





    void Awake()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is not assigned in BuffManager.");
        }
        playerKillable = player.GetComponent<Killable>();
        if (playerKillable == null)
        {
            Debug.LogError("Killable component not found on Player in BuffManager.");
        }
        playerRockAOE = player.GetComponent<RockAEO>();
        if (playerRockAOE == null)
        {
            Debug.LogError("RockAEO component not found on Player in BuffManager.");
        }
        playerShockWave = player.GetComponent<ShockWave>();
        if (playerShockWave == null)
        {
            Debug.LogError("ShockWave component not found on Player in BuffManager.");
        }
    }

    public void IncreasePlayerHealth()
    {
        if (playerKillable != null)
        {
            playerKillable.factorIncreaseLife(healthIncreaseFactor); // Heal 5 health points
        }

        OnbuffApplied?.Invoke();

    }

    public void IncreasePlayerFireRate()
    {
        if (playerKillable != null)
        {
            playerKillable.IncreaseFireRate(fireRateIncreaseAmount);
        }
        OnbuffApplied?.Invoke();
    }

    public void IncreasePlayerDamage()
    {
        if (playerKillable != null)
        {
            playerKillable.IncreaseFactorDamage(damageIncreaseFactor); // Increase damage by 10%
        }
        OnbuffApplied?.Invoke();

    }

    public void IncraseKnockback()
    {
        if (playerShockWave != null)
        {
            playerShockWave.forceMagnitude = Mathf.Min(playerShockWave.forceMagnitude + knockbackIncreaseAmount, 300f); // Increase knockback by 3%
        }
        OnbuffApplied?.Invoke();
    }

    public void IncreaseShockwaveRadius()
    {
        if (playerShockWave != null)
        {
            playerShockWave.radius = Mathf.Min(playerShockWave.radius + shockwaveRadiusIncreaseAmount, 10f); // Increase radius by 0.25
        }
        OnbuffApplied?.Invoke();
    }

    public void DecreaseShockWaveCooldown()
    {
        if (playerShockWave != null)
        {
            playerShockWave.cooldown = Mathf.Max(playerShockWave.cooldown - shockwaveCooldownDecreaseAmount, 0.1f); // Decrease cooldown by 0.05s
        }
        OnbuffApplied?.Invoke();
    }

    public void DecreaseTickRateFireRain()
    {
        if (playerRockAOE != null)
        {
            playerRockAOE.tickRate = Mathf.Max(playerRockAOE.tickRate - tickRateFireRainDecreaseAmount, 0.1f); // Decrease tick rate by 0.1s
        }
        OnbuffApplied?.Invoke();
    }

    public void IncreaseRadiusFireRain()
    {
        if (playerRockAOE != null)
        {
            playerRockAOE.IncreaseRadius(radiusFireRainIncreaseAmount);
        }
        OnbuffApplied?.Invoke();

    }

    public void DecreaseCooldownFireRain()
    {
        if (playerRockAOE != null)
        {
            playerRockAOE.DecreaseCooldown(decreaseCooldownFireRain);
        }
        OnbuffApplied?.Invoke();
    }

    public void IncreaseLifeTimeFireRain()
    {
        if (playerRockAOE != null)
        {
            playerRockAOE.IncreaseLifeTime(increaseFireRainLifetime);
        }
        OnbuffApplied?.Invoke();
    }






    

    void Start()
    {

    }

    void Update()
    {

    }
    

    
}
