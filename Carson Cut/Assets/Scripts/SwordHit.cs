using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : MonoBehaviourPunCallbacks
{
    PlayerMovement playerMovement;

    public bool check;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != transform.parent.gameObject.name && collision.gameObject.name != "CombatManager")
        {
            if (collision.GetComponentInChildren<combatManager>().Health > 0)
            {
                //If Trigger Hits Other Players Collider TakeDamage
                collision.GetComponentInChildren<combatManager>().TakeDamage(10, playerMovement.lookDir);
            }
        }
    }
}
