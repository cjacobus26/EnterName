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
    
    private MainManager mainManager;
    private PlayerMovement playerMovement;
    private combatManager CombatManager;
    private PhotonView pView;

    public int playerLives = 3;
    private int networkHealth;
    private int networkDir;

    private float currentTime = 0;

    private double currentPacketTime = 0;
    private double lastPacketTime = 0;

    public bool calcLag;

    private Quaternion networkRot;

    private Vector2 positionAtLastPacket = Vector2.zero;
    private Vector2 velocityAtLastPacket = Vector2.zero;
    private Vector2 networkPos;
    private Vector2 networkVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        CombatManager = GetComponentInChildren<combatManager>();
        playerMovement = this.transform.gameObject.GetComponent<PlayerMovement>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();
        pView = this.GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            //Sync GameObject Name With All Server Clients
            pView.RPC("RPC_ChangeName", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
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
                //Leave Room when Esc is Pressed
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LeaveRoom();
            }
        }

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
            playerMovement.lookDir = networkDir;

            if (Vector3.Distance(rb2D.position, networkPos) > 3)
            {
                //Update Pos of actual distance if too far from servers Pos
                rb2D.position = networkPos;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(rb2D.position);
            stream.SendNext(rb2D.velocity);
            stream.SendNext(transform.rotation);
            stream.SendNext(CombatManager.Health);
            stream.SendNext(playerMovement.lookDir);
        }
        else
        {
            // Network player, receive data
            networkPos = (Vector2)stream.ReceiveNext();
            networkVelocity = (Vector2)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            networkHealth = (int)stream.ReceiveNext();
            networkDir = (int)stream.ReceiveNext();

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
