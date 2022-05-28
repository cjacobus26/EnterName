using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject Player1WinsText, Player2WinsText;
    public GameObject Player1WinsGameText, Player2WinsGameText;
    public GameObject readyText, fightText;
    public GameObject KnightLifeBannerPrefab;
    public GameObject Player1, Player2;
    private GameObject P1Banner1, P1Banner2, P1Banner3, P2Banner1, P2Banner2, P2Banner3;

    public AudioSource readyClip, fightClip;

    public int roundNum;
    public int Player1Lives = 3, Player2Lives = 3;
    public int RoundWinTime = 3;
    private int ReadyWait = 1, FightWait = 1;

    public bool GameRunning = false;

    private Vector3 BannerSpace = new Vector3(40, 0, 0);
    private Vector3 SpawnPos1 = new Vector3(-5.75f, -2.4f, 0);
    private Vector3 SpawnPos2 = new Vector3(5.75f, -2.4f, 0);

    // Start is called before the first frame update
    void Start()
    {
        //Spawn Players and Banners
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer1();
            SpawnBanner1();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer2();
            SpawnBanner2();
        }

        //Start Ready Text
        StartCoroutine(TurnOnReadyText(ReadyWait));
        roundNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //If Player is null find Player
        if (Player1 == null && Player2 != null && !PhotonNetwork.IsMasterClient)
        {
            Player1 = GameObject.Find("Player1");
        }

        if (Player2 == null && Player1 != null && PhotonNetwork.IsMasterClient)
        {
            Player2 = GameObject.Find("Player2");
        }

        if (Player1 != null)
        {
            if (GameRunning)
            {
                if (Player1.GetComponentInChildren<combatManager>().transform.position.y < -5)
                {
                    if (Player1.GetComponentInChildren<combatManager>().Health > 0)
                    {
                        //If Player2 is Below Map
                        Player1.GetComponentInChildren<combatManager>().TakeDamage(Player1.GetComponentInChildren<combatManager>().maxHealth, 0);
                    }
                }

                if (Player1.GetComponentInChildren<combatManager>().Health <= 0)
                {
                    //If Player1 Died
                    GameRunning = false;
                    Player1Lives--;
                    StartCoroutine(Player2RoundWin(RoundWinTime));
                }
            }
        }

        if (Player2 != null)
        {
            if (GameRunning)
            {
                if (Player2.GetComponentInChildren<combatManager>().transform.position.y < -5)
                {
                    if (Player2.GetComponentInChildren<combatManager>().Health > 0)
                    {
                        //If Player2 is Below Map
                        Player2.GetComponentInChildren<combatManager>().TakeDamage(Player2.GetComponentInChildren<combatManager>().maxHealth, 0);
                    }
                }

                if (Player2.GetComponentInChildren<combatManager>().Health <= 0)
                {
                    //If Player2 Died
                    GameRunning = false;
                    Player2Lives--;
                    StartCoroutine(Player1RoundWin(RoundWinTime));
                }
            }
        }
    }

    void SpawnPlayer1()
    {
        Player1 = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos1, Quaternion.identity);
        Player1.name = "Player1";
        Player1.tag = "Knight";
        Player1.GetComponent<PlayerMovement>().lookDir = 1;
    }

    void SpawnPlayer2()
    {
        Player2 = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPos2, Quaternion.identity);
        Player2.name = "Player2";
        Player2.tag = "Knight";
        Player2.transform.eulerAngles = new Vector3(0, 180, 0);
        Player2.GetComponent<PlayerMovement>().lookDir = -1;
    }

    void SpawnBanner1()
    {
        P1Banner1 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, KnightLifeBannerPrefab.transform.position, transform.rotation);
        P1Banner1.transform.SetParent(GameObject.Find("Canvas/Health Bar 1").transform, false);
        P1Banner1.name = "Knight Life Banner 1";

        P1Banner2 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, KnightLifeBannerPrefab.transform.position + BannerSpace, transform.rotation);
        P1Banner2.transform.SetParent(GameObject.Find("Canvas/Health Bar 1").transform, false);
        P1Banner2.name = "Knight Life Banner 2";

        P1Banner3 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, KnightLifeBannerPrefab.transform.position + (BannerSpace * 2), transform.rotation);
        P1Banner3.transform.SetParent(GameObject.Find("Canvas/Health Bar 1").transform, false);
        P1Banner3.name = "Knight Life Banner 3";
    }

    void SpawnBanner2()
    {
        P2Banner1 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, new Vector3(120.98999f, -30.1900024f, 0f), transform.rotation);
        P2Banner1.transform.SetParent(GameObject.Find("Canvas/Health Bar 2").transform, false);
        P2Banner1.name = "Knight Life Banner 1";
        P2Banner1.transform.eulerAngles = new Vector3(0, 180, 0);

        P2Banner2 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, new Vector3(120.98999f, -30.1900024f, 0f) + -BannerSpace, transform.rotation);
        P2Banner2.transform.SetParent(GameObject.Find("Canvas/Health Bar 2").transform, false);
        P2Banner2.name = "Knight Life Banner 2";
        P2Banner2.transform.eulerAngles = new Vector3(0, 180, 0);

        P2Banner3 = PhotonNetwork.Instantiate(KnightLifeBannerPrefab.name, new Vector3(120.98999f, -30.1900024f, 0f) + (-BannerSpace * 2), transform.rotation);
        P2Banner3.transform.SetParent(GameObject.Find("Canvas/Health Bar 2").transform, false);
        P2Banner3.name = "Knight Life Banner 3";
        P2Banner3.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    void RestartRound()
    {
        roundNum++;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(Player1);
            SpawnPlayer1();
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(Player2);
            SpawnPlayer2();
        }

        Player1 = GameObject.Find("Player1");
        Player2 = GameObject.Find("Player2");

        StartCoroutine(TurnOnReadyText(ReadyWait));
    }

    public IEnumerator Player1RoundWin(int waitTime)
    {
        //Freeze Player1
        Player1.GetComponent<PlayerMovement>().canMove = true;
        Player1.GetComponent<PlayerMovement>().Froze = true;

        //Remove Player2 Banner Head
        if (Player2Lives == 2)
        {
            GameObject.Find("Health Bar 2/Knight Life Banner 3/Head").SetActive(false);
        }

        if (Player2Lives == 1)
        {
            GameObject.Find("Health Bar 2/Knight Life Banner 2/Head").SetActive(false);
        }

        if (Player2Lives == 0)
        {
            //If Player2 Died
            GameObject.Find("Health Bar 2/Knight Life Banner 1/Head").SetActive(false);
            Player1Win();
        }

        if (Player2Lives > 0)
        {
            //If Player2 has lives
            Player1WinsText.SetActive(true);
        }
        yield return new WaitForSeconds(waitTime);
        if (Player1Lives > 0 && Player2Lives > 0)
        {
            //If Player2 has lives Restart Round
            Player1WinsText.SetActive(false);
            RestartRound();
        }
    }
    public IEnumerator Player2RoundWin(int waitTime)
    {
        //Freeze Player2
        Player2.GetComponent<PlayerMovement>().canMove = true;
        Player2.GetComponent<PlayerMovement>().Froze = true;

        //Remove Player1 Banner Head
        if (Player1Lives == 2)
        {
            GameObject.Find("Health Bar 1/Knight Life Banner 3/Head").SetActive(false);
        }

        if (Player1Lives == 1)
        {
            GameObject.Find("Health Bar 1/Knight Life Banner 2/Head").SetActive(false);
        }

        if (Player1Lives == 0)
        {
            //If Player1 Died
            GameObject.Find("Health Bar 1/Knight Life Banner 1/Head").SetActive(false);
            Player2Win();
        }

        if (Player1Lives > 0 && Player2Lives > 0)
        {
            //If Player1 has lives
            Player2WinsText.SetActive(true);
        }
        yield return new WaitForSeconds(waitTime);
        if (Player1Lives > 0 && Player2Lives > 0)
        {
            //If Player1 has lives Restart Round
            Player2WinsText.SetActive(false);
            RestartRound();
        }
    }
    public void Player1Win()
    {
        Player1WinsGameText.SetActive(true);
        StartCoroutine(WaitToLeave(5));
    }
    public void Player2Win()
    {
        Player2WinsGameText.SetActive(true);
        StartCoroutine(WaitToLeave(5));
    }

    IEnumerator TurnOnReadyText(int waitTime)
    {
        readyText.SetActive(true);
        readyClip.Play();
        yield return new WaitForSeconds(waitTime);
        readyText.SetActive(false);
        StartCoroutine(TurnOnFightText(FightWait));
    }

    IEnumerator TurnOnFightText(int waitTime)
    {
        fightText.SetActive(true);
        fightClip.Play();
        yield return new WaitForSeconds(waitTime);
        fightText.SetActive(false);

        GameRunning = true;

        //Unfreeze Players
        if (Player1 != null)
        {
            Player1.GetComponent<PlayerMovement>().canMove = true;
        }

        if (Player2 != null)
        {
            Player2.GetComponent<PlayerMovement>().canMove = true;
        }
    }

    IEnumerator WaitToLeave(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyCreate");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
    }
}
