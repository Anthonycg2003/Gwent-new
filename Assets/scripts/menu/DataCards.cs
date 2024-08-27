using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCards : MonoBehaviour
{
    public ElementalProgram elementalProgram;
    public Dictionary<Card,int> PlayerDeck;
    public Dictionary<Card,int> OpponentDeck;
    void Start()
    {
        PlayerDeck=new Dictionary<Card, int>();
        OpponentDeck=new Dictionary<Card, int>();
    }
}
