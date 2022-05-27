using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject swordHitbox;
    public Animator anim;
    public Rigidbody2D rb2D;
    public AudioSource hitClip, deathClip, swingClip;
    
    MainManager mainManager;
    PlayerMovement playerMovement;
    combatManager CombatManager;
    PhotonView pView;

    public int playerLives = 3;

    private int networkHealth;
    private Vector2 networkVelocity;
    private Quaternion networkRot;
    private Vector2 networkPos;
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector2 positionAtLastPacket = Vector2.zero;
    Vector2 velocityAtLastPacket = Vector2.zero;
    private bool calcLag;

    // Start is called before the first frame update
    void Start()
    {
        CombatManager = GetComponentInChildren<combatManager>();
        playerMovement = this.transform.gameObject.GetComponent<PlayerMovement>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        pView = this.GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            pView.RPC("RPC_ChangeName", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private void Awake()
    {
        
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine && calcLag)
        {
            //Lag compensation
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            //Update remote player
            rb2D.position = Vector2.Lerp(positionAtLastPacket, networkPos, (float)(currentTime / timeToReachGoal));
            rb2D.velocity = Vector2.Lerp(velocityAtLastPacket, networkVelocity, (float)(currentTime / timeToReachGoal));
            transform.rotation = networkRot;
            CombatManager.Health = networkHealth;

            if (Vector3.Distance(rb2D.position, networkPos) > 5)
            {
                rb2D.position = networkPos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (CombatManager.Health < 0)
            {
                //If player is dead
                mainManager.GameRunning = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    void Attack()
    {
        swingClip.Play();
        swordHitbox.SetActive(true);
        new WaitForSeconds(0.1f);
        swordHitbox.SetActive(false);
    }

    void RemoveBanner(int lives)
    {
        if (lives == 2)
        {
            GameObject.Find("Health Bar 1/Knight Life Banner 3/Head").SetActive(false);
        }

        if (lives == 1)
        {
            GameObject.Find("Health Bar 1/Knight Life Banner 2/Head").SetActive(false);
        }

        if (lives == 0)
        {
            GameObject.Find("Health Bar 1/Knight Life Banner 1/Head").SetActive(false);
            mainManager.Player2Win();
        }
    }

    void RemoveBanner2(int lives)
    {
        if (lives == 2)
        {
            GameObject.Find("Health Bar 2/Knight Life Banner 3/Head").SetActive(false);
        }

        if (lives == 1)
        {
            GameObject.Find("Health Bar 2/Knight Life Banner 2/Head").SetActive(false);
        }

        if (lives == 0)
        {
            GameObject.Find("Health Bar 2/Knight Life Banner 1/Head").SetActive(false);
            mainManager.Player1Win();
        }
    }

    [PunRPC]
    private void RPC_ChangeName(int playerID)
    {
        this.gameObject.name = "Player" + playerID;
    }

    [PunRPC]
    private void RPC_Attack()
    {
        anim.SetTrigger("Attack");
    }

    [PunRPC]
    private void RPC_Hit()
    {
        anim.SetTrigger("Hit");
        hitClip.Play();
    }

    [PunRPC]
    private void RPC_Death()
    {
        anim.SetTrigger("Death");
        deathClip.Play();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(rb2D.position);
            stream.SendNext(rb2D.velocity);
            stream.SendNext(transform.rotation);
            stream.SendNext(CombatManager.Health);
        }
        else
        {
            // Network player, receive data
            networkPos = (Vector2)stream.ReceiveNext();
            networkVelocity = (Vector2)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            networkHealth = (int)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            if (rb2D != null)
            {
                positionAtLastPacket = rb2D.position;
                velocityAtLastPacket = rb2D.velocity;

                calcLag = true;
            }
        }
    }
}
