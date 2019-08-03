using UnityEngine;
using UnityEngine.Rendering;
public class csParticleAttractor : ParticleBase
{
    public GameObject DummyAttractor;
    public float AttractorForce = 0.5f;
    private float mouseRange = 20f;

    public override void Init()
    {
        base.Init();
    }
    public override void PreDispatch(CommandBuffer commanBuffer)
    {
        base.PreDispatch(commanBuffer);
        //pass data to attractor cs
        commanBuffer.SetComputeBufferParam(computeShader, kernel, "output", ParticleManager.outputBuffer);
        commanBuffer.DispatchCompute(computeShader, kernel, ParticleManager.ThreadDispatchX, 1, 1);

    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    void Update()
    {

        //just to have a moveable attractor. very imprecise, but whatever.
        float mx = map(getMouseCoordinates().x, 0, Screen.width, mouseRange * -1, mouseRange);
        float my = map(getMouseCoordinates().y, 0, Screen.height, mouseRange * -1, mouseRange);
        DummyAttractor.transform.localPosition = new Vector3(mx, DummyAttractor.transform.localScale.y/2f, my);

        float f = 0;
        if (Input.GetMouseButton(0))
            f = AttractorForce * -1;
        if (Input.GetMouseButton(1))
            f = AttractorForce;

        computeShader.SetFloat("_Force", f);
        computeShader.SetFloat("_Size", DummyAttractor.transform.localScale.x);
        computeShader.SetVector("_AttractorPos", DummyAttractor.transform.localPosition);
        computeShader.SetInt("_ParticleCount", ParticleManager.ParticleCount);
        computeShader.SetInt("_RandomIndex", Random.Range(0, 12000000));
    }
    private Vector2 getMouseCoordinates()
    {
        return new Vector2(Mathf.Max(Mathf.Min(Input.mousePosition.x, Screen.width), 0), Mathf.Max(Mathf.Min(Input.mousePosition.y, Screen.height), 0));
    }
    private float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return (NewValue);
    }
}
