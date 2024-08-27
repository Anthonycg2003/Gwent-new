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
    [SerializeField] TMP_Text Type;
    [SerializeField] TMP_Text Range;
     public DataType dataType{get{return DataType.Card;}}
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}

    public Card Card;
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
        Type.text=Card.Type.Value;
        if(Range!=null)
        {
            string s="";
            foreach(string Range in Card.Ranges)
            {
                s+=Range+",";
            }
            Range.text=s;
        }
        Metods=new Dictionary<string, Funtion>();
    }
}
