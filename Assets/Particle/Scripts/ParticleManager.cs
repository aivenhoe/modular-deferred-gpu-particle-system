using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public class ParticleManager : MonoBehaviour
{
    public Material particleMaterial;

    private CommandBuffer computeShaderCommandBuffer;
    private CommandBuffer cameraCommandBuffer;
    private CommandBuffer lightCommandBuffer;

    public Camera camera;
    public Light light;
    public const int ParticleCount = 1500000;
    private const int ThreadGroupSizeXInShader = 128;
    public const int ThreadDispatchX = ParticleCount / ThreadGroupSizeXInShader;

    public static ComputeBuffer outputBuffer;
    public static ComputeBuffer initAccBuffer;
    public static ComputeBuffer initMassBuffer;
    public static ComputeBuffer initColBuffer;
    public static ComputeBuffer initRandomBuffer;

    public float ResetRange = 20f;
    public float initialAccelerationRange = 0.6f;
    public float initialMassRangeMin = 1.1f;
    public float initialMassRangeMax = 1.9f;

    void initBuffers()
    {
        Vector3[] initAcc = new Vector3[ParticleCount];
        float[] initMass = new float[ParticleCount];
        Vector3[] initRandom = new Vector3[ParticleCount];
        Vector4[] initCol = new Vector4[ParticleCount];

        //creating these buffers so when emitted, they have a slightly different value
        initAccBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(initAcc.GetType().GetElementType()));
        initMassBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(initMass.GetType().GetElementType()));
        initColBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(initCol.GetType().GetElementType()));
        initRandomBuffer = new ComputeBuffer(ParticleCount, Marshal.SizeOf(initRandom.GetType().GetElementType()));

        for (int i = 0; i < ParticleCount; i++)
        {
            initAcc[i] = new Vector3(Random.Range(initialAccelerationRange * -1, initialAccelerationRange), Random.Range(initialAccelerationRange * -1, initialAccelerationRange), Random.Range(initialAccelerationRange * -1, initialAccelerationRange));
            initMass[i] = Random.Range(initialMassRangeMin, initialMassRangeMax);
            initRandom[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //randombuffer for brownian cs. we can change its range with csBrownianBrownianStrength in realtime
            float mtc = Random.Range(0.5f, 1.0f);
            initCol[i] = new Vector4(mtc, mtc, mtc, 1); //just a random gray value

        }
        initAccBuffer.SetData(initAcc);
        initMassBuffer.SetData(initMass);
        initColBuffer.SetData(initCol);
        initRandomBuffer.SetData(initRandom);
    }
    void Awake()
    {
        initBuffers();

        computeShaderCommandBuffer = new CommandBuffer();
        cameraCommandBuffer = new CommandBuffer();
        lightCommandBuffer = new CommandBuffer();


        outputBuffer = new ComputeBuffer(ParticleManager.ParticleCount, 15 * 4); //the stride of the outputBuffer is the stride of the struct "particle", I just calculated it manually since I know it. see particleStruct.cginc

        particleMaterial.SetPass(0);
        particleMaterial.SetBuffer("buf_Points", outputBuffer);

        foreach (var cs in GameObject.FindObjectsOfType<ParticleBase>())
        {
            //tell renderengine when to dispatch computeshaders aka when to compute particles position

            cs.Init();
            cs.PreDispatch(computeShaderCommandBuffer);
        }
        camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, computeShaderCommandBuffer);
        //tell renderengine when to  draw particles
        cameraCommandBuffer.DrawProcedural(Matrix4x4.identity, particleMaterial, 0, MeshTopology.Triangles, ParticleCount);
        camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, cameraCommandBuffer);
        //tell renderengine when to draw geometry for shadow pass
        lightCommandBuffer = new CommandBuffer();
        lightCommandBuffer.DrawProcedural(Matrix4x4.identity, particleMaterial, 1, MeshTopology.Triangles, ParticleCount);
        light.AddCommandBuffer(LightEvent.BeforeShadowMapPass, lightCommandBuffer);

    }

    private void OnDisable()
    {
        initAccBuffer.Release();
        initMassBuffer.Release();
        initColBuffer.Release();
        initRandomBuffer.Release();
        outputBuffer.Release();
    }
}