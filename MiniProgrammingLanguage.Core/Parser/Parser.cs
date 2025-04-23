using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Lexer;
using MiniProgrammingLanguage.Core.Lexer.Enums;
using MiniProgrammingLanguage.Core.Lexer.Extensions;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;
using ValueType = MiniProgrammingLanguage.Core.Interpreter.Values.Enums.ValueType;

namespace MiniProgrammingLanguage.Core.Parser;

public class Parser
{
    public Parser(IReadOnlyList<Token> tokens, string filepath, ParserConfiguration configuration)
    {
        Tokens = tokens;
        Configuration = configuration;
        Filepath = filepath;
    }

    public IReadOnlyList<Token> Tokens { get; }

    public Token Current => Tokens[Position];

    public bool IsNotEnded => Position < Tokens.Count;
    
    public string Filepath { get; }

    public int Position { get; private set; }
    
    public ParserConfiguration Configuration { get; }

    private FunctionBodyExpression _root;

    public FunctionBodyExpression Parse()
    {
        var statements = new List<IStatement>();

        while (IsNotEnded)
        {
            statements.Add(ParseStatement());
        }

        return new FunctionBodyExpression(statements.ToArray(), null, new Location
        {
            Line = 0,
            Position = 0,
            Filepath = Filepath
        });
    }

    private IStatement ParseStatement()
    {
        if (Match(TokenType.Await))
        {
            return ParseAwait();
        }

        if (Match(TokenType.Import))
        {
            return ParseImport();
        }

        if (Match(TokenType.Enum))
        {
            return ParseEnumDeclaration();
        }

        if (Match(TokenType.Break))
        {
            return new BreakExpression(Current.Location);
        }

        if (Match(TokenType.While))
        {
            return ParseWhile();
        }

        if (Match(TokenType.Implement))
        {
            return ParseImplementFunctionDeclaration();
        }

        if (Match(TokenType.Function))
        {
            return ParseFunctionDeclaration();
        }

        if (Match(TokenType.Return))
        {
            return ParseReturn();
        }
        
        if (Match(TokenType.If))
        {
            return ParseIf();
        }

        if (Match(TokenType.For))
        {
            return ParseForOrForeach();
        }

        if (Match(TokenType.Module))
        {
            return ParseModule();
        }
        
        if (Match(TokenType.Async))
        {
            return ParseFunctionDeclaration(true);
        }

        if (Match(TokenType.Type))
        {
            return ParseType();
        }

        if (Is(TokenType.Word))
        {
            if (IsWithOffset(1, TokenType.Equals, TokenType.Colon))
            {
                return ParseAssign();
            }
            
            if (IsWithOffset(1, TokenType.LeftParentheses))
            {
                return ParseFunctionCall();
            }
            
            for (var i = Position; i < Tokens.Count - 1; i++)
            {
                var position = i - Position;
                
                if (IsWithOffset(position, TokenType.Dot, TokenType.Word))
                {
                    continue;
                }

                if (IsWithOffset(position, TokenType.Equals))
                {
                    return ParseTypeMemberAssign();
                }

                if (IsWithOffset(position, TokenType.LeftParentheses))
                {
                    var binary = ParseBinary();

                    if (binary is not DotExpression dotExpression)
                    {
                        break;
                    }

                    return dotExpression;
                }
                
                break;
            }
        }
        
        ParserThrowHelper.ThrowStatementExceptedException(Current.Location);
        
        return null;
    }

    private EnumDeclarationExpression ParseEnumDeclaration()
    {
        Match(TokenType.Enum);

        var name = Current;

        MatchOrException(TokenType.Word);

        var members = new Dictionary<string, int>();

        var isEnded = false;
        
        while (IsNotEnded)
        {
            if (Match(TokenType.End))
            {
                isEnded = true;
                
                break;
            }

            var memberName = Current;

            MatchOrException(TokenType.Word).
                MatchOrException(TokenType.Equals);

            var value = ParseUnary();

            if (value is not RoundNumberExpression roundNumberExpression)
            {
                ParserThrowHelper.ThrowTokenExpectedException(Configuration, value.Location, TokenType.Number);

                return null;
            }
            
            members.Add(memberName.Value, roundNumberExpression.Value);
        }

        if (!isEnded)
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, name.Location, TokenType.End);
        }

        return new EnumDeclarationExpression(name.Value, members, _root, name.Location);
    }

    private WhileExpression ParseWhile()
    {
        Match(TokenType.While);

        var isPars = Match(TokenType.LeftParentheses);

        var condition = ParseBinary();

        if (isPars)
        {
            MatchOrException(TokenType.RightParentheses);
        }

        var body = ParseFunctionBody();

        return new WhileExpression(condition, body, condition.Location);
    }
    
    private AwaitExpression ParseAwait()
    {
        Match(TokenType.Await);
        
        var functionCall = ParseFunctionCall();
        
        return new AwaitExpression(functionCall, functionCall.Location);
    }
    
    private AbstractEvaluableExpression ParseUnary()
    {
        if (Match(TokenType.Plus))
        {
            return ParseValue();
        }
        
        if (Match(TokenType.Minus))
        {
            return new UnaryExpression(BinaryOperatorType.Minus, ParseValue(), Current.Location);
        }

        return ParseValue();
    }
    
    private AbstractEvaluableExpression ParseMultiplicative()
    {
        var left = ParseUnary();
        
        while (IsNotEnded)
        {
            if (Match(TokenType.Multiplication))
            {
                left = new BinaryExpression(BinaryOperatorType.Multiplication, left, ParseUnary(), left.Location);
                
                continue;
            }
            
            if (Match(TokenType.Division))
            {
                left = new BinaryExpression(BinaryOperatorType.Division, left, ParseUnary(), left.Location);
                
                continue;
            }
            
            break;
        }
        
        return left;
    }

    private AbstractEvaluableExpression ParseBinary(AbstractEvaluableExpression last = null)
    {
        var left = ParseMultiplicative();

        while (IsNotEnded)
        {
            if (Match(TokenType.And))
            {
                left = new BinaryExpression(BinaryOperatorType.And, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Or))
            {
                left = new BinaryExpression(BinaryOperatorType.Or, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Dot))
            {
                var right = ParseBinary(left);
                
                switch (right)
                {
                    case AssignTypeMemberExpression when last is null:
                        return right;
                    case AssignTypeMemberExpression assignTypeMemberExpression:
                    {
                        var parent = new DotExpression(last,
                            assignTypeMemberExpression.Left.Left, right.Location);
                        var member = new DotExpression(parent,
                            assignTypeMemberExpression.Left.Right, right.Location);
                        right = new AssignTypeMemberExpression(member, assignTypeMemberExpression.Right,
                            right.Location);
                        return right;
                    }
                    case DotExpression dotExpression:
                        return new DotExpression(new DotExpression(left, dotExpression.Left, left.Location),
                            dotExpression.Right, dotExpression.Location);
                    default:
                        return new DotExpression(left, right, left.Location);
                }
            }

            if (Match(TokenType.Plus))
            {
                left = new BinaryExpression(BinaryOperatorType.Plus, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Minus))
            {
                left = new BinaryExpression(BinaryOperatorType.Minus, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Is))
            {
                left = new BinaryExpression(BinaryOperatorType.Equals, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Greater))
            {
                left = new BinaryExpression(BinaryOperatorType.Greater, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Less))
            {
                left = new BinaryExpression(BinaryOperatorType.Less, left, ParseMultiplicative(), left.Location);
                continue;
            }

            if (Match(TokenType.Equals))
            {
                if (IsWithOffset(-3, TokenType.Dot))
                    return new AssignTypeMemberExpression(
                        new DotExpression(last, left, left.Location),
                        ParseMultiplicative(),
                        left.Location
                    );

                if (left is VariableExpression variableExpression)
                    return new AssignExpression(variableExpression.Name, null, ParseMultiplicative(), _root,
                        variableExpression.Location);
            }

            break;
        }

        return left;
    }

    private LoopExpression ParseForOrForeach()
    {
        Match(TokenType.For);

        var isPars = Match(TokenType.LeftParentheses);

        if (!Is(TokenType.Word))
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, Current.Location, TokenType.Word);
        }

        var variable = IsWithOffset(1, TokenType.Equals) ? ParseAssign() : ParseValue();
            
        var name = variable switch
        {
            AssignExpression assignExpression => assignExpression.Name,
            VariableExpression variableExpression => variableExpression.Name,
            _ => null
        };

        if (name is null)
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, variable.Location, TokenType.Word);
        }

        MatchOrException(TokenType.In);

        var count = ParseValue();
        AbstractEvaluableExpression stepExpression = new NumberExpression(1, count.Location);

        if (Match(TokenType.Comma))
        {
            stepExpression = ParseValue();
        }

        if (isPars)
        {
            MatchOrException(TokenType.RightParentheses);
        }

        var body = ParseFunctionBody();
        
        //We need change the root of variable
        variable = variable switch
        {
            AssignExpression assignExpression => new AssignExpression(name, null, assignExpression.EvaluableExpression,
                assignExpression.Root, assignExpression.Location),
            VariableExpression => new VariableExpression(name, body, variable.Location),
            _ => variable
        };

        var condition = new BinaryExpression(BinaryOperatorType.Less, new VariableExpression(name, body, variable.Location), count, variable.Location);
        var step = new BinaryExpression(BinaryOperatorType.Plus, new VariableExpression(name, body, variable.Location), stepExpression, variable.Location);

        return new ForExpression(condition, variable, step, name, body, variable.Location);
    }

    private IfExpression ParseIf()
    {
        Match(TokenType.If);

        var isPars = Match(TokenType.LeftParentheses);

        var condition = ParseBinary();

        if (isPars)
        {
            MatchOrException(TokenType.RightParentheses);
        }
        
        MatchOrException(TokenType.Then);

        var body = ParseFunctionBody(TokenType.End, TokenType.Else);
        FunctionBodyExpression elseBody = null;

        if (IsWithOffset(-1, TokenType.Else))
        {
            elseBody = ParseFunctionBody();
        }

        return new IfExpression(condition, body, elseBody, condition.Location);
    }
    
    private ImplementFunctionDeclarationExpression ParseImplementFunctionDeclaration()
    {
        Match(TokenType.Implement);
        
        var isAsync = Match(TokenType.Async);

        MatchOrException(TokenType.Function);
        
        var type = Current;
        
        MatchOrException(TokenType.Word).
            MatchOrException(TokenType.Dot);
        
        var function = ParseFunctionDeclaration(isAsync);

        return new ImplementFunctionDeclarationExpression(type.Value, function, type.Location);
    }
    
    private FunctionDeclarationExpression ParseFunctionDeclaration(bool isAsync = false)
    {
        Match(TokenType.Function);

        var name = Current;

        if (!Match(TokenType.Word))
        {
            //if anonymous we do empty name
            name = new Token
            {
                Type = TokenType.Word,
                Value = string.Empty,
                Location = name.Location
            };
        }
        
        MatchOrException(TokenType.LeftParentheses);

        var arguments = ParseArguments();
        var returnType = Match(TokenType.Colon) ? ParseObjectType(true) : ObjectTypeValue.Any;
        
        var body = ParseFunctionBody();

        return new FunctionDeclarationExpression(name.Value, arguments, body, returnType, isAsync, _root, name.Location);
    }

    private TypeDeclarationExpression ParseType()
    {
        Match(TokenType.Type);

        var name = Current;

        MatchOrException(TokenType.Word);

        var members = new List<ITypeMemberExpression>();

        while (!Match(TokenType.End))
        {
            members.Add(ParseTypeMember(name.Value));
        }

        return new TypeDeclarationExpression(name.Value, members, _root, name.Location);
    }

    private ITypeMemberExpression ParseTypeMember(string parent)
    {
        if (Match(TokenType.Async))
        {
            MatchOrException(TokenType.Function);

            return ParseTypeFunctionMember(parent, true);
        }
        
        if (Match(TokenType.Function))
        {
            return ParseTypeFunctionMember(parent, false);
        }

        return ParseTypeKeyMember(parent);
    }
    
    private KeyTypeMemberExpression ParseTypeKeyMember(string parent)
    {
        var name = Current;

        MatchOrException(TokenType.Word);

        var type = ParseObjectType(true);

        return new KeyTypeMemberExpression(name.Value, parent, type, name.Location);
    }
    
    private TypeFunctionMemberExpression ParseTypeFunctionMember(string parent, bool isAsync)
    {
        Match(TokenType.Function);

        var name = Current;

        MatchOrException(TokenType.Word).
            MatchOrException(TokenType.LeftParentheses);

        var arguments = ParseArguments();

        ObjectTypeValue returnValue;

        if (Match(TokenType.Colon))
        {
            returnValue = ParseObjectType();
        }
        else
        {
            returnValue = ObjectTypeValue.Any;
        }

        return new TypeFunctionMemberExpression(parent, name.Value, arguments, returnValue, isAsync, name.Location);
    }

    private FunctionBodyExpression ParseFunctionBody(params TokenType[] endTokens)
    {
        if (!endTokens.Any())
        {
            endTokens = new[] { TokenType.End };
        } 
        
        var lastRoot = _root;
        var location = Current.Location;
        
        _root = new FunctionBodyExpression(null, _root, location);
        
        var statements = new List<IStatement>();

        while (!Match(endTokens))
        {
            statements.Add(ParseStatement());
        }

        _root.Statements = statements.ToArray();

        var currentRoot = _root;
        _root = lastRoot;

        return currentRoot;
    }
    
    private FunctionArgument[] ParseArguments(TokenType endToken = TokenType.RightParentheses)
    {
        Match(TokenType.LeftParentheses);
        
        var arguments = new List<FunctionArgument>();
        
        while (!Match(endToken))
        {
            arguments.Add(ParseArgument());
        }

        return arguments.ToArray();
    }

    private FunctionArgument ParseArgument()
    {
        var name = Current;

        MatchOrException(TokenType.Word);
        
        var objectType = ParseObjectType(true);
        AbstractValue defaultValue = null;

        if (Match(TokenType.Equals))
        {
            var expression = ParseBinary();

            if (!expression.IsValue)
            {
                ParserThrowHelper.ThrowTokenExpectedException(Configuration, expression.Location, TokenType.String, TokenType.Number, TokenType.True, TokenType.False);
            }

            defaultValue = expression.Evaluate(null);
        }

        if (!Is(0, TokenType.RightParentheses))
        {
            MatchOrException(TokenType.Comma);
        }

        return new FunctionArgument(name.Value, objectType, defaultValue);
    }

    private CastExpression ParseCast()
    {
        Match(TokenType.LeftParentheses);

        var type = ParseObjectType();

        MatchOrException(TokenType.RightParentheses);

        var value = ParseBinary();

        return new CastExpression(type, value, value.Location);
    }

    private FunctionCallExpression ParseFunctionCall()
    {
        var name = Current;

        MatchOrException(TokenType.Word)
            .MatchOrException(TokenType.LeftParentheses);

        var arguments = new List<AbstractEvaluableExpression>();

        while (!Match(TokenType.RightParentheses))
        {
            arguments.Add(ParseBinary());
        }

        return new FunctionCallExpression(name.Value, arguments.ToArray(), _root, name.Location);
    }

    private AssignExpression ParseAssign()
    {
        var name = Current;

        MatchOrException(TokenType.Word);

        ObjectTypeValue objectTypeValue = null;
        
        if (Match(TokenType.Colon))
        {
            objectTypeValue = ParseObjectType();
        }
        else
        {
            objectTypeValue = ObjectTypeValue.Any;
        }

        MatchOrException(TokenType.Equals);
        
        var value = ParseBinary();

        return new AssignExpression(name.Value, objectTypeValue, value, _root, name.Location);
    }
    
    private AssignTypeMemberExpression ParseTypeMemberAssign()
    {
        var expression = ParseBinary();

        if (expression is not AssignTypeMemberExpression assignTypeMember)
        {
            ParserThrowHelper.ThrowStatementExceptedException(expression.Location);

            return null;
        }

        return new AssignTypeMemberExpression(assignTypeMember.Left, assignTypeMember.Right, expression.Location);
    }

    private ReturnExpression ParseReturn()
    {
        Match(TokenType.Return);

        var value = ParseBinary();

        return new ReturnExpression(value, value.Location);
    }

    private ModuleExpression ParseModule()
    {
        Match(TokenType.Module);

        var value = ParseValue();

        if (value is not StringExpression stringExpression)
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, value.Location, TokenType.String);

            return null;
        }

        return new ModuleExpression(stringExpression, value.Location);
    }

    private ArrayExpression ParseArray()
    {
        Match(TokenType.LeftSquareBracket);

        var current = Current;
        var values = new List<AbstractEvaluableExpression>();

        while (IsNotEnded)
        {
            values.Add(ParseBinary());

            if (Match(TokenType.RightSquareBracket))
            {
                break;
            }
            
            MatchOrException(TokenType.Comma);
        }

        return new ArrayExpression(values.ToArray(), current.Location);
    }

    private ImportExpression ParseImport()
    {
        Match(TokenType.Import);

        var name = ParseBinary();

        return new ImportExpression(name, name.Location);
    }

    private CreateExpression ParseCreate()
    {
        Match(TokenType.Create);

        var name = Current;

        MatchOrException(TokenType.Word);

        return new CreateExpression(name.Value, _root, name.Location);
    }

    private AbstractEvaluableExpression ParseValue()
    {
        var current = Current;

        if (Match(TokenType.LeftParentheses))
        {
            if (Current.Type.IsTypeToken() && IsWithOffset(1, TokenType.RightParentheses))
            {
                return ParseCast();
            }

            var result = ParseBinary();

            MatchOrException(TokenType.RightParentheses);
                
            return result;
        }

        if (Match(TokenType.Async))
        {
            MatchOrException(TokenType.Function);
            
            return new FunctionExpression(ParseFunctionDeclaration(true), current.Location);
        }
        
        if (Match(TokenType.Function))
        {
            return new FunctionExpression(ParseFunctionDeclaration(), current.Location);
        }
        
        if (Match(TokenType.String))
        {
            return new StringExpression(current.Value, current.Location);
        }
        
        if (Match(TokenType.Number))
        {
            if (current.Value.Contains(','))
            {
                if (!float.TryParse(current.Value, out var value))
                {
                    ParserThrowHelper.ThrowInvalidNumberFormatException(current.Value, current.Location);
                }
                
                return new NumberExpression(value, current.Location);
            }
            
            if (!int.TryParse(current.Value, out var roundValue))
            {
                ParserThrowHelper.ThrowInvalidNumberFormatException(current.Value, current.Location);
            }
            
            return new RoundNumberExpression(roundValue, current.Location);
        }

        if (Match(TokenType.Word))
        {
            if (Is(TokenType.LeftParentheses))
            {
                Position--;
                
                return ParseFunctionCall();
            }
            
            return new VariableExpression(current.Value, _root, current.Location);
        }

        if (Match(TokenType.LeftSquareBracket))
        {
            return ParseArray();
        }

        if (Match(TokenType.True))
        {
            return new BooleanExpression(true, current.Location);
        }
        
        if (Match(TokenType.False))
        {
            return new BooleanExpression(false, current.Location);
        }

        if (Match(TokenType.NoneValue))
        {
            return new NoneExpression(Current.Location);
        }

        if (Match(TokenType.Create))
        {
            return ParseCreate();
        }
        
        ParserThrowHelper.ThrowValueExceptedException(current.Location);

        return null;
    }

    private ObjectTypeValue ParseObjectType(bool anyIfNull = false)
    {
        Match(TokenType.Colon);
        
        var current = Current;

        if (Match(TokenType.StringType))
        {
            return ObjectTypeValue.String;
        }
        
        if (Match(TokenType.BooleanType))
        {
            return ObjectTypeValue.Boolean;
        }
        
        if (Match(TokenType.NumberType))
        {
            return ObjectTypeValue.Number;
        }
        
        if (Match(TokenType.RoundNumberType))
        {
            return ObjectTypeValue.RoundNumber;
        }
        
        if (Match(TokenType.EnumMember))
        {
            var enumName = Current;

            if (!Match(TokenType.Word))
            {
                return new ObjectTypeValue(string.Empty, ValueType.EnumMember);
            }
            
            return new ObjectTypeValue(enumName.Value, ValueType.EnumMember);
        }
        
        if (Match(TokenType.Enum))
        {
            var enumName = Current;

            if (!Match(TokenType.Word))
            {
                return new ObjectTypeValue(string.Empty, ValueType.Enum);
            }
            
            return new ObjectTypeValue(enumName.Value, ValueType.Enum);
        }
        
        if (Match(TokenType.Word))
        {
            return new ObjectTypeValue(current.Value, ValueType.Type);
        }

        if (anyIfNull)
        {
            return ObjectTypeValue.Any;
        }
        
        ParserThrowHelper.ThrowTypeExceptedException(current.Location);

        return null;
    }

    private bool Match(params TokenType[] tokenTypes)
    {
        if (tokenTypes.Contains(TokenType.None))
        {
            return true;
        }
        
        if (!Is(tokenTypes))
        {
            return false;
        }
        
        Skip();
        return true;
    }
    
    private Parser MatchOrException(params TokenType[] tokenTypes)
    {
        if (!Is(tokenTypes))
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, Current.Location, tokenTypes);
        }
        
        Skip();

        return this;
    }

    private bool Is(params TokenType[] tokenTypes) => tokenTypes.Contains(Current.Type);
    
    private bool IsWithOffset(int offset, params TokenType[] tokenTypes) => tokenTypes.Contains(Tokens[Position + offset].Type);

    private void Skip()
    {
        Position++;
    }
}