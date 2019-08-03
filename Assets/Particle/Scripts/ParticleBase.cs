using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class ParticleBase : MonoBehaviour
{
    public ComputeShader computeShader;

    [HideInInspector]
    public int kernel;
    public virtual void Init()
    {
        Debug.Log("initalizing " + gameObject.name + " | " + computeShader.name);
        kernel = computeShader.FindKernel("CSMain");
    }
    public virtual void PreDispatch(CommandBuffer commanBuffer)
    {
        Debug.Log("predispatching " + gameObject.name + " | " + computeShader.name);

    }
    public virtual void OnDisable()
    {
        Debug.Log("diabling " + gameObject.name + " | " + computeShader.name);
    }

}
