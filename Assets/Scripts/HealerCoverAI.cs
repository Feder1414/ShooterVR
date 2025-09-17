using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class HealerCoverAI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float coverDistance = 0.75f;
    [SerializeField] float repathEvery = 20f;

    [SerializeField] HealerEnemy healerEnemy;

    [SerializeField] Transform player;

    [SerializeField] Animator animator;

    [SerializeField] Killable mKillable;

    const string MoveState = "Move";

NavMeshAgent agent;
    float nextRepath;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found in " + gameObject.name);
        }
        healerEnemy = GetComponent<HealerEnemy>();
        if (healerEnemy == null)
        {
            Debug.LogError("HealerEnemy component not found in " + gameObject.name);
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player Transform not assigned in " + gameObject.name);
        }
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found in children of " + gameObject.name);
        }
        mKillable = GetComponent<Killable>();
        if (mKillable == null)
        {
            Debug.LogError("Killable component not found in " + gameObject.name);
        }
        agent.speed = mKillable.GetSpeed();

    }


    void Start()
    {
        
      
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) return;
        bool isMoving = !(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending);
        animator.SetBool("IsMoving", isMoving);


        if (healerEnemy.IsHealing) { agent.isStopped = true; return; }
        agent.isStopped = false;

        if (Time.time < nextRepath) return;
        nextRepath = Time.time + repathEvery;



        var tankEnemies = FindObjectsOfType<BirbTank>();

        if (tankEnemies.Length == 0) return;
        var tankEnemy = tankEnemies
            .OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
            .First().transform;


        Vector3 toPlayer = (player.position - tankEnemy.position).normalized;
        Vector3 desired = tankEnemy.position - toPlayer * coverDistance;


        if (NavMesh.SamplePosition(desired, out var hit, 1.0f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(desired);
            
        

            
    }
}
