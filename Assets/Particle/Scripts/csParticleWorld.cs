using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class csParticleWorld : ParticleBase
{

    public float Gravity = -0.005f;
    public float ColorModifierFactor = 0.3f;
    public override void Init()
    {
        base.Init();
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to World cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initAccBuffer", ParticleManager.initAccBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initMassBuffer", ParticleManager.initMassBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initColBuffer", ParticleManager.initColBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    void Update()
    {
        computeShader.SetFloat("_Gravity", Gravity);
        computeShader.SetFloat("_ColorModifierFactor", ColorModifierFactor);
    }
}
