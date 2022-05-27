using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatManager : MonoBehaviourPunCallbacks
{

    public Animator anim;
    HealthBar healthBar;
    PlayerMovement playerMovement;
    PhotonView pView;
    PlayerScript playerScript;

    public int maxHealth = 100;
    public int Health;

    public Vector2 Knockback = new Vector2(150, 0);
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

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(Health);

        if (Input.GetMouseButtonDown(0) && pView.IsMine)
        {
            if (playerMovement.canMove && !playerMovement.Froze)
            {
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                StartCoroutine(AttackCooldown(.5f));
                pView.RPC("RPC_Attack", RpcTarget.All);
            }
        }
    }

    IEnumerator StaggerTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerMovement.canMove = true;
        playerMovement.canDash = true;
    }

    IEnumerator AttackCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerMovement.canMove = true;
        playerMovement.canDash = true;
    }

    public void TakeDamage(int damage, int lookDir)
    {
        Debug.Log(transform.parent.gameObject.name + " Took damage");
        Health -= damage;
        gameObject.transform.parent.GetComponent<Rigidbody2D>().AddForce(Knockback * lookDir);
        playerMovement.Dash = false;

        if (Health > 0)
        {
            pView.RPC("RPC_Hit", RpcTarget.Others);
            playerMovement.canMove = false;
            playerMovement.canDash = false;
            StartCoroutine(StaggerTime(0.1f));
        }
        else
        {
            //If dead
            pView.RPC("RPC_Death", RpcTarget.Others);
            playerMovement.canMove = false;
            playerMovement.canDash = false;
        }
    }
}
