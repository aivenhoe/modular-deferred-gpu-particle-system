using UnityEngine;


public class rotate : MonoBehaviour
{
    public float rotateX = 0.4f;
    public float rotateY = 0.1f;

    void Update()
    {
        transform.Rotate(rotateX, rotateY, 0); 
    }
}
