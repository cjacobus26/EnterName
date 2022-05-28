using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviourPunCallbacks
{
    HealthBar healthBar;
    PlayerMovement playerMovement;
    PhotonView pView;
    PlayerScript playerScript;

    public int maxHealth = 100;
    public int Health;
    public int Knockback = 10;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
        playerScript = transform.parent.gameObject.GetComponent<PlayerScript>();
        pView = PhotonView.Get(this);

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

        if (Input.GetMouseButtonDown(0) && pView.IsMine)
        {
            if (playerMovement.canMove && !playerMovement.Froze)
            {
                //Attack
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                StartCoroutine(AttackCooldown(.5f));
                playerScript.anim.SetTrigger("Attack");
                pView.RPC("RPC_Attack", RpcTarget.Others);
            }
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
            playerScript.anim.SetTrigger("Hit");
            playerScript.hitClip.Play();
            StartCoroutine(StaggerTime(0.5f));
        }
        else
        {
            //If Dead
            playerScript.anim.SetTrigger("Death");
            playerScript.deathClip.Play();
        }
    }
}
