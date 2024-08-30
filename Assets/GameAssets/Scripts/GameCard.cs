using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameCard : MonoBehaviour,GwentClass
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Faction;
    [SerializeField] TMP_Text Power;
    public TMP_Text Type;
    [SerializeField] TMP_Text Range;
     public DataType dataType{get{return DataType.Card;}}
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}
    public bool InBattle;
    public Card Card;
     static GameControler gameControler;
    void Start()
    {
        InBattle=false;
        gameControler=GameObject.FindWithTag("Controler").GetComponent<GameControler>();
    }
    public void UpdateProperties()
    {
        name=Card.Name;
        Properties=new Dictionary<string, object?>
        {
            {"Name",name},{"Faction",Card.Faction},{"Power",Card.Power},{"Owner",Card.Properties["Owner"]}
        };
        Name.text=name;
        Faction.text=Card.Faction;
        Power.text=Card.Power.ToString();
        Type.text=Card.Type.ToString();
        if(Range!=null)
        {
            string s="";
            foreach(Range Range in Card.Ranges)
            {
                s+=Range+",";
            }
            Range.text=s;
        }
        Metods=new Dictionary<string, Funtion>();
    }
    public void RefreshProperties()
    {
        Name.text=(string)Properties["Name"];
        Faction.text=(string)Properties["Faction"];
        Power.text=Properties["Power"].ToString();
    }
    void OnMouseDown()
    {
        Debug.Log("click");
        gameControler.ManagerCard(this);
    }
}
