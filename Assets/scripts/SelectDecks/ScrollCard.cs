using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ScrollCard : MonoBehaviour
{
    static SelectDecksControler selectDecksControler;
    public string CardName;
    public bool IsPlayer;
    void Start()
    {
        selectDecksControler=GameObject.FindWithTag("Controler").GetComponent<SelectDecksControler>();
        gameObject.GetComponentInChildren<TMP_InputField>().onEndEdit.AddListener(SendAmount);
    }
    public void SendAmount(string input)
    {
        int result=0;
        if(Int32.TryParse(input,out result))
        {
            if(IsPlayer)
            {
                Card card=selectDecksControler.PlayerCards[CardName];
                selectDecksControler.PlayerDeck[card]=result;
            }
            else
            {
                Card card=selectDecksControler.OpponentCards[CardName];
                selectDecksControler.OpponentDeck[card]=result;
            }
        }
        else
        {
            gameObject.GetComponentInChildren<TMP_InputField>().text="";
        }
    }
}
