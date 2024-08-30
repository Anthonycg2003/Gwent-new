using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;

public class Lexer
{
    #region Literals
        const string card="card";
        const string effect="effect";
        const string two_points=":";
        const string comma=",";
        const string left_bracket="[";
        const string right_bracket="]";
        const string left_key="{";
        const string right_key="}";
        const string left_parent="(";
        const string right_parent=")";
        const string minus="-";
        const string semi_colon=";";
        const string slash="/";
        const string star="*";
        const string dot=".";
        const string Caret="^";
        const string type="Type";
        const string name="Name";
        const string Effect="Effect";
        const string faction="Faction";
        const string power="Power";
        const string range="Range";
        const string Activation="OnActivation";
        const string PARAMS="Params";
        const string ACTION="Action";
        const string EQUAL="=";
        const string EQUAL_EQUAL="==";
        const string GREATER=">";
        const string GREATER_EQUAL=">=";
        const string LESS="<";
        const string LESS_EQUAL="<=";
        const string ARROBA="@";
        const string ARROBA_ARROBA="@@";
        const string PLUS="+";
        const string PLUS_PLUS="++";
        const string AND="&&";
        const string OR="||";
        const string FALSE="false";
        const string TRUE="true";
        const string FOR="for";
        const string WHILE="while";
        const string LAMBDA="=>";
        const string NumberType="Number";
        const string StringType="String";
        const string BoolType="Bool";
        const string Source="Source";
        const string Selector="Selector";
        const string Single="Single";
        const string Predicate="Predicate";
        const string weather="weather";
        const string unit="unit";
        const string boost="boost";
        const string IN="in";
        const string PostAction="PostAction";
        #endregion
    public Lexer(string code) 
    {
        this.code=code;
        errors=new List<CompilingError>();
        Operators = new Dictionary<string, TokenType>()
        {
            {right_parent,TokenType.RIGHT_PAREN},{IN,TokenType.IN},{left_parent,TokenType.LEFT_PAREN}, {comma,TokenType.COMMA},{minus,TokenType.MINUS},{semi_colon,TokenType.SEMICOLON},{slash,TokenType.SLASH},{star,TokenType.STAR},{dot,TokenType.DOT},{two_points,TokenType.COLON},{right_key,TokenType.RIGHT_KEY},{left_key,TokenType.LEFT_KEY},{left_bracket,TokenType.LEFT_BRACE},{right_bracket,TokenType.RIGHT_BRACE},{Caret,TokenType.Caret},{LAMBDA,TokenType.LAMBDA},
            {EQUAL_EQUAL,TokenType.EQUAL_EQUAL},{EQUAL,TokenType.EQUAL},{GREATER_EQUAL,TokenType.GREATER_EQUAL},{GREATER,TokenType.GREATER},{LESS_EQUAL,TokenType.LESS_EQUAL},{LESS,TokenType.LESS},{ARROBA_ARROBA,TokenType.ARROBA_ARROBA},{ARROBA,TokenType.ARROBA},{PLUS_PLUS,TokenType.PLUS_PLUS},{PLUS,TokenType.PLUS},
        };
        KeyWords = new Dictionary<string, TokenType>()
        {
            {card,TokenType.CARD},{PostAction,TokenType.PostAction},{effect,TokenType.EFFECT},{faction,TokenType.FACTION},{power,TokenType.POWER},{range,TokenType.RANGE},{type,TokenType.TYPE},{Activation,TokenType.OnACTIVATION},{name,TokenType.NAME},{PARAMS,TokenType.PARAMS},{AND,TokenType.AND},{OR,TokenType.OR},{FOR,TokenType.FOR},{FALSE,TokenType.FALSE},{TRUE,TokenType.TRUE},{WHILE,TokenType.WHILE},{ACTION,TokenType.ACTION},{Effect,TokenType.effect},{NumberType,TokenType.NumberType},{StringType,TokenType.StringType},{BoolType,TokenType.BoolType},{Selector,TokenType.Selector},{Single,TokenType.Single},{Predicate,TokenType.Predicate},{Source,TokenType.Source},
            {weather,TokenType.WEATHER},{unit,TokenType.UNIT},{boost,TokenType.BOOST},
        };
    }

    public Dictionary<string, TokenType> Operators { get; set; }
    string code;
    public Dictionary<string, TokenType> KeyWords { get; set; }
    public List<CompilingError> errors;
    public List<string> Get_errors()
    {
        List<string> Errors=new List<string>();
        foreach (CompilingError error in errors)
        {
            Errors.Add(error.Code + "=> " + error.Argument + " at line " + error.location.line + " and column " + error.location.column);
        }
        return Errors;
    }

    private bool MatchSymbol(TokenReader stream, List<Token> tokens)
    {
        foreach (var op in Operators.Keys.OrderByDescending(k => k.Length))
        {
            if (stream.Match(op))
            {
                tokens.Add(new Token(Operators[op], op, stream.Location));
                return true;
            }
        }
        return false;
    }

    private bool MatchText(TokenReader stream, List<Token> tokens, List<CompilingError> errors)
    {
            string text;
            if (stream.Match("\""))
            {
                if (!stream.ReadUntil("\"", out text))
                    errors.Add(new CompilingError(stream.Location, ErrorCode.Expected, "\""));
                tokens.Add(new Token(TokenType.STRING, text, stream.Location));
                return true;
            }
        return false;
    }
    

    public List<Token> GetTokens()
    {
        List<Token> tokens = new List<Token>();

        TokenReader stream = new TokenReader(code);

        while (!stream.EOF)
        {
            string value;

            if (stream.ReadWhiteSpace())
                continue;
            if (MatchSymbol(stream, tokens))
                continue;
            if (stream.ReadID(out value))
            {
                if (KeyWords.ContainsKey(value))
                    tokens.Add(new Token(KeyWords[value], value, stream.Location));
                else
                    tokens.Add(new Token(TokenType.IDENTIFIER, value, stream.Location));
                continue;
            }
            

            if (stream.ReadNumber(out value))
            {
                double d;
                if (!double.TryParse(value, out d))
                    errors.Add(new CompilingError(stream.Location, ErrorCode.Invalid, "Number format"));
                tokens.Add(new Token(TokenType.NUMBER, value, stream.Location));
                continue;
            }

            if (MatchText(stream, tokens, errors))
                continue;

            

            var unkOp = stream.ReadAny();
            errors.Add(new CompilingError(stream.Location, ErrorCode.Unknown, unkOp.ToString()));
        }
        tokens.Add(new Token(TokenType.EOF,"null",stream.Location));
        return tokens;
    }

    class TokenReader
    {
        readonly string code;
        int pos;
        int line;
        int lastLB;

        public TokenReader(string code)
        {
            this.code = code;
            this.pos = 0;
            this.line = 1;
            this.lastLB = -1;
        }

        public bool EOF => (pos >= code.Length);

        public bool EOL => (EOF || code[pos] == '\n');

        public CodeLocation Location
        {
            get
            {
                return new CodeLocation
                {
                    line = line,
                    column = pos - lastLB
                };
            }
        }

        public char Peek()
        {
            if (pos < 0 || pos >= code.Length) throw new InvalidOperationException();

            return code[pos];
        }

        public bool ContinuesWith(string prefix)
        {
            if (pos + prefix.Length > code.Length)
                return false;
            for (int i = 0; i < prefix.Length; i++)
                if (code[pos + i] != prefix[i])
                    return false;
            return true;
        }

        public bool Match(string prefix)
        {
            if (ContinuesWith(prefix))
            {
                pos += prefix.Length;
                return true;
            }

            return false;
        }

        public bool ValidIdCharacter(char c, bool begining)
        {
            return c == '_' || (begining ? char.IsLetter(c) : char.IsLetterOrDigit(c));
        }

        public bool ReadID(out string id)
        {
            id = "";
            while (!EOL && ValidIdCharacter(Peek(), id.Length == 0))
                id += ReadAny();
            return id.Length > 0;
        }

        public bool ReadNumber(out string number)
        {
            number = "";
            while (!EOL && char.IsDigit(Peek()))
                number += ReadAny();
            if (!EOL && Match("."))
            {
                number += '.';
                while (!EOL && char.IsDigit(Peek()))
                    number += ReadAny();
            }

            return number.Length > 0;
        }

        public bool ReadUntil(string end, out string text)
        {
            text = "";
            while (!Match(end))
            {
                if (EOL || EOF)
                    return false;
                text += ReadAny();
            }
            return true;
        }

        public bool ReadWhiteSpace()
        {
            if (char.IsWhiteSpace(Peek()))
            {
                ReadAny();
                return true;
            }
            return false;
        }

        public char ReadAny()
        {
            if (EOF)
                throw new InvalidOperationException();

            if (EOL)
            {
                line++;
                lastLB = pos;
            }
            return code[pos++];
        }
    }
}
