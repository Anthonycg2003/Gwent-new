using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SelectDecksControler : MonoBehaviour
{
    [SerializeField]GameObject FactionPrefab;
    [SerializeField]GameObject ScrollCardPrefab;
    [SerializeField]GameObject ButtomValidatePrefab;
    [SerializeField]GameObject ReadyTextPrefab;
    [SerializeField]GameObject ScrollPlayer;
    [SerializeField]GameObject ScrollOpponent;
    Dictionary<string,List<Card>> dataCards;
    public Dictionary<string,Card> PlayerCards;
    public Dictionary<string,Card> OpponentCards;
    public Dictionary<Card,int> PlayerDeck;
    public Dictionary<Card,int> OpponentDeck;
    bool IsPlayerReady;
    bool IsOpponentReady;
    void Start()
    {
        dataCards=GameObject.FindWithTag("Data").GetComponent<DataCards>().elementalProgram.Cards;
        PlayerCards=new Dictionary<string, Card>();
        OpponentCards=new Dictionary<string, Card>();
        PlayerDeck=new Dictionary<Card, int>();
        OpponentDeck=new Dictionary<Card, int>();
        IsPlayerReady=false;
        IsOpponentReady=false;
        GenerateFactions();
    }
    void Play()
    {
        GameObject.FindWithTag("Data").GetComponent<DataCards>().PlayerDeck=PlayerDeck;
        GameObject.FindWithTag("Data").GetComponent<DataCards>().OpponentDeck=OpponentDeck;
        SceneManager.LoadScene(2);
    }
    public void ValidateDeck(bool IsPlayer)
    {
        int CardsCount=0;
        if(IsPlayer)
        {
            int[]CardsNumber=PlayerDeck.Values.ToArray();
            foreach(int n in CardsNumber)
            {
                CardsCount+=n;
            }
            if(CardsCount>=25)
            {
                Instantiate(ReadyTextPrefab,GameObject.FindWithTag("text Player").transform);
                Destroy(GameObject.FindWithTag("Player Visualizer"));
                IsPlayerReady=true;
                if(IsOpponentReady)
                {
                    Play();
                }
            }
            else
            {
                Debug.Log("invalid number of cards");
            }
        }
        else
        {
            int[]CardsNumber=OpponentDeck.Values.ToArray();
            foreach(int n in CardsNumber)
            {
                CardsCount+=n;
            }
            if(CardsCount>=25)
            {
                Instantiate(ReadyTextPrefab,GameObject.FindWithTag("text Opponent").transform);
                Destroy(GameObject.FindWithTag("Opponent Visualizer"));
                IsOpponentReady=true;
                if(IsPlayerReady)
                {
                    Play();
                }
            }
            else
            {
                Debug.Log("invalid number of cards");
            }
        }
    }
    void Alert(string message)
    {

    }
    public void ShowCards(string Faction,bool IsPlayer)
    {
        GameObject scroll;
        if(IsPlayer)
        {
            scroll=ScrollPlayer;
        }
        else
        {
            scroll=ScrollOpponent;
        }
        Transform transform_scroll=scroll.transform;
        for(int i=0;i<transform_scroll.childCount;i++)
        {
            Destroy(transform_scroll.GetChild(0).gameObject);//delete factions in scroll
        }
        foreach (Card card in dataCards[Faction])
        {
            GameObject scroll_card=CreateScrollCard(card,transform_scroll);//add cards in scroll
            ScrollCard scrollCard=scroll_card.GetComponent<ScrollCard>();
            scrollCard.CardName=card.Name;
            if(IsPlayer)
            {
                PlayerCards[card.Name]=card;
                scrollCard.IsPlayer=true;
            }
            else
            {
                scrollCard.IsPlayer=false;
                OpponentCards[card.Name]=card;
            }
        }
        if(IsPlayer)
        {
            GameObject ButtomValidate=Instantiate(ButtomValidatePrefab,transform_scroll);
            ButtomValidate.GetComponent<ButtomValidateDeck>().IsPlayer=true;
        }
        else
        {
            GameObject ButtomValidate=Instantiate(ButtomValidatePrefab,transform_scroll);
            ButtomValidate.GetComponent<ButtomValidateDeck>().IsPlayer=false;
        }
    }
    GameObject CreateScrollCard(Card card,Transform parent)
    {
        GameObject scroll_card=Instantiate(ScrollCardPrefab,parent);
        scroll_card.GetComponentInChildren<TMP_Text>().text=card.Name;
        return scroll_card;
    }
    void GenerateFactions()
    {
        foreach(string Faction in dataCards.Keys)
        {
            GameObject factionOptionOpponent=Instantiate(FactionPrefab,ScrollOpponent.transform);
            factionOptionOpponent.GetComponentInChildren<TMP_Text>().text=Faction;
            Button buttonOpponent=factionOptionOpponent.GetComponentInChildren<Button>();
            buttonOpponent.GetComponent<ButtomSelectFaction>().Faction=Faction;
            buttonOpponent.gameObject.tag="Opponent";
            //cambiar texto
            GameObject factionOptionPlayer=Instantiate(factionOptionOpponent,ScrollPlayer.transform);//duplicar en player
            Button buttonPlayer=factionOptionPlayer.GetComponentInChildren<Button>();
            buttonPlayer.gameObject.tag="Player";
            buttonPlayer.GetComponent<ButtomSelectFaction>().Faction=Faction;
        }
    }

}
