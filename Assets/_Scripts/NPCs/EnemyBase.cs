using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public float health = 100f;
    public float detectionRange = 10f;
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public abstract void Act(); // Each enemy type implements this differently

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
