using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Killable killable;

    void Awake()
    {
        if (killable == null)
        {
            killable = GetComponentInParent<Killable>();
        }

        if (killable == null)
        {
            Debug.LogError("Killable component not found in parent of " + gameObject.name);
        }
        killable.OnDied += _ => OnDie();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnDie()
    {
        Destroy(gameObject);
    }
}
