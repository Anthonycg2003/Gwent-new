using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtomSelectFaction : MonoBehaviour
{
    public string Faction;
    static SelectDecksControler selectDecksControler;
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SelectFaction);
        selectDecksControler=GameObject.FindWithTag("Controler").GetComponent<SelectDecksControler>();
    }
    void SelectFaction()
    {
        GameObject parent=gameObject.transform.parent.gameObject;
        GameObject text=parent.GetComponentInChildren<TMP_Text>().gameObject;
        if(gameObject.tag=="Opponent")
        {
            selectDecksControler.ShowCards(Faction,false);
        }
        if(gameObject.tag=="Player")
        {
            selectDecksControler.ShowCards(Faction,true);
        }
    }
}
