using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class buttom : MonoBehaviour
{
    public Image image;
    public void ChangeColorAtStart()
    {
        image.gameObject.SetActive(true);
        gameObject.GetComponent<TMP_Text>().color=Color.red;
    }
    public void ChangeColorAtEnd()
    {
        image.gameObject.SetActive(false);
        gameObject.GetComponent<TMP_Text>().color=Color.white;
    }
}
