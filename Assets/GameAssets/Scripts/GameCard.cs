using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCard : MonoBehaviour
{
    [SerializeField] TMP_Text Name;
    [SerializeField] TMP_Text Faction;
    [SerializeField] TMP_Text Power;
    [SerializeField] TMP_Text Type;
    [SerializeField] TMP_Text Range;
    public Card Card;
    public void UpdateProperties()
    {
        name=Card.Name;
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
        
    }

}
