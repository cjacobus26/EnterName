using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb2D;
    public Animator anim;
    public AudioSource jumpClip, dashClip;

    private float moveSpeed = 6;
    private float dashSpeed = 15;
    private float jumpHeight = 1;
    public float horizontalInput;
    public float verticalVelocity;
    private float dashTime = 0.25f;
    private float dashCooldownTime = .5f;

    public int lookDir;

    public bool canMove = false;
    public bool canDash = true;
    public bool Froze = false;
    public bool Dash = false;
    public bool isGrounded;
    public bool Hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;

        Movement();

        if (Froze)
        {
            horizontalInput = 0;
        }
    }

    void Movement()
    {
        verticalVelocity = Mathf.Clamp(rb2D.velocity.y, -1, 1);
        anim.SetFloat("Y Velocity", verticalVelocity);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canDash && canMove && !Froze)
            {
                canMove = false;
                canDash = false;
                Dash = true;
                anim.SetTrigger("Dash");
                dashClip.Play();
                StartCoroutine(DashTime(dashTime));
                StartCoroutine(DashCooldown(dashCooldownTime));
            }
        }

        if (isGrounded && canMove && !Froze)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            rb2D.velocity = new Vector2(horizontalInput * moveSpeed, rb2D.velocity.y);
            if (!canMove)
            {
                horizontalInput = 0;
            }
        }

        if (!canMove && isGrounded && !Hit)
        {
            rb2D.velocity = Vector2.zero;
        }

        if (canMove && !Froze)
        {

            if (horizontalInput < 0)
            {
                lookDir = -1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (horizontalInput > 0)
            {
                lookDir = 1;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (horizontalInput < .25)
            {
                anim.SetBool("Moving", true);
            }
            else if (horizontalInput > .25)
            {
                anim.SetBool("Moving", true);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (isGrounded && canMove && !Froze)
                {
                    rb2D.AddForce(transform.up * jumpHeight * 500);
                    isGrounded = false;
                    anim.SetBool("Grounded", isGrounded);
                    anim.SetTrigger("Jump");
                    jumpClip.Play();
                }
            }
        }

        if (horizontalInput == 0)
        {
            anim.SetBool("Moving", false);
        }

        if (Dash)
        {
            rb2D.position += new Vector2(lookDir * dashSpeed * Time.deltaTime, 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
        }
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
        }
    }

    private IEnumerator DashTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Dash = false;
        canMove = true;
    }
    private IEnumerator DashCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        canDash = true;
    }
}
