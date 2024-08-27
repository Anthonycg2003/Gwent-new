using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour,ZoneInterface
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
            Context_class.GraveyardOfPlayer.cards=gameCards;
        }
        else
        {
            Context_class.GraveyardOfOpponent.cards=gameCards;
        }
    }
    public void SyncWithList()
    {
        if(Isplayer)
        {
            foreach(GameCard gameCard in Context_class.GraveyardOfPlayer.cards)
            {
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        else
        {
           foreach(GameCard gameCard in Context_class.GraveyardOfOpponent.cards)
            {
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        
    }
}
