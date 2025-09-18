using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;


public static class MaterialTools
{
    // [MenuItem("Tools/Make Selected Materials Transparent")]
    // static void MakeTransparent()
    // {
    //     foreach (var obj in Selection.objects)
    //     {
    //         if (obj is Material mat)
    //             MakeMatTransparent(mat);
    //     }
    // }

    // public static void MakeMatTransparent(Material mat)
    // {
    //     // Mezcla alfa clásica
    //     mat.SetOverrideTag("RenderType", "Transparent");
    //     mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
    //     mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
    //     mat.SetInt("_ZWrite", 0);
    //     mat.DisableKeyword("_ALPHATEST_ON");
    //     mat.EnableKeyword("_ALPHABLEND_ON");
    //     mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
    //     mat.renderQueue = (int)RenderQueue.Transparent;

    //     // Hints para URP Shader Graph (si existen)
    //     if (mat.HasProperty("_Surface"))        mat.SetFloat("_Surface", 1f); // 0=Opaque,1=Transparent
    //     if (mat.HasProperty("_AlphaClip"))      mat.SetFloat("_AlphaClip", 0f);
    //     if (mat.HasProperty("_Cull"))           mat.SetFloat("_Cull", (int)CullMode.Off); // opcional: doble cara
    //     // Keywords nuevas URP (no pasa nada si no existen)
    //     mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    //     mat.DisableKeyword("_ALPHATEST_ON");
    // }
}
