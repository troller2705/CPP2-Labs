using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthRegen, WinCondition }
    public PowerUpType powerUpType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            ApplyPowerUp(player);
            Destroy(gameObject);
        }
    }

    private void ApplyPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                player.normSpeed *= 1.5f;
                player.sprintSpeed *= 1.5f;
                player.swimSpeed *= 1.5f;
                break;
            case PowerUpType.HealthRegen:
                player.health += 50;
                break;
            case PowerUpType.WinCondition:
                Debug.Log("Player Collected the Winning Item!");
                player.hasWinningItem = true;
                break;
        }
    }
}
