
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEngine;

public class Interpreter : IVisitorExpression, IVisitorDeclaration
{
    public Scope Scope;
    public List<CompilingError> errors;
    Dictionary<string,Effect>Effects;
    public Interpreter(ElementalProgram elementalProgram)
    {
        Effects=elementalProgram.Effects;
        Scope = new Scope();
        errors = new List<CompilingError>();
        Define("context",new Context_class());
        Define("player",Player.player);
        Define("opponent",Player.opponent);
        
    }
    #region Expressions
    public object Visit_Atom(Atom expression)
    {
        return expression.Value;
    }
    public object Visit_Binary(BinaryExpression expression)
    {
        object left = Evaluate(expression.Left);
        object right = Evaluate(expression.Right);
        switch (expression.Operator.Type)
        {
            case TokenType.MINUS:
                {
                    return Convert.ToDouble(left) - Convert.ToDouble(right);
                    
                }
            case TokenType.PLUS:
                {
                    return Convert.ToDouble(left) + Convert.ToDouble(right);
                    
                }
            case TokenType.STAR:
                {
                    return Convert.ToDouble(left) * Convert.ToDouble(right);
                    
                }
            case TokenType.SLASH:
                {
                    return Convert.ToDouble(left) / Convert.ToDouble(right);
                    
                }
            case TokenType.GREATER:
                {
                    return Convert.ToDouble(left) > Convert.ToDouble(right);
                    
                }
            case TokenType.GREATER_EQUAL:
                {
                    return Convert.ToDouble(left) >= Convert.ToDouble(right);
                }
            case TokenType.LESS:
                {
                    return Convert.ToDouble(left) < Convert.ToDouble(right);
                    
                }
            case TokenType.LESS_EQUAL:
                {
                    return Convert.ToDouble(left) <= Convert.ToDouble(right);
                    
                }
            case TokenType.EQUAL_EQUAL:
                {
                    return left.Equals(right);
                    
                }
            case TokenType.Caret:
                {
                    return Math.Pow( Convert.ToDouble(left),Convert.ToDouble(right));
                    
                }
            case TokenType.AND:
                {
                    return (bool)left && (bool)right;
                }
            case TokenType.OR:
                {
                    return (bool)left || (bool)right;
                }
            case TokenType.ARROBA:
                {
                    return (string)left + (string)right;
                }
            case TokenType.ARROBA_ARROBA:
                {
                    return (string)left + " " + (string)right;
                }
        }
        return null;
    }
    public object Visit_Group(Group expression)
    {
        return Evaluate(expression.expression);
    }
    public object Visit_Unary(Unary expression)
    {
        object left = Evaluate(expression.rigth_expression);
        return -(double)left;
    }
    public object Visit_Get(GetExpression expression)
    {
        object Class = Evaluate(expression.callee);
        return ((GwentClass)Class).Get(expression.name.Value);
    }
    public object Visit_Set(SetExpression expression)
    {
        object Class = Evaluate(expression.getExpression.callee);
        object value = Evaluate(expression.value);
        ((GwentClass)Class).Properties[expression.getExpression.name.Value] = value;
        return value;
    }
    public object? Visit_Metod(MetodExpression expression)
    {
        object Class = Evaluate(expression.callee);
        Funtion funtion = ((GwentClass)Class).FindMetod(expression.name.Value);
        List<object> arguments = new List<object>();
        foreach (Expression expr in expression.arguments)
        {
            arguments.Add(Evaluate(expr));
        }
        return funtion.Call(Class,arguments);
    }
    public void Visit_CallEffect(CallEffect expression)
    {
        Effect calleer = Effects[expression.effect_name];
        ActionText.Add_Action("Executing Action: "+calleer.Name);
        foreach (KeyValuePair<Token, Expression> keyValuePair in expression.arguments)
        {
            Define(keyValuePair.Key.Value, Evaluate(keyValuePair.Value));
        }
        if(expression.Selector.Source==SourceType.parent)
        {
            List<GameCard>source=((PackOfCards)Scope.GetValue("targets")).cards;
            Define("targets",new PackOfCards(expression,this,source));
        }
        else
        {
            Define("targets", new PackOfCards(expression,this));
        }
        List<GameCard> test=((PackOfCards)Scope.GetValue("targets")).cards;
        foreach(GameCard gameCard in test)
        {
            ActionText.Add_Action("Target "+gameCard.name);
        }
        foreach (Stmt stmt in calleer.body)
        {
            Execute(stmt);
        }
    }
    public object Visit_Variable(Variable expression)
    {
        object? value=Scope.GetValue(expression.name);
        if(value==null)
		{
			errors.Add(new CompilingError(expression.Location, ErrorCode.Invalid, "The variable " + expression.name + " does not exist in the current context"));
		}
        return value;
    }
    public object Visit_Assign(AssignExpression expression)
    {
        object value = Evaluate(expression.Right);
        Define(expression.variable.name,value);
        return value;
    }

    #endregion
    #region Declarations
    public void Visit_Card(Card expression)
    {
    }
    public void Visit_Effect(Effect expression)
    {
    }
    public void Visit_Program(ElementalProgram program)
    {
    }
    public void Visit_DeclarationExpression(ExpressionStmt declaration)
    {
        Evaluate(declaration.expression);
    }
    public void Visit_WhileDeclaration(WhileStmt declaration)
    {
        Scope last = this.Scope;
        this.Scope = last.CreateChild();
        while ((bool)Evaluate(declaration.condition))
        {
            ExecuteBlock(declaration.body);
        }
        this.Scope = last;
    }
    public void Visit_ForDeclaration(ForStmt declaration)
    {
        Scope last = this.Scope;
        this.Scope = last.CreateChild();
        PackOfCards pack = (PackOfCards)Scope.GetValue(declaration.ienumerable.Value);
        string identifier=declaration.identifier.Value;
        foreach(GameCard gameCard in pack.cards)
        {
            Define(identifier,gameCard);
            ExecuteBlock(declaration.body);
        }
        this.Scope = last;
    }
    public void Visit_DeclarationEmpty(EmptyStmt declaration)
    {
    }
    #endregion

    public void Define(string name,object value)
    {
        Scope.Set(name,value);
    }
    public object Evaluate(Expression expression)
    {
        object s = expression.Accept(this);
        return s;
    }
    void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }
    public void ExecuteBlock(List<Stmt> body)
    {
        foreach (Stmt stmt in body)
        {
            Execute(stmt);
        }
    }
    public void Interpret(Card card)
    {
        if (card.Effects != null)
        {
            foreach(CallEffect effect in card.Effects)
            {
                Execute(effect);
            }
        }
    }
    public void Print_errors()
    {
        if (errors.Count == 0)
        {
            return;
        }
        foreach (CompilingError error in errors)
        {
            Console.WriteLine(error.Code + "=> " + error.Argument + " at line " + error.location.line + " and column " + error.location.column);
        }
    }
}
