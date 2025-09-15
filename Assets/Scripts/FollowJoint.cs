using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;

public class FollowJoint : MonoBehaviour
{
    public XRHandSkeletonDriver skeleton;
    [SerializeField] XRHandJointID jointId = XRHandJointID.Palm;

    [Header("Local offsets (opcional)")]
    public Vector3 localPositionOffset = new Vector3(0f, 0f, 0.07f); // 7 cm delante de la palma
    public Vector3 localEulerOffset = Vector3.zero;                   // ajusta si quieres que mire al frente

    Transform joint;

    void OnEnable() => StartCoroutine(AttachWhenReady());

    IEnumerator AttachWhenReady()
    {
        // Espera hasta que el skeleton tenga referencias válidas
        while ((joint = GetJointTransform()) == null)
            yield return null;

        // Parent en espacio local y reset
        transform.SetParent(joint, false);              // ahora "false" = trabajaremos en local
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // Aplica offsets cómodos
        transform.localPosition += localPositionOffset;
        transform.localRotation *= Quaternion.Euler(localEulerOffset);
    }

    Transform GetJointTransform()
    {
        if (skeleton == null) return null;
        foreach (var j in skeleton.jointTransformReferences)
            if (j.xrHandJointID == jointId)
                return j.jointTransform;
        return null;
    }

    // Si el provider aplica la pose en LateUpdate, no hace falta nada más.
    // Si vieras "jitter", podrías forzar aquí una corrección:
    void LateUpdate()
    {
        if (joint == null) return;
        // Si por alguna razón te quitan el parent, re-aplica offsets:
        if (transform.parent != joint)
        {
            transform.position = joint.position;
            transform.rotation = joint.rotation * Quaternion.Euler(localEulerOffset);
        }
    }
}
