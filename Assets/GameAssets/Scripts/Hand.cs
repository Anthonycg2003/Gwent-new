using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour,ZoneInterface
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
            Context_class.HandOfPlayer.cards=gameCards;
        }
        else
        {
            Context_class.HandOfOpponent.cards=gameCards;
        }
    }
    public void SyncWithList()
    {
        if(Isplayer)
        {
            foreach(GameCard gameCard in Context_class.HandOfPlayer.cards)
            {
                gameCard.gameObject.transform.SetParent(gameObject.transform);
            }
        }
        else
        {
           foreach(GameCard gameCard in Context_class.HandOfOpponent.cards)
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
