using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //PlayerMovenment
    [Header("Player Movenment")]
    [SerializeField] private float speed = 5f;
    private float xAxis;

    //Player Jumping
    [Header("Player Jumping")]
    [SerializeField] private float jumpForce = 14f;

    //PlayerAttack
    [Header("Player Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float nextAttackTime = 0f;

    //HP Player
    [Header("Player Damage")]
    [SerializeField] private int attackDamage = 2;
    

    //Check IsGrounded
    [SerializeField] private LayerMask groundLayer;

    //Khoi tao bien logic
    private PlayerHealth playerHealth;
    private EnemyController enemyController;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyController = GetComponent<EnemyController>();  
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        InputMove();
        Movement();
        Flip();
        Jump();
        InputAttack();
        Die();
        
    }
    //Quay nhan vat theo huong di chuyen
    private void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }
    //Ham di chuyen nhan vat
    private void Movement()
    {
        rb.velocity = new Vector2(xAxis * speed, rb.velocity.y);
        anim.SetBool("Running", rb.velocity.x != 0);
    }
    //Ham nhan gia tri tu ban phim
    private void InputMove()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    private void InputAttack()
    {
        if (Time.time > nextAttackTime)
        {

            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }
    //Ham nhay
    private void Jump()
    {
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        anim.SetBool("Jumping", !IsGrounded());
        if (!IsGrounded() && rb.velocity.y < 0)
        {
            anim.SetBool("Falling", true);
        }
        else
        {
            anim.SetBool("Falling", false);
        }
    }
    //Ham check IsGround
    private bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 1.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if(hit.collider != null)
        {
            return true;
        }
        return false;
    }
    //Ham Attack
    private void Attack()
    {
        //set Aninmation
        anim.SetTrigger("Attacking");
        //Attack
        Collider2D[] hitEnemics = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //Cong Damge
        foreach(Collider2D enemy in hitEnemics)
        {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
        }
    }
    //Ham ve ra vung tan cong
    private void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    //Ham nhan sat thuong tu quai
    public void Hurt()
    {
        anim.SetTrigger("Hurting");
        
    }
    private void Die()
    {
        if (playerHealth.health <= 0)
        {
            anim.SetBool("IsDead", true);
            this.enabled = false;
            rb.simulated = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }

}
