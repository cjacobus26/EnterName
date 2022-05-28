using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        //Set Banner Name
        if (this.gameObject.name == "Knight Life Banner(Clone)")
        {
            if (PhotonNetwork.IsMasterClient && photonView.IsMine)
            {
                this.gameObject.transform.SetParent(GameObject.Find("Canvas/Health Bar 1").GetComponent<Transform>(), false);
                this.gameObject.name = "Knight Life Banner " + (photonView.ViewID - 1001);
            }
            else if (PhotonNetwork.IsMasterClient && !photonView.IsMine)
            {
                this.gameObject.transform.SetParent(GameObject.Find("Canvas/Health Bar 2").GetComponent<Transform>(), false);
                this.gameObject.name = "Knight Life Banner " + (photonView.ViewID - 2001);
                this.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (!PhotonNetwork.IsMasterClient && photonView.IsMine)
            {
                this.gameObject.transform.SetParent(GameObject.Find("Canvas/Health Bar 2").GetComponent<Transform>(), false);
                this.gameObject.name = "Knight Life Banner " + (photonView.ViewID - 2001);
                this.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else if (!PhotonNetwork.IsMasterClient && !photonView.IsMine)
            {
                this.gameObject.transform.SetParent(GameObject.Find("Canvas/Health Bar 1").GetComponent<Transform>(), false);
                this.gameObject.name = "Knight Life Banner " + (photonView.ViewID - 1001);
            }
        }
    }
}
