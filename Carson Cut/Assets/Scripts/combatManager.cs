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

    private void FixedUpdate()
    {
        healthBar.SetHealth(Health);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && pView.IsMine)
        {
            if (playerMovement.canMove && !playerMovement.Froze)
            {
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

    public void TakeDamage(GameObject Target, int damage, int lookDir)
    {
        Health -= damage;
        playerMovement.Hit = true;
        playerMovement.Dash = false;
        playerMovement.canMove = false;
        playerMovement.canDash = false;
        playerMovement.rb2D.AddForce(new Vector2(Knockback * lookDir, 0));

        if (Health > 0)
        {
            //If Hit
            playerScript.anim.SetTrigger("Hit");
            playerScript.hitClip.Play();
            StartCoroutine(StaggerTime(0.5f));
        }
        else
        {
            //If dead on Hit
            playerScript.anim.SetTrigger("Death");
            playerScript.deathClip.Play();
        }
    }
}
