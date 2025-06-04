using UnityEngine;
using UnityEngine.UI;

public class PatrolSkeleton : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform checkPoint;
    public float distance = 1f;
    public LayerMask layerMask;
    public bool menghadapKiri = true;
    private bool inRange = false;
    public Transform player;
    public float attackRange = 10f;
    public float retrieveRange = 2.5f;  
    public float chaseSpeed = 4f;
    public Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    public int maxHealth = 10;
    public bool isDie = false;
    public AudioSource attackAudio;
    public int attackDamage = 1;
    public Text scoreText;
    private Rigidbody2D rb;
    public int gravityScale = 3;
    public bool randomGravityScale = true;
    public float radiusJatuh = 10f;
    public bool isBoss = false;
    private bool isBossAudioActive = false;
    public AudioSource bossAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindFirstObjectByType<GameManager>().isGameActive == false) {
            animator.SetBool("Attack 1", false);
            return;
        }

        if (maxHealth <= 0 && !isDie) {
            Die();
        }

        if (Mathf.Abs(transform.position.x - player.position.x) <= radiusJatuh) {
            if (randomGravityScale) {
                rb.gravityScale = Random.Range(0.7f, 1.2f);

            } else {
                rb.gravityScale = gravityScale;
            }

            if (isBoss && !isBossAudioActive) {
                bossAudio.Play();
                isBossAudioActive = true;
            }
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange) {
            inRange = true;
           
        } else {
            inRange = false;
        }

        if (inRange) {
            if (player.position.x > transform.position.x && menghadapKiri) {
                transform.eulerAngles = new Vector3(0, -180, 0);
                menghadapKiri = false;
            } else if (player.position.x < transform.position.x && !menghadapKiri) {
                transform.eulerAngles = new Vector3(0, 0, 0);
                menghadapKiri = true;
            }

            if (Vector2.Distance(transform.position, player.position) > retrieveRange) {
                animator.SetBool("Attack 1", false);
                transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            } else {
                animator.SetBool("Attack 1", true);
            }
        } else {
            transform.Translate(Vector2.left * Time.deltaTime * moveSpeed);

            RaycastHit2D hit = Physics2D.Raycast(checkPoint.position, Vector2.down, distance, layerMask);

            if (hit == false && menghadapKiri)  {
                transform.eulerAngles = new Vector3(0, -180, 0);
                menghadapKiri = false;
            } else if(hit == false && !menghadapKiri) {
                transform.eulerAngles = new Vector3(0, 0, 0);
                menghadapKiri = true;
            }
        }
    }

    public void Attack () {
        Collider2D colliderInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);

        if (colliderInfo) {
            if (colliderInfo.gameObject.GetComponent<Player> () != null) {
                attackAudio.Play();
                colliderInfo.gameObject.GetComponent<Player> ().takeDamage(attackDamage);
            }
        }
    }

    void Die () {
        isDie = true;
        animator.SetBool("Attack 1", false);
        Destroy(gameObject, 0.5f);
    }

    public void takeDamage (int damage) {
        FindFirstObjectByType<GameData>().score += 10;
        scoreText.text = FindFirstObjectByType<GameData>().score.ToString();

        if (maxHealth <= 0) {
            return;
        }

        maxHealth -= damage;
        animator.SetTrigger("Hurt");
    }

    private void OnDrawGizmosSelected()
    {
        if (checkPoint == null) {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(checkPoint.position, Vector2.down * distance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position,attackRadius);
    }
}
