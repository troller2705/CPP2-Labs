using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifespan = 5f;

    void Start()
    {
        Destroy(gameObject, lifespan); // Destroy fireball after lifespan expires
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.RestartGame();
            }
        }
    }
}
