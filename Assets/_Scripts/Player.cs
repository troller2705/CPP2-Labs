
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(xInput * speed, rb.velocity.y, zInput * speed);

        if (Input.GetButton("Jump"))
        {
            
        }
    }
}
