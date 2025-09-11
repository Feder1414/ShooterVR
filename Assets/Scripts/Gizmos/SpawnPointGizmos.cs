using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnPointGizmos : MonoBehaviour
{
    // Start is called before the first frame update
    public float debugRadius = 1f;

    public string identifier = "Spawn Point";

    public Color color = Color.red;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, debugRadius);

        #if UNITY_EDITOR
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        Handles.Label(transform.position + Vector3.up * 0.5f, identifier, style);
        #endif
        
    }
}
