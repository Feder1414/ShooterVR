using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.GrabAPI;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.XR.Hands;

public class FollowJoint : MonoBehaviour
{
    // Start is called before the first frame update

    public XRHandSkeletonDriver skeleton;
    [SerializeField] XRHandJointID jointId = XRHandJointID.Palm;



    void Start()
    {
        var palm = GetJointTransform();

        gameObject.transform.SetParent(palm, worldPositionStays: false);



    }

    // Update is called once per frame
    void Update()
    {

    }

    Transform GetJointTransform()
    {
        if (skeleton == null)
        {
            Debug.Log("Skeleton not found error");
            return null;

        }

        foreach (var joint in skeleton.jointTransformReferences)
        {
            if (joint.xrHandJointID == jointId)
            {
                return joint.jointTransform;
            }
        }

        

        return null;
    }
}
