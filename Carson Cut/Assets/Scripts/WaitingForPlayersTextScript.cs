using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitingForPlayersTextScript : MonoBehaviour
{
    public TMP_Text waitingText;

    private int updateNum;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(UpdateWaitingText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateWaitingText()
    {
        updateNum++;

        if(updateNum == 1)
        {
            waitingText.text = "Waiting For Players";
        }

        if (updateNum == 2)
        {
            waitingText.text = "Waiting For Players .";
        }

        if (updateNum == 3)
        {
            waitingText.text = "Waiting For Players . .";
        }

        if (updateNum == 4)
        {
            waitingText.text = "Waiting For Players . . .";

            updateNum = 0;
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(UpdateWaitingText());
    }
}
