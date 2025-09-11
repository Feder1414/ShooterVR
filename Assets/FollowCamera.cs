using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform head;      // Main Camera
    [SerializeField] float bodyHeight = 1.7f;

    void LateUpdate()
    {
        Vector3 p = transform.position;
        p.x = head.position.x; p.z = head.position.z;
        p.y = bodyHeight * 0.5f;          // centro de la capsule
        transform.position = p;

        Vector3 f = head.forward;
        f.y = 0;
        if (f.sqrMagnitude > 1e-4) transform.rotation = Quaternion.LookRotation(f);
    }
}
