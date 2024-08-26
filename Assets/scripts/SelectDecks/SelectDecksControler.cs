using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SelectDecksControler : MonoBehaviour
{
    [SerializeField]GameObject FactionPrefab;
    [SerializeField]GameObject ScrollCardPrefab;
    [SerializeField]GameObject ScrollFactionsPlayer;
    [SerializeField]GameObject ScrollFactionsOpponent;
    Dictionary<string,List<Card>> dataCards;
    void Start()
    {
        dataCards=GameObject.FindWithTag("Data").GetComponent<DataCards>().elementalProgram.Cards;
        GenerateFactions();
    }
    public void ShowCards(string Faction,bool IsPlayer)
    {
        GameObject scroll;
        if(IsPlayer)
        {
            scroll=ScrollFactionsPlayer;
        }
        else
        {
            scroll=ScrollFactionsOpponent;
        }
        Transform transform_scroll=scroll.transform;
        for(int i=0;i<transform_scroll.childCount;i++)
        {
            Destroy(transform_scroll.GetChild(0).gameObject);//delete factions in scroll
        }
        foreach (Card card in dataCards[Faction])
        {
            CreateScrollCard(card,transform_scroll);//add cards in scroll
        }
    }
    void CreateScrollCard(Card card,Transform parent)
    {
        GameObject scroll_card=Instantiate(ScrollCardPrefab,parent);
        scroll_card.GetComponentInChildren<TMP_Text>().text=card.Name;
    }
    void GenerateFactions()
    {
        foreach(string Faction in dataCards.Keys)
        {
            GameObject factionOptionOpponent=Instantiate(FactionPrefab,ScrollFactionsOpponent.transform);
            factionOptionOpponent.GetComponentInChildren<TMP_Text>().text=Faction;
            Button buttonOpponent=factionOptionOpponent.GetComponentInChildren<Button>();
            buttonOpponent.GetComponent<ButtomSelectFaction>().Faction=Faction;
            buttonOpponent.gameObject.tag="Opponent";
            //cambiar texto
            GameObject factionOptionPlayer=Instantiate(factionOptionOpponent,ScrollFactionsPlayer.transform);//duplicar en player
            Button buttonPlayer=factionOptionPlayer.GetComponentInChildren<Button>();
            buttonPlayer.gameObject.tag="Player";
            buttonPlayer.GetComponent<ButtomSelectFaction>().Faction=Faction;
        }
    }

}
