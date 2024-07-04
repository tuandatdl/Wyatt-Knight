using System.Collections;
using TreeEditor;
using UnityEngine;

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
    private float countdownTimer = 1f;

    private void Start()
    {
        rb= GetComponent<Rigidbody2D>();    
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }
    private void Update()
    {
        if(playerTransform != null)
        {
            MoveToWaypoint();
        }
    }
    //Nhan damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Hurt();
    }
    private void Hurt()
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
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

        if (isChasing)
        {
            if(transform.position.x > playerTransform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }
            if (transform.position.x < playerTransform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            if(Vector2.Distance(transform.position, playerTransform.position) < chanseDistance)
            {
                isChasing = true;
            }
            anim.SetBool("Running", true);
            if (wayPointDestination == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, wayPoints[0].position, moveSpeed* Time.deltaTime);
                if (Vector2.Distance(transform.position, wayPoints[0].position) < 0.2f)
                {
                    transform.localScale=new Vector3(1, 1, 1);
                    wayPointDestination = 1;
                }
            }
            if (wayPointDestination == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, wayPoints[1].position, moveSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, wayPoints[1].position) < 0.2f)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    wayPointDestination = 0;
                }
            }
        }
    }


}
