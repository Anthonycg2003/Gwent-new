using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class SelectDecksControler : MonoBehaviour
{
    [SerializeField]GameObject FactionPrefab;
    [SerializeField]GameObject ScrollFactionsPlayer;
    [SerializeField]GameObject ScrollFactionsOpponent;
    Dictionary<string,List<Card>> dataCards;
    void Start()
    {
        dataCards=GameObject.FindWithTag("Data").GetComponent<DataCards>().elementalProgram.Cards;
        GenerateFactions();
    }
    void GenerateFactions()
    {
        
        foreach(string Faction in dataCards.Keys)
        {
            GameObject factionOption=Instantiate(FactionPrefab,ScrollFactionsOpponent.transform);
            factionOption.GetComponentInChildren<TMP_Text>().text=Faction;
            //cambiar texto
            Instantiate(factionOption,ScrollFactionsPlayer.transform);
        }
    }

}
