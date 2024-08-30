using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class ActionText
{
    public static TMP_Text Action_Text=GameObject.FindWithTag("ActionText").GetComponent<TMP_Text>();
    public static void Add_Action(string text)
    {
        Action_Text.text+="\n"+text;
    }
    
}
