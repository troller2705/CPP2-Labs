using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifespan = 5f;
    public string casterTag;

    void Start()
    {
        Destroy(gameObject, lifespan); // Destroy fireball after lifespan expires
        PlayerController.OnControllerColliderHitInternal += OnPlayerControllerHit;
    }

    void OnPlayerControllerHit(Collider playerCollider, ControllerColliderHit hit)
    {
        if (playerCollider.CompareTag("Player") && hit.collider.name == gameObject.name && casterTag != playerCollider.tag)
        {
            PlayerController playerController = playerCollider.GetComponent<PlayerController>();
            if (playerController != null)
            {
                //playerController.RestartGame();
            }
        }
    }

    private void OnDestroy()
    {
        PlayerController.OnControllerColliderHitInternal -= OnPlayerControllerHit;
    }
}
