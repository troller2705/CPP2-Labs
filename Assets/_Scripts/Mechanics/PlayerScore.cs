using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public static PlayerScore instance;
    public int totalCoins = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log("Coins: " + totalCoins);
    }
}
