using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Weapon : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }
    
    public void Equip(Collider player, Transform attachPoint)
    {
        rb.isKinematic = true;
        bc.isTrigger = true;
        transform.SetParent(attachPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Drop(Collider player, Transform playerForward)
    {
        transform.parent = null;
        rb.isKinematic = true;
        bc.isTrigger = true;
        //rb.AddForce(playerForward. * 10.0f, ForceMode.Impulse);
    }
}
