using UnityEngine;
using UnityEngine.Rendering;






[RequireComponent(typeof(Renderer))]
public class ShieldTransparency : MonoBehaviour
{
    [Range(0,1)] public float alpha = 0.3f;
    public Color tint = Color.cyan;

    void Awake()
    {
        var mat = GetComponent<Renderer>().material;
        MaterialTools.MakeMatTransparent(mat);
        tint.a = alpha;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", tint);
        else                               mat.color = tint; // Standard
    }
}
