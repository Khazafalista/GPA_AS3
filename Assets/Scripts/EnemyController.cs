using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 4.0f;
    public static int enemiesAlive = 0;
    public AudioClip hitSE;

    private GameController gameController;
    private Animator animator;
    private AudioSource SEPlayer;

    private void Awake()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        enemiesAlive++;
        
        animator = GetComponent<Animator>();
        SEPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(gameObject.transform.position.y < -10.0f && !animator.GetBool("Dead"))
        {
            EnemyDie();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > health && !animator.GetBool("Dead"))
        {
            SEPlayer.volume = 0.5f;
            SEPlayer.PlayOneShot(hitSE);
            EnemyDie();
        }
    }

    private void EnemyDie()
    {
        enemiesAlive--;
        animator.SetBool("Dead", true);
        gameController.score += 100 / gameController.maxEnemy;

        if (enemiesAlive <= 0)
        {
            Debug.Log("Game Over");
            gameController.state = (int)GameController.PlayerState.CLEAR;
        }
    }
}
