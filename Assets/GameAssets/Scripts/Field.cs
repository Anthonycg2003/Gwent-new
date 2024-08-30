using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour,ZoneInterface
{
    [SerializeField]bool Isplayer;
    GameObject[]Zones;
    static GameControler gameControler;
    void Start()
    {
        GameObject MeleeZone;
        GameObject RangeZone;
        GameObject SiegeZone;
        gameControler=GameObject.FindWithTag("Controler").GetComponent<GameControler>();
        if(Isplayer)
        {
            MeleeZone=gameControler.PlayerMeleeZone;
            RangeZone=gameControler.PlayerRangeZone;
            SiegeZone=gameControler.PlayerSiegeZone;
        }
        else
        {
            MeleeZone=gameControler.OpponentMeleeZone;
            RangeZone=gameControler.OpponentRangeZone;
            SiegeZone=gameControler.OpponentSiegeZone;
        }
        Zones=new GameObject[3]{MeleeZone,RangeZone,SiegeZone};
    }
    public void UpdateContext()
    {
        List<GameCard> gameCards=new List<GameCard>();
        foreach(GameObject zone in Zones)
        {
            for(int i=0;i<zone.transform.childCount;i++)
            {
                gameCards.Add(zone.transform.GetChild(i).GetComponent<GameCard>());
            }
        }
        if(Isplayer)
        {
            Context_class.FieldOfPlayer.cards=gameCards;
            
        }
        else
        {
            Context_class.FieldOfOpponent.cards=gameCards;
           
        }
       Context_class.SyncBoard();
    }
    public void SyncWithList()
    {
        
    }
    public void UpdateCardsProperties()
    {
        foreach(GameObject zone in Zones)
        {
            for(int i=0;i<zone.transform.childCount;i++)
            {
                zone.transform.GetChild(i).GetComponent<GameCard>().RefreshProperties();
            }
        }
    }

    public void OnTransformChildrenChanged()
    {
        
    }
}
