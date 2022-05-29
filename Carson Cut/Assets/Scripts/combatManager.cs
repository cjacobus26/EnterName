using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviourPunCallbacks
{
    HealthBar healthBar;
    PlayerMovement playerMovement;
    PlayerScript playerScript;

    public int maxHealth = 100;
    public int Health;
    public int Knockback = 10;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
        playerScript = transform.parent.gameObject.GetComponent<PlayerScript>();

        //Get HealthBar from correct place
        if (this.transform.parent.name == "Player1")
        {
            healthBar = GameObject.Find("Canvas/Health Bar 1").GetComponent<HealthBar>();
        }
        else
        { 
            healthBar = GameObject.Find("Canvas/Health Bar 2").GetComponent<HealthBar>();
        }

        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(Health);

        if (Input.GetMouseButtonDown(0) && photonView.IsMine)
        {
            if (playerMovement.canMove && !playerMovement.Froze)
            {
                //Attack
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                photonView.RPC("RPC_Attack", RpcTarget.All);
                StartCoroutine(AttackCooldown(.5f));
            }
        }
    }

    public void TakeDamage(int damage, int lookDir)
    {
        Health -= damage;
        playerMovement.Hit = true;
        playerMovement.Dash = false;
        playerMovement.canMove = false;
        playerMovement.canDash = false;
        playerMovement.rb2D.AddForce(new Vector2(Knockback * lookDir, 0));

        if (Health > 0)
        {
            //If not Dead
            playerMovement.anim.SetTrigger("Hit");
            playerScript.hitClip.Play();
            StartCoroutine(StaggerTime(0.5f));
        }
        else
        {
            //If Dead
            playerMovement.anim.SetTrigger("Death");
            playerScript.deathClip.Play();
        }
    }

    IEnumerator StaggerTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerMovement.canMove = true;
        playerMovement.canDash = true;
        playerMovement.Hit = false;
    }

    IEnumerator AttackCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerMovement.canMove = true;
        playerMovement.canDash = true;
    }
}
