using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtomValidateDeck : MonoBehaviour
{
    public bool IsPlayer;
    static SelectDecksControler selectDecksControler;
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Validate);
        selectDecksControler=GameObject.FindWithTag("Controler").GetComponent<SelectDecksControler>();
    }
    void Validate()
    {
        selectDecksControler.ValidateDeck(IsPlayer);
    }
}
