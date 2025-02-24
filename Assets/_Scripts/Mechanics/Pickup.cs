using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int coinValue = 1; // How much the coin is worth
    public AudioClip pickupSound; // Optional: Assign a sound effect

    public void PickupCoin()
    {
            // Increase player's score
            PlayerScore.instance.AddCoins(coinValue);

            // Play sound if assigned
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Destroy the coin
            Destroy(gameObject);
    }

}
