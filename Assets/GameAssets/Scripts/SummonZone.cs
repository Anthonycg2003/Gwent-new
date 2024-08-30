using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummonZone : MonoBehaviour
{
    public bool IsActive;
    [SerializeField] Field field;
    GameCard AsociatedCard;
    List<Range> ActivateZones;
    static GameControler gameControler;
    [SerializeField]Text text_Power;
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
    public int RefreshPower()
    {
        int power=0;
        for(int i=0;i<gameObject.transform.childCount;i++)
        {
            power+=Convert.ToInt32(gameObject.transform.GetChild(i).GetComponent<GameCard>().Properties["Power"]);
        }
        text_Power.text=power.ToString();
        return power;
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
        RefreshPower();
    }
}

