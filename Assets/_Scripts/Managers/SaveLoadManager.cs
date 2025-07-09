using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private static string saveFile => Path.Combine(Application.persistentDataPath, "gameSave.json");

    public static void SaveGame(PlayerController player, List<EnemyBase> enemies)
    {
        GameData data = new GameData
        {
            playerPosition = player.transform.position,
            playerRotation = player.transform.rotation,
            health = player.health,
            hasWinningItem = player.hasWinningItem,
            enemies = new List<EnemyData>()
        };

        foreach (EnemyBase enemy in enemies)
        {
            data.enemies.Add(new EnemyData
            {
                enemyType = enemy.GetType().ToString(),
                position = enemy.transform.position,
                isAlive = enemy.health > 0
            });
        }

        File.WriteAllText(saveFile, JsonUtility.ToJson(data, true));
        Debug.Log("Game Saved!");
    }

    public static GameData LoadGame()
    {
        if (!File.Exists(saveFile))
            return null;

        return JsonUtility.FromJson<GameData>(File.ReadAllText(saveFile));
    }
}
