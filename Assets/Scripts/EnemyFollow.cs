using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyFollow : MonoBehaviour
{
    // Start is called before the first frame update



    private Transform player;


    private Animator animator;
    private Killable killable;

    private Rigidbody rb;




    public float stopDistance = 0.5f;



    public bool touchingPlayer = false;

    public bool isKnocked = false;

    [SerializeField] bool makeContactDamage = true;

    private float knockEndTime;


    void Awake()
    {
        killable = GetComponent<Killable>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found in children of " + gameObject.name);
        }
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        killable = GetComponent<Killable>();

    }
    private void OnEnable()
    {
        killable.OnDied += Die;
        killable.OnDamaged += OnTakeDamage;

    }

    void Start()
    {
        
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        FollowPlayer();


    }


    public void ApplyKnockback(Vector3 impulseDir, float impulse, float duration)
    {

        isKnocked = true;
        knockEndTime = Time.time + duration; // Duración del knockback

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(impulseDir * impulse, ForceMode.Impulse);
    }
    private void FollowPlayer()
    {



        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        //var st = animator.GetCurrentAnimatorStateInfo(0);
        //bool inWalk = st.IsName("Walk") && !animator.IsInTransition(0);
        if (isKnocked)
        {
            if (Time.time >= knockEndTime)
            {
                isKnocked = false;
            }
            animator.SetBool("IsMoving", false);
            return;
        }

        bool shouldMove = !touchingPlayer && Vector3.Distance(transform.position, player.position) > stopDistance;


        animator.SetBool("IsMoving", shouldMove);

        if (!shouldMove)
        {

            return;
        }



        //rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
        //rb.position = rb.position + direction * killable.GetSpeed() * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + direction * killable.GetSpeed() * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), killable.GetTurnSpeed() * Time.fixedDeltaTime);


    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Collided with Player");
            var killableComponent = collision.gameObject.GetComponent<Killable>();
            if (killableComponent != null && makeContactDamage)
            {
                killableComponent.TakeDamage(killable.GetDamage());
            }

            if (killableComponent == null)
            {
                Debug.LogWarning("The player does not have a Killable component.");
            }
            

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Stopped Colliding with Player");
            touchingPlayer = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            touchingPlayer = true;
        }
    }


    private void Die(Killable k)
    {
        Destroy(gameObject);
    }

    private void OnEnable(Killable k)
    {
        if (killable != null)
        {
            killable.OnDied += Die;
        }
    }

    private void OnTakeDamage(Killable k, int damage)
    {
        Debug.Log(gameObject.name + " took " + damage + " damage.");
    }

}
