using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveMe : MonoBehaviour
{
    public float speed = 1.2f;
    private Vector3 startPosition;
    void Start()
    {
        startPosition = gameObject.transform.localPosition;   
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        gameObject.transform.Translate(dir.normalized * Time.deltaTime * speed);

        if (Input.GetKeyDown(KeyCode.Space))
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);

        if (Input.GetKeyDown(KeyCode.R))
            gameObject.transform.localPosition = startPosition;
    }
}
