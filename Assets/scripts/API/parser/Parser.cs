﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;


class Parser
{
    private List<Token> tokens;
    public List<CompilingError> errors;
    private int current = 0;
    bool PanicMode;
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        errors = new List<CompilingError>();
        PanicMode = false;
    }
    #region Internal Metods
    bool IsAtEnd()
    {
        if (current == tokens.Count - 1)
        {
            return true;
        }
        return false;
    }
    Token peek()
    {
        return tokens[current];
    }
    Token previus()
    {
        return tokens[current - 1];
    }
    Token advance()
    {
        if (!IsAtEnd()) { current++; }
        return previus();
    }
    bool Check(TokenType type)
    {
        if (IsAtEnd()) { return false; }
        return peek().Type == type;
    }
    bool match(TokenType type)
    {
        if (Check(type))
        {
            current++;
            return true;
        }
        return false;
    }
    Token Consume(TokenType type, string message)
    {

        if (match(type))
        {
            PanicMode = false;
            return previus();
        }
        errors.Add(new CompilingError(peek().Location, ErrorCode.Expected, message));
        if (PanicMode)
        {
            current++;
        }
        return peek();
    }
    Token Consume(string message,TokenType type,params string[]values)
    {
        if (Check(type))
        {
            foreach(string value in values)
            {
                if(peek().Value==value)
                {
                    PanicMode = false;
                    return advance();
                }
            }
        }
        errors.Add(new CompilingError(peek().Location, ErrorCode.Expected, message));
        if (PanicMode)
        {
            advance();
        }
        return peek();
    }
    Token Consume(string message,params TokenType[] types)
    {
        foreach(TokenType tokenType in types)
        {
            if (match(tokenType))
            {
                PanicMode = false;
                return previus();
            }
        }
        errors.Add(new CompilingError(peek().Location, ErrorCode.Expected, message));
        if (PanicMode)
        {
            current++;
        }
        return peek();
    }
    void Consume(string message)
    {
        errors.Add(new CompilingError(peek().Location, ErrorCode.Expected, message));
        advance();
    }
    public List<string> Get_errors()
    {
        List<string> Errors=new List<string>();
        foreach (CompilingError error in errors)
        {
            Errors.Add(error.Code + "=> " + error.Argument + " at line " + error.location.line + " and column " + error.location.column);
        }
        return Errors;
    }
    #endregion
    public ElementalProgram Parse()
    {
        ElementalProgram program = new ElementalProgram(new CodeLocation());
        while (!IsAtEnd())
        {
            if (match(TokenType.CARD))
            {
                Card card = ParseCard();
                if(program.Cards.ContainsKey(card.Faction))
                {
                    program.Cards[card.Faction].Add(card);
                }
                else
                {
                    program.Cards[card.Faction]=new List<Card>();
                    program.Cards[card.Faction].Add(card);
                }
            }
            else if (match(TokenType.EFFECT))
            {
                Effect effect = ParseEffect();
                program.Effects[effect.Name] = effect;
            }
            else
            {
                Consume("Card or effect declaration expected");
            }
        }
        return program;
    }
    Card ParseCard()
    {
        CodeLocation location = peek().Location;
        string? name = null;
        string? faction = null;
        CardType? type = null;
        List<Range>? ranges = null;
        int? power = null;
        List<CallEffect>? effects = null;
        Consume(TokenType.LEFT_KEY, "{ expected");
        while (!Check(TokenType.RIGHT_KEY))
        {
            switch (peek().Type)
            {
                case TokenType.NAME:
                    {
                        if (name != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Name property has been declared"));
                        }
                        name = ParseName();
                        break;
                    }
                case TokenType.FACTION:
                    {
                        if (faction != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Faction property has been declared"));
                        }
                        faction = ParseFaction();
                        break;
                    }
                case TokenType.TYPE:
                    {
                        if (type != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Type property has been declared"));
                        }
                        advance();
                        Consume(TokenType.COLON, ": expected");
                        Token type_name=Consume(TokenType.STRING, "valid type expected");
                        if(type_name.Type==TokenType.STRING)
                        {
                            switch(type_name.Value)
                            {
                                case "Oro":
                                {
                                    type=CardType.Oro;
                                    break;
                                }
                                case "Plata":
                                {
                                    type=CardType.Plata;
                                    break;
                                }
                                case "Lider":
                                {
                                    type=CardType.Lider;
                                    break;
                                }
                                case "Clima":
                                {
                                    type=CardType.Clima;
                                    break;
                                }
                                case "Aumento":
                                {
                                    type=CardType.Aumento;
                                    break;
                                }
                                default:
                                {
                                    errors.Add(new CompilingError(type_name.Location, ErrorCode.Invalid, String.Format("{0} Type Does not exists",type_name.Value)));
                                    break;
                                }
                            }
                        }
                        Consume(TokenType.COMMA, ", expected");
                        break;
                    }
                case TokenType.POWER:
                    {
                        if (power != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Power property has been declared"));
                        }
                        power = ParsePower();
                        break;
                    }
                case TokenType.RANGE:
                    {
                        if (ranges != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Range property has been declared"));
                        }
                        ranges = new List<Range>();
                        advance();
                        Consume(TokenType.COLON, ": expected");
                        Consume(TokenType.LEFT_BRACE, "[ expected");
                        bool into_range = true;
                        while (into_range)
                        {
                            Token Range=Consume(TokenType.STRING, "valid range expected");
                            if(Range.Type==TokenType.STRING)
                            {
                                switch(Range.Value)
                                {
                                    case "Melee":
                                    {
                                        ranges.Add(global::Range.Melee);
                                        break;
                                    }
                                    case "Ranged":
                                    {
                                        ranges.Add(global::Range.Range);
                                        break;
                                    }
                                    case "Siege":
                                    {
                                        ranges.Add(global::Range.Siege);
                                        break;
                                    }
                                    default:
                                    {
                                        type=CardType.Lider;
                                        errors.Add(new CompilingError(Range.Location, ErrorCode.Invalid, String.Format("{0} Range Does not exists", Range)));
                                        break;
                                    }
                                }
                            }
                            if (!Check(TokenType.COMMA))
                            {
                                into_range = false;
                                break;
                            }
                            advance();
                        }
                        Consume(TokenType.RIGHT_BRACE, "] expected");
                        Consume(TokenType.COMMA, ", expected");
                        break;
                    }
                case TokenType.OnACTIVATION:
                    {
                        if (effects != null)
                        {
                            errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "OnActivation property has been declared"));
                        }
                        effects=new List<CallEffect>();
                        advance();
                        Consume(TokenType.LEFT_KEY, "{ expected");
                        effects.Add(ParseCallEffect());
                        int effects_count=1;
                        while(true)
                        {
                            if(peek().Type==TokenType.PostAction)
                            {
                                effects.Add(ParsePostAction(effects[effects_count-1].Selector));
                                effects_count++;
                            }
                            else if(peek().Type==TokenType.LEFT_KEY)
                            {
                                effects.Add(ParsePostCallEffect(effects[effects_count-1].Selector));
                                effects_count++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        Consume(TokenType.RIGHT_KEY, "} expected");
                        break;
                    }
                default:
                    {
                        Consume("card property expected");
                        break;
                    }
            }
        }
        #region much text
        Consume(TokenType.RIGHT_KEY, "} expected");
        if (name == null)
        {
            errors.Add(new CompilingError(location, ErrorCode.Expected, "card name expected"));
            name = "null";
        }
        if (faction == null)
        {
            errors.Add(new CompilingError(location, ErrorCode.Expected, "card faction expected"));
            faction = "null";
        }
        if (type == null)
        {
            errors.Add(new CompilingError(location, ErrorCode.Expected, "card type expected"));
            type = CardType.Lider;
        }
        #endregion
        return new Card(location, name, faction, (CardType)type, ranges, power, effects);
    }
    #region CardProperties
    string ParseFaction()
    {
        advance();
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.STRING, "valid faction expected");
        string faction = previus().Value;
        Consume(TokenType.COMMA, ", expected");
        return faction;
    }
    string ParseName()
    {
        advance();
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.STRING, "valid name expected");
        string name = previus().Value;
        Consume(TokenType.COMMA, ", expected");
        return name;
    }
    int ParsePower()
    {
        advance();
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.NUMBER, "valid power expected");
        int power;
        Int32.TryParse(previus().Value, out power);
        Consume(TokenType.COMMA, ", expected");
        return power;
    }
    CallEffect ParseCallEffect()
    {
        CodeLocation codeLocation = peek().Location;
        Consume(TokenType.effect, "keyword Effect expected");
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        if(peek().Type!=TokenType.STRING)
        {
            Consume(TokenType.NAME, "Name expected");
            Consume(TokenType.COLON, ": expected");
        }
        Token effect_name = Consume(TokenType.STRING, "valid string expected");
        Consume(TokenType.COMMA, ", expected");
        Dictionary<Token, Expression> arguments = new Dictionary<Token, Expression>();
        while (Check(TokenType.IDENTIFIER))
        {
            Token param = peek();
            current++;
            Consume(TokenType.COLON, ": expected");
            Expression expression = ParseExpression();
            arguments.Add(param, expression);
            Consume(TokenType.COMMA, ", expected");
        }
        Consume(TokenType.RIGHT_KEY, "} expected");
        return new CallEffect(codeLocation, arguments, effect_name.Value, ParseSelector());
    }
    CallEffect ParsePostCallEffect(Selector father)
    {
        CodeLocation codeLocation = peek().Location;
        advance();
        Consume(TokenType.effect, "keyword Effect expected");
        Consume(TokenType.COLON, ": expected");
        if(peek().Type!=TokenType.STRING)
        {
            Consume(TokenType.LEFT_KEY, "{ expected");
            Consume(TokenType.NAME, "Name expected");
            Consume(TokenType.COLON, ": expected");
        }
        Token effect_name = Consume(TokenType.STRING, "valid string expected");
        Consume(TokenType.COMMA, ", expected");
        Dictionary<Token, Expression> arguments = new Dictionary<Token, Expression>();
        while (Check(TokenType.IDENTIFIER))
        {
            Token param = peek();
            current++;
            Consume(TokenType.COLON, ": expected");
            Expression expression = ParseExpression();
            arguments.Add(param, expression);
            Consume(TokenType.COMMA, ", expected");
        }
        Consume(TokenType.RIGHT_KEY, "} expected");
        return new CallEffect(codeLocation, arguments, effect_name.Value, father);
    }
    Selector ParseSelector()
    {
        Token Keyword_Selector = Consume(TokenType.Selector, "selector keyword expected");
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        bool? single = null;
        SourceType? source = null;
        PredicateStmt? predicateStmt = null;
        while (!Check(TokenType.RIGHT_KEY))
        {
            switch (peek().Type)
            {
                case TokenType.Single:
                    advance();
                    Consume(TokenType.COLON,": expected");
                    if (single != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Single property has been declared"));
                    }
                    if (Check(TokenType.TRUE))
                    {
                        single = true;
                        advance();
                    }
                    else if (Check(TokenType.FALSE))
                    {
                        single = false;
                        advance();
                    }
                    else
                    {
                        Consume("bool expected");
                    }
                    Consume(TokenType.COMMA, ", expected");
                    break;
                case TokenType.Source:
                    advance();
                    if (source != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Source property has been declared"));
                    }
                    Consume(TokenType.COLON,": expected");
                    if (Check(TokenType.STRING))
                    {
                        Token String = advance();
                        switch (String.Value)
                        {
                            case "board":
                                source = SourceType.board;
                                break;
                            case "hand":
                                source = SourceType.hand;
                                break;
                            case "otherHand":
                                source = SourceType.otherHand;
                                break;
                            case "deck":
                                source = SourceType.deck;
                                break;
                            case "otherDeck":
                                source = SourceType.otherDeck;
                                break;
                            case "field":
                                source = SourceType.field;
                                break;
                            case "otherField":
                                source = SourceType.otherField;
                                break;
                            default:
                                errors.Add(new CompilingError(String.Location, ErrorCode.Expected, " valid source expected"));
                                break;
                        }
                    }
                    else
                    {
                        Consume("string expected");
                    }
                    Consume(TokenType.COMMA, ", expected");
                    break;
                case TokenType.Predicate:
                    advance();
                    if (predicateStmt != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Predicate property has been declared"));
                    }
                    Consume(TokenType.COLON, ": expected");
                    predicateStmt = ParsePredicate();
                    break;
                default:
                    Consume("Selector property expected");
                    break;

            }
        }
        Consume(TokenType.RIGHT_KEY, "} expected");
        if (single == null)
        {
            single = false;
        }
        if (source == null)
        {
            source = SourceType.board;//something
            errors.Add(new CompilingError(Keyword_Selector.Location, ErrorCode.Expected, "source in selector expected"));
        }
        if(predicateStmt==null)
        {
            predicateStmt=new PredicateStmt(Keyword_Selector.Location,new Atom("",peek().Location),ParamsPredicate.card);
            errors.Add(new CompilingError(Keyword_Selector.Location, ErrorCode.Expected, "predicate in selector expected"));
        }
        return new Selector(Keyword_Selector.Location, (bool)single, (SourceType)source, predicateStmt,null);
    }
    CallEffect ParsePostAction(Selector father)
    {
        CodeLocation codeLocation = peek().Location;
        advance();
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        Consume(TokenType.TYPE, "keyword Type expected");
        Consume(TokenType.COLON, ": expected");
        Token effect_name = Consume(TokenType.STRING, "valid string expected");
        Consume(TokenType.COMMA, ", expected");
        Dictionary<Token, Expression> arguments = new Dictionary<Token, Expression>();
        /*while (Check(TokenType.IDENTIFIER))
        {
            Token param = peek();
            current++;
            Consume(TokenType.COLON, ": expected");
            Expression expression = ParseExpression();
            arguments.Add(param, expression);
            Consume(TokenType.COMMA, ", expected");
        }
        */
        if(peek().Type==TokenType.Selector)
        {
            return new CallEffect(codeLocation, arguments, effect_name.Value, ParsePostSelector(father));
        }
        return new CallEffect(codeLocation, arguments, effect_name.Value, father);
    }
    Selector ParsePostSelector(Selector parent)
    {
        bool? single = null;
        SourceType? source = null;
        PredicateStmt? predicateStmt = null;
        Token Keyword_Selector = Consume(TokenType.Selector, "selector keyword expected");
        Consume(TokenType.COLON, ": expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        while (!Check(TokenType.RIGHT_KEY))
        {
            switch (peek().Type)
            {
                case TokenType.Single:
                    advance();
                    Consume(TokenType.COLON,": expected");
                    if (single != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Single property has been declared"));
                    }
                    if (Check(TokenType.TRUE))
                    {
                        single = true;
                        advance();
                    }
                    else if (Check(TokenType.FALSE))
                    {
                        single = false;
                        advance();
                    }
                    else
                    {
                        Consume("bool expected");
                    }
                    Consume(TokenType.COMMA, ", expected");
                    break;
                case TokenType.Source:
                    advance();
                    if (source != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Source property has been declared"));
                    }
                    Consume(TokenType.COLON,": expected");
                    if (Check(TokenType.STRING))
                    {
                        Token String = advance();
                        switch (String.Value)
                        {
                            case "board":
                                source = SourceType.board;
                                break;
                            case "hand":
                                source = SourceType.hand;
                                break;
                            case "otherHand":
                                source = SourceType.otherHand;
                                break;
                            case "deck":
                                source = SourceType.deck;
                                break;
                            case "otherDeck":
                                source = SourceType.otherDeck;
                                break;
                            case "field":
                                source = SourceType.field;
                                break;
                            case "otherField":
                                source = SourceType.otherField;
                                break;
                            case "parent":
                                source = SourceType.parent;
                                break;
                            default:
                                errors.Add(new CompilingError(String.Location, ErrorCode.Expected, " valid source expected"));
                                break;
                        }
                    }
                    else
                    {
                        Consume("string expected");
                    }
                    Consume(TokenType.COMMA, ", expected");
                    break;
                case TokenType.Predicate:
                    advance();
                    if (predicateStmt != null)
                    {
                        errors.Add(new CompilingError(peek().Location, ErrorCode.Invalid, "Predicate property has been declared"));
                    }
                    Consume(TokenType.COLON, ": expected");
                    predicateStmt = ParsePredicate();
                    break;
                default:
                    Consume("Selector property expected");
                    break;

            }
        }
        Consume(TokenType.RIGHT_KEY, "} expected");
        Consume(TokenType.RIGHT_KEY, "} expected");
        if (single == null)
        {
            single = false;
        }
        if (source == null)
        {
            source = SourceType.board;//something
            errors.Add(new CompilingError(Keyword_Selector.Location, ErrorCode.Expected, "source in selector expected"));
        }
        if(predicateStmt==null)
        {
            predicateStmt=new PredicateStmt(Keyword_Selector.Location,new Atom("",peek().Location),ParamsPredicate.card);
            errors.Add(new CompilingError(Keyword_Selector.Location, ErrorCode.Expected, "predicate in selector expected"));
        }
        return new Selector(Keyword_Selector.Location, (bool)single, (SourceType)source, predicateStmt,parent);
    }
    PredicateStmt ParsePredicate()
    {
        ParamsPredicate? paramsPredicate = null;
        Expression? expression = null;
        Token paren = Consume(TokenType.LEFT_PAREN, "( expected");
        while (!Check(TokenType.RIGHT_PAREN))
        {
            switch (peek().Type)
            {
                case TokenType.CARD:
                    paramsPredicate = ParamsPredicate.card;
                    break;
                case TokenType.BOOST:
                    paramsPredicate = ParamsPredicate.boost;
                    break;
                case TokenType.WEATHER:

                    paramsPredicate = ParamsPredicate.weather;
                    break;
                case TokenType.UNIT:
                    paramsPredicate = ParamsPredicate.unit;
                    break;
                default:
                    Consume("expected predicate param unit, card, boost or weather");
                    break;
            }
            advance();
        }
        Consume(TokenType.RIGHT_PAREN, ") expected");
        Consume(TokenType.LAMBDA, "=> expected");
        expression = ParseExpression();
        Consume(TokenType.COMMA, ", expected");
        if (expression == null)
        {
            expression = new Atom("", new CodeLocation());//something
            errors.Add(new CompilingError(paren.Location, ErrorCode.Expected, "expression in lambda expected"));
        }
        if (paramsPredicate == null)
        {
            paramsPredicate = ParamsPredicate.unit;//something
        }
        return new PredicateStmt(paren.Location, expression, (ParamsPredicate)paramsPredicate);
    }
    #endregion
    #region Statements AST
    Stmt ParseStmt()
    {
        if (match(TokenType.WHILE))
        {
            return ParseWhileStmt(peek().Location);
        }
        if (match(TokenType.FOR))
        {
            return ParseForStmt(peek().Location);
        }
        if (match(TokenType.SEMICOLON))
        {
            return new EmptyStmt(peek().Location);
        }
        return ParseExpressionStmt(peek().Location);
    }
    Stmt ParseExpressionStmt(CodeLocation location)
    {
        Expression expr = ParseExpression();
        Consume(TokenType.SEMICOLON, "; expected after expression");
        return new ExpressionStmt(expr, location);
    }
    Stmt ParseWhileStmt(CodeLocation location)
    {
        Consume(TokenType.LEFT_PAREN, "( expected");
        Expression condition = ParseExpression();
        Consume(TokenType.RIGHT_PAREN, ") expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        List<Stmt> body = new List<Stmt>();
        while (peek().Type != TokenType.RIGHT_KEY)
        {
            body.Add(ParseStmt());
        }
        current++;
        return new WhileStmt(condition, body, location);
    }
    Stmt ParseForStmt(CodeLocation location)
    {
        Token identifier=Consume(TokenType.IDENTIFIER, "identifier expected");
        Consume(TokenType.IN, "in keyword expected");
        Token pack=Consume(TokenType.IDENTIFIER, "identifier expected");
        Consume(TokenType.LEFT_KEY, "{ expected");
        List<Stmt> body = new List<Stmt>();
        while (peek().Type != TokenType.RIGHT_KEY)
        {
            body.Add(ParseStmt());
        }
        advance();
        return new ForStmt(identifier,pack,body,location);
    }
    Effect ParseEffect()
    {
        CodeLocation codeLocation = peek().Location;
        string? name = null;
        Dictionary<Token,DataType> Params = new Dictionary<Token, DataType>();
        List<Stmt>? body = null;
        Consume(TokenType.LEFT_KEY, "{ expected");
        while (!Check(TokenType.RIGHT_KEY))
        {
            switch (peek().Type)
            {
                case TokenType.NAME:
                    {
                        advance();
                        Consume(TokenType.COLON, ": expected");
                        Consume(TokenType.STRING, "valid name expected");
                        name = previus().Value;
                        Consume(TokenType.COMMA, ", expected");
                        break;
                    }
                case TokenType.ACTION:
                    {
                        body = new List<Stmt>();
                        advance();
                        Consume(TokenType.COLON, ": expected at Action declaration");
                        Consume(TokenType.LEFT_PAREN, "( expected at Action declaration");
                        Token ActionArg=Consume("expected context or targets argument",TokenType.IDENTIFIER,"context","targets");
                        switch(ActionArg.Value)
                        {
                            case "context":
                            {
                                Consume(TokenType.COMMA, ", expected");
                                Consume("expected targets argument",TokenType.IDENTIFIER,"targets");
                                break;
                            }
                            case "targets":
                            {
                                Consume(TokenType.COMMA, ", expected");
                                Consume("expected context argument",TokenType.IDENTIFIER,"context");
                                break;
                            }
                        }
                        Consume(TokenType.RIGHT_PAREN, ") expected after param");
                        Consume(TokenType.LAMBDA, "=> expected after params");
                        Consume(TokenType.LEFT_KEY, "{ expected after =>");
                        while (!match(TokenType.RIGHT_KEY))
                        {
                            body.Add(ParseStmt());
                        }
                        break;
                    }
                case TokenType.PARAMS:
                    {
                        advance();
                        Consume(TokenType.COLON, ": expected");
                        Consume(TokenType.LEFT_KEY, "{ expected");
                        do
                        {
                            if (!match(TokenType.RIGHT_KEY))
                            {
                                Token identifier=Consume(TokenType.IDENTIFIER, "Identifier in param expected");
                                Params.Add(identifier,DataType.Null);
                                if(!Check(TokenType.COMMA)&&!Check(TokenType.RIGHT_KEY))
                                {
                                    Consume(TokenType.COLON,": expected after param identifier");
                                    Token Type=Consume("type expected after param identifier",TokenType.NumberType,TokenType.BoolType,TokenType.StringType);
                                    switch(Type.Type)
                                    {
                                        case TokenType.NumberType:
                                        Params[identifier]=DataType.Number;
                                        break;
                                        case TokenType.StringType:
                                        Params[identifier]=DataType.String;
                                        break;
                                        case TokenType.BoolType:
                                        Params[identifier]=DataType.Bool;
                                        break;
                                    }
                                }
                            }
                        }
                        while (match(TokenType.COMMA));
                        Consume(TokenType.RIGHT_KEY, "} expected");
                        break;
                    }
            }
        }
        Consume(TokenType.RIGHT_KEY, "} expected");
        if (name == null)
        {
            errors.Add(new CompilingError(codeLocation, ErrorCode.Expected, "effect name expected"));
            name = "null";
        }
        if (body == null)
        {
            errors.Add(new CompilingError(codeLocation, ErrorCode.Expected, "effect Action expected"));
            body = new List<Stmt>(); ;
        }
        return new Effect(name, Params, body, codeLocation);
    }
    #endregion
    #region Expression AST
    Expression ParseExpression()
    {
        CodeLocation location = peek().Location;
        return ParseAssign(location);
    }
    Expression ParseAssign(CodeLocation Location)
    {
        Expression expr = ParseOr(Location);
        if (match(TokenType.EQUAL))
        {
            Expression right = ParseOr(Location);
            if (expr.GetType() == typeof(Variable))
            {
                return new AssignExpression(Location, ((Variable)expr), right);
            }
            if (expr.GetType() == typeof(GetExpression))
            {
                GetExpression Get = (GetExpression)expr;
                return new SetExpression(Location, Get, right);
            }
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Invalid assignment target"));
        }
        return expr;
    }
    Expression ParseOr(CodeLocation Location)
    {
        Expression expr = ParseAnd(Location);
        while (match(TokenType.OR))
        {
            Token Operator = previus();
            Expression right = ParseAnd(Location);
            expr = new BinaryExpression(Location, expr, Operator, right);
        }
        return expr;
    }
    Expression ParseAnd(CodeLocation Location)
    {
        Expression expr = ParseEquality(Location);
        while (match(TokenType.AND))
        {
            Token Operator = previus();
            Expression right = ParseEquality(Location);
            expr = new BinaryExpression(Location, expr, Operator, right);
        }
        return expr;
    }
    Expression ParseEquality(CodeLocation Location)
    {
        Expression expr = ParseComparisson(Location);
        if (match(TokenType.EQUAL_EQUAL))
        {
            Token tokenOperator = previus();
            Expression right = ParseComparisson(Location);
            expr = new BinaryExpression(Location, expr, tokenOperator, right);
        }
        return expr;
    }
    Expression ParseComparisson(CodeLocation Location)
    {
        Expression expr = ParseTerm(Location);
        if (match(TokenType.GREATER) || match(TokenType.GREATER_EQUAL) || match(TokenType.LESS_EQUAL) || match(TokenType.LESS))
        {
            Token tokenOperator = previus();
            Expression right = ParseTerm(Location);
            expr = new BinaryExpression(Location, expr, tokenOperator, right);
        }
        return expr;
    }
    Expression ParseTerm(CodeLocation Location)
    {
        Expression expr = ParseFactor(Location);
        if (match(TokenType.PLUS) || match(TokenType.MINUS)||match(TokenType.ARROBA)||match(TokenType.ARROBA_ARROBA))
        {
            Token tokenOperator = previus();
            Expression right = ParseFactor(Location);
            expr = new BinaryExpression(Location, expr, tokenOperator, right);
        }
        if (match(TokenType.PLUS_PLUS))
        {
            if (expr.GetType() == typeof(Variable))
            {
                Expression right = new BinaryExpression(Location,expr,new Token(TokenType.PLUS,"+",previus().Location),new Atom((double)1,new CodeLocation()));
                return new AssignExpression(Location, ((Variable)expr), right);
            }
            if (expr.GetType() == typeof(GetExpression))
            {
                Expression right = new BinaryExpression(Location,expr,new Token(TokenType.PLUS,"+",previus().Location),new Atom((double)1,new CodeLocation()));
                GetExpression Get = (GetExpression)expr;
                return new SetExpression(Location, Get, right);
            }
            errors.Add(new CompilingError(Location, ErrorCode.Invalid, "Invalid assignment target"));
        }
        return expr;
    }
    Expression ParseFactor(CodeLocation Location)
    {
        Expression expr = ParsePow(Location);
        if (match(TokenType.STAR) || match(TokenType.SLASH))
        {
            Token tokenOperator = previus();
            Expression right = ParsePow(Location);
            expr = new BinaryExpression(Location, expr, tokenOperator, right);
        }
        return expr;
    }
    Expression ParsePow(CodeLocation Location)
    {
        Expression expr = ParseUnary(Location);
        if (match(TokenType.Caret))
        {
            Token tokenOperator = previus();
            Expression right = ParseUnary(Location);
            expr = new BinaryExpression(Location, expr, tokenOperator, right);
        }
        return expr;
    }
    Expression ParseUnary(CodeLocation Location)
    {
        if (match(TokenType.MINUS))
        {
            Token tokenOperator = previus();
            Expression right = ParseUnary(Location);
            return new Unary(right, tokenOperator, Location);
        }
        return ParseCall(Location); ;
    }
    Expression ParseCall(CodeLocation Location)
    {
        Expression expr = ParseAtom(Location);
        while (match(TokenType.DOT))
        {
            Token name = Consume( "expected property name",TokenType.IDENTIFIER,TokenType.NAME,TokenType.FACTION,TokenType.POWER);
            if (match(TokenType.LEFT_PAREN))
            {
                List<Expression> arguments = new List<Expression>();
                if (!Check(TokenType.RIGHT_PAREN))
                {
                    do
                    {
                        arguments.Add(ParseExpression());
                    }
                    while (match(TokenType.COMMA));
                }
                Consume(TokenType.RIGHT_PAREN, ") expected");
                expr = new MetodExpression(Location, name, arguments, expr);
            }
            else
            {
                expr = new GetExpression(Location, name, expr);
            }
        }
        return expr;
    }
    Expression ParseAtom(CodeLocation Location)
    {
        if (match(TokenType.FALSE)) { return new Atom(false, Location); }
        if (match(TokenType.TRUE)) { return new Atom(true, Location); }
        if (match(TokenType.NUMBER)) { return new Atom(Double.Parse(previus().Value), Location); }
        if (match(TokenType.STRING)) { return new Atom(previus().Value, Location); }
        if (match(TokenType.LEFT_PAREN))
        {
            Expression expr = ParseExpression();
            Consume(TokenType.RIGHT_PAREN, ") expected after group expression");
            return new Group(expr, Location);
        }
        if (match(TokenType.IDENTIFIER))
        {
            return new Variable(Location, previus().Value);
        }
        if (match(TokenType.CARD)||match(TokenType.BOOST)||match(TokenType.UNIT)||match(TokenType.WEATHER))
        {
            return new Variable(Location, previus().Value);
        }
        return null;//inalcanzable
    }
    #endregion

}
