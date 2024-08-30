using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonZone : MonoBehaviour
{
    public bool IsActive;
    [SerializeField] Field field;
    GameCard AsociatedCard;
    List<Range> ActivateZones;
    static GameControler gameControler;
    void Start()
    {
        IsActive=false;
        gameControler=GameObject.FindWithTag("Controler").GetComponent<GameControler>();
    }
    public void ActiveZone(GameCard gameCard,List<Range>ActivateZones)
    {
        IsActive=true;
        AsociatedCard=gameCard;
        this.ActivateZones=ActivateZones;
    }
    void OnMouseDown()
    {
        if(IsActive)
        {
            gameControler.SummonIn(gameObject.transform,ActivateZones,AsociatedCard);
        }
    }
    public void DisableZone()
    {
        IsActive=false;
        AsociatedCard=null;
        ActivateZones=null;
    }
    public void OnTransformChildrenChanged()
    {
        field.UpdateContext();
    }
}
