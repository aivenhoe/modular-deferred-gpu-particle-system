using UnityEngine.Rendering;

public class csParticleFloor : ParticleBase
{
    public override void Init()
    {
        base.Init();
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to floor cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);
        
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    void Update()
    {


    }
}
