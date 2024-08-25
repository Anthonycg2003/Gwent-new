using System.Collections.Generic;
using System.Data;



public class ElementalProgram:Stmt
{
    public ElementalProgram(CodeLocation location) : base (location)
    {
        Errors = new List<CompilingError>();
        Cards = new Dictionary<string, List<Card>>();
        Effects=new Dictionary<string, Effect>();
    }

    public List<CompilingError> Errors { get; set; }
    public Dictionary<string, List<Card>> Cards { get; set; }
    public Dictionary<string, Effect> Effects { get; set; }
    public override void Accept(IVisitorDeclaration visitor)
    {
        visitor.Visit_Program(this);
    }
    public override void CheckSemantic(SemanticAnalizer semanticAnalizer)
    {
        semanticAnalizer.Visit_Program(this);
    }
}
