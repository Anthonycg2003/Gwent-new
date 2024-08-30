using System;
using System.Collections.Generic;
using System.Data;

public class Card:Stmt,GwentClass
{
    public Card(CodeLocation location,string Name,string Faction,CardType Type,List<Range>? Ranges,int? Power,List<CallEffect>? funtion):base(location)
    {
        this.Name=Name;
        Effects=funtion;
        this.Power=Power;
        this.Ranges=Ranges;
        this.Faction=Faction;
        this.Type=Type;
        Properties=new Dictionary<string, object?>
        {
            {"Name",Name},{"Faction",Faction},{"Power",Power},{"Owner",Player.none}
        };
        Metods=new Dictionary<string, Funtion>();
    }
    
    public DataType dataType{get{return DataType.Card;}}
    public Dictionary<string,Funtion> Metods{get;set;}
    public Dictionary<string,object?> Properties{get;set;}
    public string name{get{return "card";}}
    public string Name { get; set; }
    public string Faction { get; set; }
    public CardType Type { get; set; }
    public List<Range>? Ranges{get;set;}
    public int? Power { get; set; }
    public List<CallEffect>? Effects;
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_Card(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Card(this);
    }
    public bool CheckPorperties(List<CompilingError> errors)
    {
            if(Type==CardType.Plata||Type==CardType.Oro)
            {
                if(this.Power==null||this.Ranges==null)
                {
                    errors.Add(new CompilingError(Location, ErrorCode.Invalid,"gold and silver cards should have range and power properties"));
                    return false;
                }
                return true;  
            }
            else
            {
                return true;
            }
    }
}
public class CallEffect:Stmt//llamada a funciones
{
    public string effect_name;
    public Dictionary<Token,Expression> arguments;
    public Selector Selector;
    public CallEffect(CodeLocation location,Dictionary<Token,Expression> arguments,string calleer,Selector selector) : base(location)
    {
        Selector=selector;
        effect_name=calleer;
        this.arguments=arguments;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_CallEffect(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_CallEffect(this);
    }
}
public class Selector:Stmt//llamada a funciones
{
    public bool Single{get;set;}
    public SourceType Source{get;set;}
    public PredicateStmt predicateStmt{get;set;}
    public Selector? parent;
    public Selector(CodeLocation location,bool single,SourceType source,PredicateStmt predicateStmt,Selector parent) : base(location)
    {
        this.parent=parent;
        Single=single;
        Source=source;
        this.predicateStmt=predicateStmt;
    }
    public override void Accept(IVisitorDeclaration visitor)
    {
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Selector(this);
    }
}
public enum SourceType
{
    board,hand,otherHand,deck,otherDeck,field,otherField,parent
}
public enum Range
{
    Melee,Range,Siege
}
public enum CardType
{
    Oro,Plata,Clima,Aumento,Lider
}