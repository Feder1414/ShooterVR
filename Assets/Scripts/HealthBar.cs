using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Killable target;
    [SerializeField] Image fill;

    [SerializeField] Transform billboard;

    [SerializeField] Transform cam;


    void Awake()
    {   

        if (target == null) target = GetComponentInParent<Killable>();
        if (!fill) fill = GetComponentInChildren<Image>();
        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        if (!billboard) billboard = transform;
        if (target)
        {
            target.OnLifeChanged += Refresh;
            target.OnDied += _ => gameObject.SetActive(false);
        }
       
    }

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned in HealthBar.");
        }

        if (fill == null)
        {
            Debug.LogError("Fill Image is not assigned in HealthBar.");
        }

        if (billboard == null)
        {
            Debug.LogError("Billboard Transform is not assigned in HealthBar.");
        }
        Refresh(target);
         

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Refresh(Killable _)
    {
        if (!target || !fill) return;
        float t = Mathf.Clamp01((float)target.GetLife() / Mathf.Max(1, target.GetBaseLife()));
        fill.fillAmount = t;
        // opcional: color por estado
        // fill.color = Color.Lerp(Color.red, Color.green, t);
    }
    
}
