using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Rider.Editor.UnitTesting;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using System.Diagnostics;

public class MenuControler : MonoBehaviour
{
    [SerializeField]GameObject DisplayCompile;
    [SerializeField]TMP_Text errorsText;
    [SerializeField]GameObject errorsDisplay;
    [SerializeField]GameObject CompletedDisplay;
    [SerializeField]GameObject DataCards;
    readonly string path=Application.dataPath+"/Source/source.txt";
    string text;
    bool IsReady;
    void Start()
    {
        text=File.ReadAllText(path);
        IsReady=false;
    }
    public void ReadText()
    {
        DisplayCompile.SetActive(true);
        Compile();
    }
    public void SelectDecks()
    {
        if(IsReady)
        {
            DontDestroyOnLoad(DataCards);
            SceneManager.LoadScene(1);
        }
    }
    void ShowErrors(List<string>errors)
    {
        errorsDisplay.SetActive(true);
        string message="";
        foreach(string error in errors)
        {
            message+=error+"\n";
        }
        errorsText.text=message;
    }
    public void OpenSource()
    {
        Process.Start(new ProcessStartInfo(path));
    }
    public void CloseDisplayCompile()
    {
        DisplayCompile.SetActive(false);
    }
    void Compile()
    {
        Lexer lexer=new Lexer(text);
        List<Token>tokens=lexer.GetTokens();
        if(lexer.errors.Count!=0)
        {
            ShowErrors(lexer.Get_errors());
        }
        else
        {
            Parser Parser=new Parser(tokens);
            ElementalProgram program=Parser.Parse();
            if(Parser.errors.Count!=0)
            {
                ShowErrors(Parser.Get_errors());
            }
            else
            {
                SemanticAnalizer semanticAnalizer=new SemanticAnalizer(program);
                semanticAnalizer.Semantic_Analizer();
                if(semanticAnalizer.errors.Count!=0)
                {
                    ShowErrors(semanticAnalizer.Get_errors()); 
                }
                else
                {
                    CompletedDisplay.SetActive(true);
                    IsReady=true;
                    DataCards.GetComponent<DataCards>().elementalProgram=program;
                }
            }
        }
    }
}
