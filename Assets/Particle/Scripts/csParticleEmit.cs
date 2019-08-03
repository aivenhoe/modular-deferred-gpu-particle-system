using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public class csParticleEmit : ParticleBase
{
    private Vector3[] emitterPositions;
    private Vector3 center_old;
    public GameObject EmittingMesh;
    private ComputeBuffer emitterPosBuffer;
    public int csEmitterParticlePerFrame = 200;
    public Vector3 csEmitterEmitlVelocity = new Vector3(0f, 0.7f, 0f);
    public float velocityFactor = -0.05f;
    public override void Init()
    {
        base.Init();  
        emitterPositions = EmittingMesh.GetComponent<MeshFilter>().sharedMesh.vertices;
        emitterPositions = transformLocalToWorldVertices(emitterPositions);
        emitterPosBuffer = new ComputeBuffer(emitterPositions.Length, Marshal.SizeOf(emitterPositions.GetType().GetElementType()));
        emitterPosBuffer.SetData(emitterPositions);
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to emit cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initAccBuffer", ParticleManager.initAccBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initMassBuffer", ParticleManager.initMassBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "initColBuffer", ParticleManager.initColBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "emitterPosBuffer", emitterPosBuffer);
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        emitterPosBuffer.Release();
    }
    void Update()
    {
        Vector3 velocity = ((EmittingMesh.transform.localPosition - center_old) / Time.deltaTime) * velocityFactor;
       
        computeShader.SetInt("_FrameCounter", Time.frameCount);
        computeShader.SetInt("_EmitterCount", emitterPositions.Length);
        computeShader.SetInt("_ParticlePerFrame", csEmitterParticlePerFrame);
        computeShader.SetInt("_MaxParticles", ParticleManager.ParticleCount);
        computeShader.SetVector("_InitialVelocity", csEmitterEmitlVelocity);

        emitterPositions = EmittingMesh.GetComponent<MeshFilter>().sharedMesh.vertices;
        emitterPositions = transformLocalToWorldVertices(emitterPositions);
        emitterPosBuffer.SetData(emitterPositions);
        computeShader.SetBuffer(kernel, "emitterPosBuffer", emitterPosBuffer);
        computeShader.Dispatch(kernel, ParticleManager.ParticleCount / 240, 1, 1);
        center_old = EmittingMesh.transform.localPosition;

    }
    public Vector3[] transformLocalToWorldVertices(Vector3[] meshVertices)
    {
        for (int i = 0; i < meshVertices.Length; i++)
            meshVertices[i] = EmittingMesh.transform.TransformPoint(meshVertices[i]);
        return meshVertices;
    }
}

