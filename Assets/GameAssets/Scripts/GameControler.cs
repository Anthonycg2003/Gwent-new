using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControler : MonoBehaviour
{
    Interpreter interpreter;
    [SerializeField]GameObject CardPrefab;
    [SerializeField]Transform PlayerDeck;
    [SerializeField]Transform OpponentDeck;
    
    void Start()
    {
        DataCards dataCards=GameObject.FindWithTag("Data").GetComponent<DataCards>();
        interpreter=new Interpreter(dataCards.elementalProgram);
        CreateCards(dataCards.PlayerDeck,true);
        CreateCards(dataCards.OpponentDeck,false);
    }
    void CreateCards(Dictionary<Card,int> Deck,bool IsPlayer)
    {
        foreach(KeyValuePair<Card,int> keyValuePair in Deck)
        {
            if(keyValuePair.Value==0)
            {
                continue;
            }
            else
            {
                CreateCard(keyValuePair.Key,IsPlayer,keyValuePair.Value);
            }
        }
    }
    void CreateCard(Card card,bool IsPlayer,int factor)
    {
        if(IsPlayer)
        {
            card.Properties["Owner"]=Player.player;
            GameObject CardGameObject=Instantiate(CardPrefab,PlayerDeck);
            CardGameObject.GetComponent<GameCard>().Card=card;
            CardGameObject.GetComponent<GameCard>().UpdateProperties();
            factor--;
            if(factor>0)
            {
                CreateCard(card,IsPlayer,factor);
            }
        }
        else
        {
            card.Properties["Owner"]=Player.opponent;
            GameObject CardGameObject=Instantiate(CardPrefab,OpponentDeck);
            CardGameObject.GetComponent<GameCard>().Card=card;
            CardGameObject.GetComponent<GameCard>().UpdateProperties();
            factor--;
            if(factor>0)
            {
                CreateCard(card,IsPlayer,factor);
            }
        }
    }

}
