using UnityEngine;

public class TurretEnemy : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float fireCooldown = 0f;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange && fireCooldown <= 0f)
        {
            Act();
            fireCooldown = fireRate;
        }
        fireCooldown -= Time.deltaTime;
    }

    public override void Act()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Debug.Log("Turret Enemy Shooting!");
    }
}
