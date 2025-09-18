using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaGizmo : MonoBehaviour
{
    public Vector3 areaCenter;
    public Vector3 areaSize;

    public Color gizmosColor = Color.green;
    
    void OnDrawGizmos()
    {
        if (areaCenter == null) return;

        Gizmos.color = gizmosColor;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}
