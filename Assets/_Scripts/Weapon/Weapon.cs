
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Weapon : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;

    public int damage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public void Equip(Collider playerCollider, Transform weaponAttachPoint)
    {
        rb.isKinematic = true;
        bc.isTrigger = true;
        transform.SetParent(weaponAttachPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Physics.IgnoreCollision(playerCollider, bc);
    }

    public void Drop(Collider playerCollider, Vector3 playerForward)
    {
        transform.parent = null;
        rb.isKinematic = false;
        rb.useGravity = true;
        bc.isTrigger = false;
        rb.AddForce(playerForward * 10.0f, ForceMode.Impulse);
        StartCoroutine(DropCooldown(playerCollider));
    }

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(3);

        //happens after cooldown
        Physics.IgnoreCollision(bc, playerCollider, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Hit Enemy: {collision.gameObject.name}");
            if (collision.gameObject.GetComponent<Bear>())
            {
                collision.gameObject.GetComponent<Bear>().HandleDamage(damage);
            }
            else if (collision.gameObject.GetComponent<BooEnemy>())
            {
                collision.gameObject.GetComponent<BooEnemy>().HandleDamage(damage);
            }
        }
    }
}
