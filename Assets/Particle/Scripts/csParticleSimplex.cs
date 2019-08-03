using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.Collections.Generic;
public class csParticleSimplex : ParticleBase
{
    public float SimplexStrength = 0.029f;
    public float Frequency = 0.029f;
    public float Amplitude = 0.029f;
    public GameObject EmittingMesh;
    public override void Init()
    {
        base.Init();
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to simplex cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    void Update()
    {
        computeShader.SetFloat("_SimplexStrength", SimplexStrength);
        computeShader.SetFloat("_Frequency", Frequency);
        computeShader.SetFloat("_Amplitude", Amplitude);
        computeShader.SetVector("_Center", EmittingMesh.transform.localPosition);

    }
}
