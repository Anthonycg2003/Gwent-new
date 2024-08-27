using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour,ZoneInterface
{
    [SerializeField]bool Isplayer;
    Transform ThisTransform;
    void Start()
    {
        ThisTransform=gameObject.transform;
    }
    public void OnTransformChildrenChanged()
    {
        List<GameCard> gameCards=new List<GameCard>();
        for(int i=0;i<ThisTransform.childCount;i++)
        {
            gameCards.Add(ThisTransform.GetChild(i).GetComponent<GameCard>());
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
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        else
        {
           foreach(GameCard gameCard in Context_class.DeckOfOpponent.cards)
            {
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        
    }
}

