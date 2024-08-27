using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour,ZoneInterface
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
            Context_class.FieldOfPlayer.cards=gameCards;
        }
        else
        {
            Context_class.FieldOfOpponent.cards=gameCards;
        }
    }
    public void SyncWithList()
    {
        if(Isplayer)
        {
            foreach(GameCard gameCard in Context_class.FieldOfPlayer.cards)
            {
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        else
        {
           foreach(GameCard gameCard in Context_class.FieldOfOpponent.cards)
            {
                gameCard.gameObject.transform.SetParent(ThisTransform);
            }
        }
        
    }
}
