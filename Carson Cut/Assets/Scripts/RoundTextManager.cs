using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundTextManager : MonoBehaviour
{
    public TMP_Text TMPComponent;
    public MainManager mainManager;
    // Start is called before the first frame update
    void Update()
    {
        TMPComponent.text = mainManager.roundNum.ToString();
    }
}
