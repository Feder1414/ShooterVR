using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    public Transform cam;
    public float distance = 1.2f;
    public Vector3 offset; // ej. (0,-0.1f,0)

    void Awake(){ if(!cam) cam = Camera.main.transform; }
    void LateUpdate(){
        if(!cam) return;
        transform.position = cam.position + cam.forward*distance + offset;
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.position, Vector3.up);
  }
}
