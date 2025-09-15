using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBar : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Image bar;

    [SerializeField] MonoBehaviour cooldownSource;
    IHasCooldown hasCooldown;



    [SerializeField] Color cooling = Color.gray;
    [SerializeField] Color ready   = Color.cyan;

    void Start()
    {

    }

     void Awake()
    {
        hasCooldown = cooldownSource as IHasCooldown;
        if (hasCooldown == null)
            Debug.LogError("El objeto asignado no implementa IHasCooldown.", this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        if (hasCooldown != null)
        {
            hasCooldown.OnCooldownStarted += StartCooldown;
        }
        else
        {
            Debug.LogError("IHasCooldown reference is not assigned in ChargeBar.");
        }
    }

    void OnDisable()
    {
        if (hasCooldown != null)
        {
            hasCooldown.OnCooldownStarted -= StartCooldown;
        }
    }

    void StartCooldown(float cooldown)
    {
        StopAllCoroutines();
        StartCoroutine(CooldownRoutine(cooldown));
    }

    IEnumerator CooldownRoutine(float cooldown)
    {
        float elapsed = 0f;
        bar.fillAmount = 0f;
        bar.color = cooling;
        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;
            bar.fillAmount = elapsed / cooldown;
            yield return null;
        }
        bar.color = ready;
        
    }


    

}
