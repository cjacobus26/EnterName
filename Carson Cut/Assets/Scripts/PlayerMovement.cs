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

    public int lookDir;

    public float horizontalInput;
    public float verticalVelocity;
    private float moveSpeed = 6;
    private float dashSpeed = 15;
    private float jumpHeight = 1;
    private float dashTime = 0.25f;
    private float dashCooldownTime = .5f;

    public bool canMove = false;
    public bool canDash = true;
    public bool Froze = false;
    public bool Dash = false;
    public bool isGrounded;
    public bool Hit;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Movement();
        }
    }

    void Movement()
    {
        //Set Y Velocity to Animator float
        verticalVelocity = Mathf.Clamp(rb2D.velocity.y, -1, 1);
        anim.SetFloat("Y Velocity", verticalVelocity);

        if (Froze)
        {
            //Freeze input
            horizontalInput = 0;
        }

        if (isGrounded && canMove && !Froze)
        {
            //Move Character
            horizontalInput = Input.GetAxis("Horizontal");
            rb2D.velocity = new Vector2(horizontalInput * moveSpeed, rb2D.velocity.y);
            if (!canMove)
            {
                horizontalInput = 0;
            }
        }

        if (!canMove && isGrounded && !Hit)
        {
            //Stop moving when not hit and isGrounded
            rb2D.velocity = Vector2.zero;
        }

        if (canMove && !Froze)
        {
            if (horizontalInput < 0)
            {
                //Flip Character Left
                lookDir = -1;
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (horizontalInput > 0)
            {
                //Flip Character Right
                lookDir = 1;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }

            if (horizontalInput > 0.25)
            {
                //Start Walk Animation when going right
                anim.SetBool("Moving", true);
            }
            else if (horizontalInput < -0.25)
            {
                //Start Walk Animation when going left
                anim.SetBool("Moving", true);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (isGrounded && canMove && !Froze)
                {
                    //Jump
                    rb2D.AddForce(transform.up * jumpHeight * 500);
                    isGrounded = false;
                    anim.SetBool("Grounded", isGrounded);
                    anim.SetTrigger("Jump");
                    jumpClip.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (canDash && canMove && !Froze)
                {
                    //Dash
                    canMove = false;
                    canDash = false;
                    Dash = true;
                    anim.SetTrigger("Dash");
                    dashClip.Play();
                    StartCoroutine(DashTime(dashTime));
                    StartCoroutine(DashCooldown(dashCooldownTime));
                }
            }
        }

        if (horizontalInput == 0)
        {
            //Stop walk animation
            anim.SetBool("Moving", false);
        }

        if (Dash)
        {
            //Apply the dash force while Dash
            rb2D.position += new Vector2(lookDir * dashSpeed * Time.deltaTime, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //Player Grounded
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
        }
        if (collision.gameObject.name == "Player1" || collision.gameObject.name == "Player2")
        {
            //Ignore Players colliding
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
