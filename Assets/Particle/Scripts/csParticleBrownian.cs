using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class csParticleBrownian : ParticleBase
{
    public float BrownianStrength = 0.009f;

    public override void Init()
    {
        base.Init();
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to brownian cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "randomBuffer", ParticleManager.initRandomBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);
        
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    void Update()
    {
        computeShader.SetInt("_ParticleCount", ParticleManager.ParticleCount);
        computeShader.SetInt("_RandomIndex", Random.Range(0, 12000000));
        computeShader.SetFloat("_BrownianStrength", BrownianStrength);

    }
}
