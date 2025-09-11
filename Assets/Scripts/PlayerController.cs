using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public InputActionReference selectFire;

    public GameObject handShootGameObject;

    public GameObject bulletPrefab;

    public Killable mKillable;


    void Awake()
    {
        mKillable = GetComponent<Killable>();
        if (mKillable == null)
        {
            Debug.LogError("Killable component not found!");
        }
    }
    private void OnEnable()
    {
        selectFire.action.performed += Fire;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Fire(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 dir = Camera.main.transform.forward;

        Vector3 target = Camera.main.transform.position + dir * 100f;


        if (Physics.Raycast(ray, out var hit, 200f))
        {
            target = hit.point;
        }

        Vector3 shotDir = (target - handShootGameObject.transform.position).normalized;
        Quaternion shotRotation = Quaternion.LookRotation(shotDir);
        var bullet = Instantiate(bulletPrefab, handShootGameObject.transform.position, shotRotation);
        var killableComponent = GetComponent<Killable>();
        var bulletComponent = bullet.GetComponent<Bullet>();


        bulletComponent.setOrigin(gameObject);

        if (killableComponent)
        {
            bulletComponent.setDamage(killableComponent.GetDamage());
        }

        killableComponent.SetTeam(mKillable.GetTeam());

        // Instantiate and fire the bullet

    }
}
