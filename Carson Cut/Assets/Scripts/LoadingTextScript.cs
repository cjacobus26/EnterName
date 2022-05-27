using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingTextScript : MonoBehaviour
{
    public TMP_Text loadingText;

    private int updateNum;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(UpdateLoadingText());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateLoadingText()
    {
        updateNum++;

        if(updateNum == 1)
        {
            loadingText.text = "Loading";
        }

        if (updateNum == 2)
        {
            loadingText.text = "Loading .";
        }

        if (updateNum == 3)
        {
            loadingText.text = "Loading . .";
        }

        if (updateNum == 4)
        {
            loadingText.text = "Loading . . .";

            updateNum = 0;
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(UpdateLoadingText());
    }
}
