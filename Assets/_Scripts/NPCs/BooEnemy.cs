using UnityEngine;

public class BooEnemy : MonoBehaviour
{
    #region Variables
    [Header("Player Detection and Chasing")]
    private Transform player;
    public float detectionRange = 10f; // Distance at which the Boo detects the player
    public float fireRange = 10f; // Distance at which the Boo fires at the player
    public float moveSpeed = 3f;       // Movement speed when chasing
    public float stoppingDistance = 1.5f; // How close the Boo gets to the player

    [Header("Fireball Attack")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 10f;
    public float fireRate = 2f; // Time between fireballs

    [Header("Visibility Settings")]
    public float visibilityAngle = 45f; // Angle within which Boo is visible to the player

    [Header("Enemy Settings")]
    private int health = 2;

    private SkinnedMeshRenderer booRenderer;
    private Animator anim;
    private float nextFireTime;

    private bool isChasing = false;
    #endregion
    #region Basic Calls(Start/Update/Awake)
    void Start()
    {
        booRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Check if the player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true; // Start chasing the player
        }
        else
        {
            isChasing = false; // Stop chasing when out of range
        }

        HandleVisibility();
        HandleChasing();
        HandleFireballAttack();
    }
    #endregion
    #region Handlers
    private void HandleVisibility()
    {
        // Vector from Boo to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Boo's forward direction
        Vector3 booForward = transform.forward;

        // Dot product to check if Boo is facing the player
        float dotProduct = Vector3.Dot(booForward, directionToPlayer);

        // Convert dot product into an angle
        float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        // Boo is visible if the angle is within the visibility threshold
        if (angle <= visibilityAngle)
        {
            booRenderer.enabled = true; // Make Boo visible
        }
        else
        {
            booRenderer.enabled = false; // Make Boo invisible
        }
    }

    private void HandleChasing()
    {
        if (isChasing)
        {
            // Look at the player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Prevent tilting the Boo upwards or downwards
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Move towards the player until within stopping distance
            if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;

                anim.SetFloat("SpeedFB", 1);
            }
        }
        else
        {
            anim.SetFloat("SpeedFB", 0);
        }
    }

    private void HandleFireballAttack()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Only attack if the player is within detection range
        if (isChasing && (Time.time >= nextFireTime) && distanceToPlayer <= fireRange)
        {
            nextFireTime = Time.time + fireRate;
            anim.SetTrigger("SpellCast");
            for (int i = 0; i < 2; i++)
            { }
            ShootFireball();
        }
    }

    public void HandleDamage(int damage)
    {
        Debug.Log($"Boo Hit");
        if (health > 0)
        {
            health -= damage;
            anim.SetTrigger("Hit");
        }
        else
        {
            Die();
        }
    }
    #endregion

    private void ShootFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        fireball.GetComponent<Fireball>().casterTag = gameObject.tag;
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        Vector3 direction = (player.position - fireballSpawnPoint.position).normalized;
        rb.velocity = direction * fireballSpeed;
    }

    public void Die()
    {
        anim.SetBool("Death", true);
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
