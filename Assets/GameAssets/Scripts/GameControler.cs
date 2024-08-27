using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControler : MonoBehaviour
{
    Interpreter interpreter;
    [SerializeField]GameObject CardPrefab;
    [SerializeField]GameObject DeckOfPlayer;
    [SerializeField]GameObject DeckOfOpponent;
    [SerializeField]GameObject FieldOfPlayer;
    [SerializeField]GameObject FieldOfOpponent;
    [SerializeField]GameObject HandOfPlayer;
    [SerializeField]GameObject HandOfOpponent;
    [SerializeField]GameObject GraveyardOfPlayer;
    [SerializeField]GameObject GraveyardOfOpponent;
    [SerializeField] GameObject coin;
    ZoneInterface[]Zones;
    public bool IsPlayerTurn;
    
    void Start()
    {
        DataCards dataCards=GameObject.FindWithTag("Data").GetComponent<DataCards>();
        interpreter=new Interpreter(dataCards.elementalProgram);
        CreateCards(dataCards.PlayerDeck,true);
        CreateCards(dataCards.OpponentDeck,false);
        Draw(10,true);
        Draw(10,false);
        Zones=new ZoneInterface[8]
        {
            DeckOfPlayer.GetComponent<Deck>(),DeckOfOpponent.GetComponent<Deck>(),HandOfPlayer.GetComponent<Hand>(),HandOfOpponent.GetComponent<Hand>(),GraveyardOfPlayer.GetComponent<Graveyard>(),GraveyardOfOpponent.GetComponent<Graveyard>(),FieldOfPlayer.GetComponent<Field>(),FieldOfOpponent.GetComponent<Field>()
        };
        Invoke("StartTurn",1f);
    }
    void Draw(int numberOfCards,bool IsPlayer)
    {
        if(IsPlayer)
        {
            for(int i=0;i<numberOfCards;i++)
            {
                try
                {
                    DeckOfPlayer.transform.GetChild(0).SetParent(HandOfPlayer.transform);
                }
                catch
                {
                    Debug.Log("there isnt any cards in player deck");
                }
                
            }
        }
        else
        {
            for(int i=0;i<numberOfCards;i++)
            {
                try
                {
                    DeckOfOpponent.transform.GetChild(0).SetParent(HandOfOpponent.transform);
                }
                catch
                {
                    Debug.Log("there isnt any cards in opponent deck");
                }
            }
        }
    }
    void StartTurn()
    {
        coin.GetComponent<Animator>().enabled=false;
        int x=Random.Range(0,2);
        if(x==0)
        {
            IsPlayerTurn=true;
            coin.transform.rotation=Quaternion.Euler(0,0,0);
        }
        else
        {
            IsPlayerTurn=false;
            coin.transform.rotation=Quaternion.Euler(359,0,0);
        }
        Destroy(coin,1f);
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
            GameObject CardGameObject=Instantiate(CardPrefab,DeckOfPlayer.transform);
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
            GameObject CardGameObject=Instantiate(CardPrefab,DeckOfOpponent.transform);
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
