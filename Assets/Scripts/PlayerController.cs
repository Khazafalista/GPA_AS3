using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxLength = 1.0f;

    public AudioClip throwSE;

    //self components
    private Rigidbody2D rb;
    private SpringJoint2D spring;
    private CircleCollider2D circle;

    //local variables
    private bool clicking;
    private bool released;
    private Vector2 prevVelocity;

    //references
    private Rigidbody2D hook;
    private LineRenderer beltFront;
    private LineRenderer beltBack;
    private GameController gameController;
    private AudioSource SEPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spring = GetComponent<SpringJoint2D>();
        circle = GetComponent<CircleCollider2D>();

        beltFront = GameObject.Find("BeltFront").GetComponent<LineRenderer>();
        beltBack = GameObject.Find("BeltBack").GetComponent<LineRenderer>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void Start()
    {
        clicking = false;
        released = false;
        prevVelocity = Vector2.zero;

        hook = spring.connectedBody.gameObject.GetComponent<Rigidbody2D>();

        beltFront.enabled = true;
        beltBack.enabled = true;
        beltFront.SetPosition(0, Vector3.zero);
        beltBack.SetPosition(0, Vector3.zero);

        SEPlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(gameController.state == (int)GameController.PlayerState.PLAYING)
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                gameController.state = (int)GameController.PlayerState.PAUSE;
            }

            if (clicking)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dragVec = mousePos - hook.position;
                if (dragVec.magnitude > maxLength)
                {
                    rb.position = hook.position + dragVec.normalized * maxLength;
                }
                else
                {
                    rb.position = mousePos;
                }
            }

            if (spring != null)
            {
                //Throw out the ball!
                if (released && prevVelocity.magnitude > rb.velocity.magnitude)
                {
                    SEPlayer.PlayOneShot(throwSE);

                    Destroy(spring);
                    rb.velocity = prevVelocity;

                    setBallSpawnFalse();
                    gameController.ballThrowCount++;
                }

                if (released)
                {
                    prevVelocity = rb.velocity;
                }

                Vector2 dragDis = rb.position - (Vector2)beltFront.transform.position;
                Vector2 dragPos = dragDis + dragDis.normalized * circle.radius;
                beltFront.SetPosition(1, dragPos);
                dragDis = rb.position - (Vector2)beltBack.transform.position;
                dragPos = dragDis + dragDis.normalized * circle.radius;
                beltBack.SetPosition(1, dragPos);
            }
            else
            {
                beltFront.enabled = false;
                beltBack.enabled = false;
            }
        }
    }

    private void setBallSpawnFalse()
    {
        Invoke("setBallSpawnFalseInvoke", gameController.ballDestroyDelay);
    }

    private void setBallSpawnFalseInvoke()
    {
        gameController.ballSpawn = false;

        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (gameController.state == (int)GameController.PlayerState.PLAYING)
        {
            clicking = true;
            rb.isKinematic = true;
        }
    }

    private void OnMouseUp()
    {
        if (gameController.state == (int)GameController.PlayerState.PLAYING)
        {
            clicking = false;
            released = true;
            rb.isKinematic = false;
        }
    }
}
