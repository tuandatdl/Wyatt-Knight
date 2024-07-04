using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private int damage = 2;
    //Cai thoi gian 5f sau do destroyObject
    [SerializeField] float destroyDelay = 5f;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;

    [Header("Enemy Movenment")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int wayPointDestination;
    [SerializeField] private Transform[] wayPoints;

    [Header("Enemy Chanse")]
    public Transform playerTransform;
    private bool isChasing;
    [SerializeField] private float chanseDistance;




    private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if (playerTransform != null)
        {
            MoveToWaypoint();
        }
    }
    //Nhan damage
    /*public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Hurt();
    }*/
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Hurt();

        
        Vector2 knockbackDirection = (transform.position - playerTransform.position).normalized;
        ApplyKnockback(knockbackDirection);
    }

    public void ApplyKnockback(Vector2 direction)
    {
        
        float knockbackForce = 7f; 
        rb.velocity = direction * knockbackForce;
    }
    public void Hurt()
    {
        anim.SetTrigger("Hurting");
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    //Ham Chet
    public void Die()
    {
        anim.SetBool("IsDead", true);
        playerTransform = null;
        this.enabled = false;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }
    //Ham xoa sau khi enemy chet
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    //Ham tra sat thuong cho player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damage);
            anim.SetTrigger("Attacking");
            playerController.Hurt();
            StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        float countdownDuration = 3.0f;

        yield return new WaitForSeconds(countdownDuration);
    }


    //enemy di chuyen
    private void MoveToWaypoint()
    {
        if (playerTransform == null)
        {
            anim.SetBool("Running", false);
            return;
        }

        if (wayPoints == null || wayPoints.Length < 2)
        {
            Debug.LogError("Not enough waypoints set.");
            return;
        }

        Vector2 wayPointMin = wayPoints[0].position;
        Vector2 wayPointMax = wayPoints[1].position;

        // Ensure wayPointMin is the minimum x value and wayPointMax is the maximum x value
        if (wayPointMin.x > wayPointMax.x)
        {
            Vector2 temp = wayPointMin;
            wayPointMin = wayPointMax;
            wayPointMax = temp;
        }

        bool isPlayerInRange = playerTransform.position.x >= wayPointMin.x && playerTransform.position.x <= wayPointMax.x;

        if (isChasing && isPlayerInRange)
        {
            anim.SetBool("Running", true);

            if (transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            else if (transform.position.x < playerTransform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }

            if (Vector2.Distance(transform.position, playerTransform.position) > chanseDistance || !isPlayerInRange)
            {
                isChasing = false;
            }
        }
        else
        {
            if (isPlayerInRange && Vector2.Distance(transform.position, playerTransform.position) < chanseDistance)
            {
                isChasing = true;
                return;
            }

            anim.SetBool("Running", true);

            if (wayPointDestination == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, wayPoints[0].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, wayPoints[0].position) < 0.2f)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    wayPointDestination = 1;
                }
            }
            else if (wayPointDestination == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, wayPoints[1].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, wayPoints[1].position) < 0.2f)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    wayPointDestination = 0;
                }
            }
        }

        // Stop running animation when idle
        if (!isChasing && Vector2.Distance(transform.position, wayPoints[wayPointDestination].position) < 0.2f)
        {
            anim.SetBool("Running", false);
        }
    }
}

