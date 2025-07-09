using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int health;
    public bool hasWinningItem;
    public List<EnemyData> enemies;
}

[Serializable]
public class EnemyData
{
    public string enemyType;
    public Vector3 position;
    public bool isAlive;
}
