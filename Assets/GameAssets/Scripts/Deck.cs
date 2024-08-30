using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour,ZoneInterface
{
    [SerializeField]bool Isplayer;
    void Start()
    {
    }
    public void OnTransformChildrenChanged()
    {
        List<GameCard> gameCards=new List<GameCard>();
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            gameCards.Add(gameObject.transform.GetChild(i).GetComponent<GameCard>());
        }
        if(Isplayer)
        {
            Context_class.DeckOfPlayer.cards=gameCards;
        }
        else
        {
            Context_class.DeckOfOpponent.cards=gameCards;
        }
    }
    public void SyncWithList()
    {
        if(Isplayer)
        {
            foreach(GameCard gameCard in Context_class.DeckOfPlayer.cards)
            {
                gameCard.gameObject.transform.SetParent(gameObject.transform);
            }
        }
        else
        {
           foreach(GameCard gameCard in Context_class.DeckOfOpponent.cards)
            {
                gameCard.gameObject.transform.SetParent(gameObject.transform);
            }
        }
        
    }

    public void UpdateCardsProperties()
    {
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            gameObject.transform.GetChild(i).GetComponent<GameCard>().RefreshProperties();
        }
    }
}

