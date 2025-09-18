using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SpaceShipEnemyController : MonoBehaviour
{
    // Start is called before the first frame update

    private float movementDuration;
    [SerializeField] float pauseAtEnds = 0.25f;

    [SerializeField] AnimationCurve easeCurve = null;

    [SerializeField] float maxPitchDeg = 10f;

    [SerializeField] float tiltSensitivity = 2f;

    [SerializeField] float tiltSmooth = 8f;

    private Killable killable;

    [SerializeField] float distanceToTravel = 5.0f;

    [SerializeField] float maxX = 10.0f;



    float leftX, rightX;
    Vector3 center;
    Quaternion baseRot;
    float currentPitch;



    void Start()
    {
        StartCoroutine(Patrol());


    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {

        killable = GetComponent<Killable>();


        if (killable == null)
        {
            Debug.LogError("Killable component not found!");
        }

        center = transform.position;
        baseRot = transform.rotation;

        killable.OnDied += _ => OnDie();


        movementDuration = distanceToTravel / killable.GetSpeed();

        Debug.Log("Movement duration: " + movementDuration + " for object " + gameObject.name);

        if (easeCurve == null || easeCurve.length < 2)
        {
            easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }




    }

    IEnumerator Move(bool right)
    {
        float startPositionX = transform.position.x;
        float targetPositionX = right ? startPositionX + distanceToTravel : startPositionX - distanceToTravel;

        float timePassed = 0;

        while (timePassed < movementDuration)
        {
            float factor = timePassed / movementDuration;
            float factorMovement = easeCurve.Evaluate(factor);
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPaused)
            {
                Debug.Log("Factor: " + factor + " evaluated to " + factorMovement);
            }

#endif
            float newX = Mathf.Lerp(startPositionX, targetPositionX, factorMovement);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPaused)
            {
                Debug.Log("Moving to " + newX);
            }
#endif

            yield return null;
            timePassed += Time.deltaTime;

        }

        transform.position = new Vector3(targetPositionX, transform.position.y, transform.position.z);


    }

    IEnumerator Patrol()
    {
        while (true)
        {   
            if (transform.position.x > maxX)
            {
                yield return Move(false);
            }
            else if (transform.position.x < -maxX)
            {
                yield return Move(true);
            }
            else
            {
                float probability = UnityEngine.Random.value;
                if (probability < 0.5f)
                {
                    yield return Move(true);
                }
                else
                {
                    yield return Move(false);
                }
            }
            yield return new WaitForSeconds(pauseAtEnds);
        }
    }


    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDie()
    { 
        Destroy(gameObject);
    }
    



    

}