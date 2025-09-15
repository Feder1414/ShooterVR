using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HealerEnemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] int healAmount = 5;
    [SerializeField] float healRate = 2f;

    [SerializeField] float maxConcurrentHeals = 3f;

    [SerializeField] float healRadius = 50f;

    [SerializeField] int healLayer = 0;

    private Killable killable;

    private Animator animator;

    public bool IsHealing = false;

    const string HealState = "Heal";


    void Awake()
    {
        killable = GetComponent<Killable>();
        animator = GetComponentInChildren<Animator>();

        if (killable == null)
        {
            Debug.LogError("Killable component not found in " + gameObject.name);
        }
        if (animator == null)
        {
            Debug.LogError("Animator component not found in children of " + gameObject.name);
        }


    }

    void OnEnable()
    {
        StartCoroutine(HealOverTime());
    }

    void OnDisable()
    {
        StopCoroutine(HealOverTime());
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator HealOverTime()
    {
        while (true)
        {

            List<GameObject> enemiesToHeal = new List<GameObject>();
            if (!hasTargetsToHeal(enemiesToHeal))
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }
            animator.SetBool("IsHealing", true);
            IsHealing = true;

            while (!animator.GetCurrentAnimatorStateInfo(healLayer).IsName(HealState)) yield return null;
            // espera a que termine (si el clip no es loop)
            for (int i = 0; i < Mathf.Min(enemiesToHeal.Count, maxConcurrentHeals); i++)
            {
                var enemy = enemiesToHeal[i];
                if (enemy == null) continue;
                var killableEnemy = enemy.GetComponent<Killable>();
                if (killableEnemy != null)
                { 
                        killableEnemy.Heal(healAmount);
                }
            
            }

            while (animator.GetCurrentAnimatorStateInfo(healLayer).normalizedTime < 1f) yield return null;

            animator.SetBool("IsHealing", false);
            IsHealing = false;
            
            yield return new WaitForSeconds(healRate);



        }
    }

    bool hasTargetsToHeal(List<GameObject> enemiesToHeal)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, healRadius);

        foreach (var collider in colliders)
        {
            var killableTarget = collider.GetComponent<Killable>();
            if (killableTarget != null && killableTarget != killable && killableTarget.GetTeam() == killable.GetTeam())
            {
                enemiesToHeal.Add(collider.gameObject);
            }
        }
        Debug.Log(gameObject.name + " Found " + enemiesToHeal.Count + " allies to heal.");
        if (enemiesToHeal.Count == 0) return false;
        enemiesToHeal.Sort((a, b) =>
        {
            var killableA = a.GetComponent<Killable>();
            var killableB = b.GetComponent<Killable>();
            return killableA.GetLife().CompareTo(killableB.GetLife());
        });

        
        
        return true;
    }


}
