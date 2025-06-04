using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator animator;
    private float movement;
    public float moveSpeed = 5f;
    private bool facingRight = true;
    public float jumpHeight = 5f;
    public bool isGround = false;
    public int maxHealth = 3;
    public bool isDie = false;
    public Text health;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    public AudioSource dieAudio1;
    public AudioSource dieAudio2;
    public AudioSource swordAudio;
    public AudioSource attackAudio;
    public int damage = 1;
    public AudioSource runAudio;
    public AudioSource spawnAudio;
    public int level = 1;
    public Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()    
    {
        health.text = maxHealth.ToString();
        spawnAudio.Play();
    }

    void Update()
    {
        if (!isDie) {
            if (maxHealth <= 0) {
                Die();
            }

            movement =  Input.GetAxis("Horizontal");

            if (movement < 0f && facingRight) {
                transform.eulerAngles = new Vector3(0f, -180f, 0f);
                facingRight = false;
            } 

            if (movement > 0f && !facingRight) {
                transform.eulerAngles = new Vector3(0f, 0f, 0f);
                facingRight = true;
            }

            if (Input.GetKey(KeyCode.Space) && isGround) {
                Jump();
                isGround = false;
                animator.SetBool("jump", true);
            }

            if (rb.linearVelocity.y < 0 && !isGround) {
                animator.SetBool("Grounded", false);
                animator.SetFloat("AirSpeedY", -1);
                animator.SetBool("jump", false);
            }

            if (Math.Abs(movement) > .1f) {
                animator.SetInteger("run", 1);
                animator.SetBool("Grounded", isGround);
            } else if (movement < .1f) {
                animator.SetInteger("run", 0);
            }

            if (Input.GetMouseButtonDown(0)) {
                swordAudio.Play();
                animator.SetTrigger("Attack1");
                // Attack();
            }
        }
    }

    void FixedUpdate()
    {
        transform.position += moveSpeed * Time.fixedDeltaTime * new Vector3(movement, 0f, 0f);
    }

    void Jump () {
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") {
            isGround = true;
            animator.SetBool("Grounded", isGround);
            animator.SetFloat("AirSpeedY", 0);
            animator.SetBool("jump", false);
        }
    }

    public void takeDamage (int damage) {
        swordAudio.Stop();
        FindFirstObjectByType<GameData>().score -= 5;
        scoreText.text = FindFirstObjectByType<GameData>().score.ToString();

        if (maxHealth <= 0) {
            return;
        }

        maxHealth -= damage;
        health.text = maxHealth.ToString();
        animator.SetTrigger("Hurt");
    }

    void Die() {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine() {
        isDie = true;
        animator.SetTrigger("Death");
        dieAudio1.Play();
        dieAudio2.Play();
        FindFirstObjectByType<GameManager>().isGameActive = false;

        // Tunggu sampai kedua audio selesai diputar
        float maxDuration = Mathf.Max(dieAudio1.clip.length, dieAudio2.clip.length);
        yield return new WaitForSeconds(maxDuration);

        SceneManager.LoadScene("Game Over");
    }

    public void Attack () {
        Collider2D colliderInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        attackAudio.Play();

        if (colliderInfo) {
            if (colliderInfo.gameObject.GetComponent<PatrolSkeleton> () != null) {
                colliderInfo.gameObject.GetComponent<PatrolSkeleton> ().takeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position,attackRadius);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trap") {
            SceneManager.LoadSceneAsync("Game Over");
        }

        if (collision.gameObject.tag == "VictoryPoint" && level == 1) {
            SceneManager.LoadSceneAsync("Level 1 Win");
        }

        if (collision.gameObject.tag == "VictoryPoint" && level == 2) {
            SceneManager.LoadSceneAsync("Level 2 Win");
        }

        if (collision.gameObject.tag == "VictoryPoint" && level == 3) {
            SceneManager.LoadSceneAsync("Level 3 Win");
        }
    }
}
