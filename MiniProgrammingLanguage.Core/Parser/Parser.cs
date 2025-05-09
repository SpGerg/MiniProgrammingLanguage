using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Create instance of parser
    /// </summary>
    /// <param name="tokens">Tokenized tokens</param>
    /// <param name="filepath">Uses in errors</param>
    /// <param name="configuration">Keywords</param>
    public Parser(IReadOnlyList<Token> tokens, string filepath, ParserConfiguration configuration)
    {
        Tokens = tokens;
        Configuration = configuration;
        Filepath = filepath;
    }

    /// <summary>
    /// Tokens
    /// </summary>
    public IReadOnlyList<Token> Tokens { get; }

    /// <summary>
    /// Current token.
    /// If position more than tokens counts throw an exception.
    /// </summary>
    public Token Current
    {
        get
        {
            if (Position > Tokens.Count)
            {
                var location = Tokens.Count is 0 ? Location.Default : Tokens[Tokens.Count - 1].Location;

                ParserThrowHelper.ThrowTokenExpectedException(Configuration, location);
            }

            return Tokens[Position];
        }
    }

    /// <summary>
    /// Is not ended
    /// </summary>
    public bool IsNotEnded => Position < Tokens.Count;

    /// <summary>
    /// Is ended
    /// </summary>
    public bool IsEnded => !IsNotEnded;

    /// <summary>
    /// Filepath to script
    /// </summary>
    public string Filepath { get; }

    /// <summary>
    /// Current position
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// Configuration
    /// </summary>
    public ParserConfiguration Configuration { get; }

    private FunctionBodyExpression _root;

    /// <summary>
    /// Parse given tokens into AST
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse statement
    /// </summary>
    /// <returns></returns>
    private IStatement ParseStatement()
    {
        var access = ParseAccess();

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
            return ParseEnumDeclaration(access);
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
            return ParseFunctionDeclaration(access);
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

        if (Match(TokenType.Try))
        {
            return ParseTryCatch();
        }

        if (Match(TokenType.Module))
        {
            return ParseModule();
        }

        if (Match(TokenType.Async))
        {
            return ParseFunctionDeclaration(access, true);
        }

        if (Match(TokenType.Type))
        {
            return ParseType(access);
        }

        if (Is(TokenType.Word))
        {
            if (IsWithOffset(1, TokenType.LeftSquareBracket))
            {
                return ParseArrayAssign();
            }
            
            if (IsWithOffset(1, TokenType.Equals, TokenType.Colon))
            {
                return ParseAssign(access);
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

    private TryCatchExpression ParseTryCatch()
    {
        Match(TokenType.Try);

        var tryBody = ParseFunctionBody(TokenType.Catch);
        var catchBody = ParseFunctionBody();

        return new TryCatchExpression(tryBody, catchBody, tryBody.Location);
    }

    /// <summary>
    /// Parse call expression.
    /// Call function without store it in a variable.
    /// <code>
    /// call (function declaration)
    /// </code>
    /// <code>
    /// return call function()
    ///     return ...
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    private CallExpression ParseCall()
    {
        Match(TokenType.Call);

        var isAsync = Match(TokenType.Async);

        return new CallExpression(ParseFunctionDeclaration(AccessType.None, isAsync), Current.Location);
    }

    /// <summary>
    /// Parse enum declaration expression
    /// <code>
    /// enum (name)
    ///     [member] = [value]
    ///     ...
    /// end
    /// </code>
    /// </summary>
    /// <param name="access"></param>
    /// <returns></returns>
    private EnumDeclarationExpression ParseEnumDeclaration(AccessType access)
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

            MatchOrException(TokenType.Word).MatchOrException(TokenType.Equals);

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

        return new EnumDeclarationExpression(name.Value, members, _root, access, name.Location);
    }

    /// <summary>
    /// Parse while expression
    /// <code>
    /// while (condition)
    ///     [body]
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    private WhileExpression ParseWhile()
    {
        Match(TokenType.While);

        var condition = ParseBinary();

        var body = ParseFunctionBody();

        return new WhileExpression(condition, body, condition.Location);
    }

    /// <summary>
    /// Parse await expression
    /// <code>
    /// await (__task)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private AwaitExpression ParseAwait()
    {
        Match(TokenType.Await);

        var functionCall = ParseFunctionCall();

        return new AwaitExpression(functionCall, functionCall.Location);
    }

    /// <summary>
    /// Parse unary expression
    /// <code>
    /// [operator](value)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private AbstractEvaluableExpression ParseUnary()
    {
        if (Match(TokenType.Not))
        {
            return new NotExpression(ParseBinary(), Current.Location);
        }

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

    /// <summary>
    /// Parse multiplicative expression
    /// <code>
    /// (left) * (right)
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse binary expression: binary, and, or, dot, assign
    /// <code>
    /// (left) (operator) (right)
    /// </code>
    /// </summary>
    /// <param name="last"></param>
    /// <returns></returns>
    private AbstractEvaluableExpression ParseBinary(AbstractEvaluableExpression last = null)
    {
        var left = ParseMultiplicative();

        while (IsNotEnded)
        {
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
                            assignTypeMemberExpression.Left.Left, _root, right.Location);
                        var member = new DotExpression(parent,
                            assignTypeMemberExpression.Left.Right, _root, right.Location);
                        right = new AssignTypeMemberExpression(member, assignTypeMemberExpression.Right,
                            right.Location);
                        return right;
                    }
                    case DotExpression dotExpression:
                        return new DotExpression(new DotExpression(left, dotExpression.Left, _root, left.Location),
                            dotExpression.Right, _root, dotExpression.Location);
                    case VariableExpression or FunctionCallExpression when left is BinaryExpression binaryExpression:
                        return new BinaryExpression(binaryExpression.Operator,
                            binaryExpression.Left,
                            new DotExpression(binaryExpression.Right, right, _root, right.Location),
                            binaryExpression.Location);
                    case BinaryExpression { Left: FunctionCallExpression functionCallExpression } binaryExpression:
                        var leftCall = new DotExpression(left, functionCallExpression, _root, left.Location);
                        var binary = new BinaryExpression(binaryExpression.Operator, leftCall, binaryExpression.Right, left.Location);

                        return binary;
                    default:
                        return new DotExpression(left, right, _root, left.Location);
                }
            }

            if (Match(TokenType.Plus))
            {
                left = new BinaryExpression(BinaryOperatorType.Plus, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Minus))
            {
                left = new BinaryExpression(BinaryOperatorType.Minus, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Is))
            {
                left = new BinaryExpression(BinaryOperatorType.Equals, left, ParseBinary(), left.Location);
                continue;
            }
            
            if (Match(TokenType.And))
            {
                left = new BinaryExpression(BinaryOperatorType.And, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Or))
            {
                left = new BinaryExpression(BinaryOperatorType.Or, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Greater))
            {
                left = new BinaryExpression(BinaryOperatorType.Greater, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Less))
            {
                left = new BinaryExpression(BinaryOperatorType.Less, left, ParseBinary(), left.Location);
                continue;
            }

            if (Match(TokenType.Equals))
            {
                if (IsWithOffset(-3, TokenType.Dot))
                    return new AssignTypeMemberExpression(
                        new DotExpression(last, left, _root, left.Location),
                        ParseBinary(),
                        left.Location
                    );

                if (left is VariableExpression variableExpression)
                    return new AssignExpression(variableExpression.Name, null, ParseBinary(), _root,
                        variableExpression.Location, AccessType.None);
            }

            break;
        }

        return left;
    }

    /// <summary>
    /// Parse for expression
    /// <code>
    /// for (variable) in (goal), (step)
    ///     [body]
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    private LoopExpression ParseForOrForeach()
    {
        Match(TokenType.For);

        var isPars = Match(TokenType.LeftParentheses);

        if (!Is(TokenType.Word))
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, Current.Location, TokenType.Word);
        }

        var variable = IsWithOffset(1, TokenType.Equals) ? ParseAssign(AccessType.None) : ParseValue();

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
                assignExpression.Root, assignExpression.Location, assignExpression.Access),
            VariableExpression => new VariableExpression(name, body, variable.Location),
            _ => variable
        };

        var condition = new BinaryExpression(BinaryOperatorType.Less,
            new VariableExpression(name, body, variable.Location), count, variable.Location);
        var step = new BinaryExpression(BinaryOperatorType.Plus, new VariableExpression(name, body, variable.Location),
            stepExpression, variable.Location);

        return new ForExpression(condition, variable, step, name, body, variable.Location);
    }

    /// <summary>
    /// Parse if expression
    /// <code>
    /// if (condition) then
    ///     [body]
    /// end
    /// </code>
    /// <code>
    /// or
    /// if (condition) then
    ///     [body]
    /// else
    ///     [else body]
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    private IfExpression ParseIf()
    {
        Match(TokenType.If);

        var condition = ParseBinary();

        MatchOrException(TokenType.Then);

        var body = ParseFunctionBody(TokenType.End, TokenType.Else);
        FunctionBodyExpression elseBody = null;

        if (IsWithOffset(-1, TokenType.Else))
        {
            elseBody = ParseFunctionBody();
        }

        return new IfExpression(condition, body, elseBody, condition.Location);
    }

    /// <summary>
    /// Parse implement function expression
    /// <code>
    /// implement function (type).(name)([arguments]): [type]
    ///     [body]
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    private ImplementFunctionDeclarationExpression ParseImplementFunctionDeclaration()
    {
        Match(TokenType.Implement);

        var isAsync = Match(TokenType.Async);
        var access = ParseAccess();

        MatchOrException(TokenType.Function);

        var type = Current;

        MatchOrException(TokenType.Word).MatchOrException(TokenType.Dot);

        var function = ParseFunctionDeclaration(access, isAsync);

        return new ImplementFunctionDeclarationExpression(type.Value, function, type.Location);
    }

    /// <summary>
    /// Parse function declaration expression
    /// <code>
    /// function (name)([arguments]): [type]
    ///     [body]
    /// end
    /// </code>
    /// </summary>
    /// <param name="accessType"></param>
    /// <param name="isAsync"></param>
    /// <returns></returns>
    private FunctionDeclarationExpression ParseFunctionDeclaration(AccessType accessType, bool isAsync = false)
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

        return new FunctionDeclarationExpression(name.Value, arguments, body, returnType, isAsync, accessType, _root,
            name.Location);
    }

    /// <summary>
    /// Parse type declaration expression
    /// <code>
    /// type (name)
    ///     [members]
    ///     ...
    /// end
    /// </code>
    /// </summary>
    /// <param name="accessType"></param>
    /// <returns></returns>
    private TypeDeclarationExpression ParseType(AccessType accessType)
    {
        Match(TokenType.Type);

        var name = Current;

        MatchOrException(TokenType.Word);

        var members = new List<ITypeMemberExpression>();

        while (!Match(TokenType.End))
        {
            members.Add(ParseTypeMember(name.Value));
        }

        return new TypeDeclarationExpression(name.Value, members, accessType, _root, name.Location);
    }

    /// <summary>
    /// Parse attributes
    /// <code>
    /// @(name)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private IEnumerable<string> ParseAttributes()
    {
        if (!Match(TokenType.At))
        {
            return Array.Empty<string>();
        }

        var attributes = new List<string>();

        while (IsNotEnded)
        {
            var name = Current;

            MatchOrException(TokenType.Word);

            attributes.Add(name.Value);

            if (Match(TokenType.At))
            {
                continue;
            }

            break;
        }

        return attributes;
    }

    /// <summary>
    /// Parse type member
    /// <code>
    /// (name): [type]
    /// </code>
    /// or
    /// <code>
    /// [async] function (name)([arguments]): [type]
    /// </code>
    /// </summary>
    /// <returns></returns>
    private ITypeMemberExpression ParseTypeMember(string parent)
    {
        var attributes = ParseAttributes();
        var access = ParseAccess();

        if (Match(TokenType.Async))
        {
            MatchOrException(TokenType.Function);

            return ParseTypeFunctionMember(parent, true, attributes, access);
        }

        if (Match(TokenType.Function))
        {
            return ParseTypeFunctionMember(parent, false, attributes, access);
        }

        return ParseTypeKeyMember(parent, attributes, access);
    }

    /// <summary>
    /// Parse type key member
    /// <code>
    /// (name): [type]
    /// </code>
    /// </summary>
    /// <returns></returns>
    private KeyTypeMemberExpression ParseTypeKeyMember(string parent, IEnumerable<string> attributes,
        AccessType accessType)
    {
        var name = Current;

        MatchOrException(TokenType.Word);

        var type = ParseObjectType(true);

        return new KeyTypeMemberExpression(name.Value, parent, type, attributes, accessType, name.Location);
    }

    /// <summary>
    /// Parse type function member, can be started after function keyword
    /// <code>
    /// [async] function (name)([arguments]): [type]
    /// </code>
    /// </summary>
    /// <returns></returns>
    private TypeFunctionMemberExpression ParseTypeFunctionMember(string parent, bool isAsync,
        IEnumerable<string> attributes, AccessType accessType)
    {
        Match(TokenType.Function);

        var name = Current;

        MatchOrException(TokenType.Word).MatchOrException(TokenType.LeftParentheses);

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

        return new TypeFunctionMemberExpression(parent, name.Value, arguments, returnValue, isAsync, attributes,
            accessType, name.Location);
    }

    /// <summary>
    /// Parse function body and change root
    /// <code>
    /// ...
    /// ...
    /// (end token)
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse arguments
    /// <code>
    /// (name1): [type1] = [default1], (name2): [type2] = [default2]
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse function argument
    /// <code>
    /// (name): [type] = [default]
    /// </code>
    /// </summary>
    /// <returns></returns>
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
                ParserThrowHelper.ThrowTokenExpectedException(Configuration, expression.Location, TokenType.String,
                    TokenType.Number, TokenType.True, TokenType.False);
            }

            defaultValue = expression.Evaluate(null);
        }

        if (!Is(0, TokenType.RightParentheses))
        {
            MatchOrException(TokenType.Comma);
        }

        return new FunctionArgument(name.Value, objectType, defaultValue);
    }

    /// <summary>
    /// Parse cast expression
    /// <code>
    /// (type) (object)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private CastExpression ParseCast()
    {
        Match(TokenType.LeftParentheses);

        var type = ParseObjectType();

        MatchOrException(TokenType.RightParentheses);

        var value = ParseBinary();

        return new CastExpression(type, value, value.Location);
    }

    /// <summary>
    /// Parse function call expression
    /// <code>
    /// function(value1, value2)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private FunctionCallExpression ParseFunctionCall()
    {
        var name = Current;

        MatchOrException(TokenType.Word)
            .MatchOrException(TokenType.LeftParentheses);

        var arguments = new List<AbstractEvaluableExpression>();

        while (!Match(TokenType.RightParentheses))
        {
            arguments.Add(ParseBinary());

            Match(TokenType.Comma);
        }

        return new FunctionCallExpression(name.Value, arguments.ToArray(), _root, name.Location);
    }

    /// <summary>
    /// Parse assign expression
    /// <code>
    /// variable = value
    /// </code>
    /// </summary>
    /// <returns></returns>
    private AssignExpression ParseAssign(AccessType access)
    {
        var name = Current;

        MatchOrException(TokenType.Word);

        ObjectTypeValue objectTypeValue = null;

        if (Match(TokenType.Colon))
        {
            objectTypeValue = ParseObjectType();
        }

        MatchOrException(TokenType.Equals);

        var value = ParseBinary();

        return new AssignExpression(name.Value, objectTypeValue, value, _root, name.Location, access);
    }

    /// <summary>
    /// Parse type member assign
    /// <code>
    /// type.member = value
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse return expression
    /// <code>
    /// return [value]
    /// </code>
    /// </summary>
    /// <returns></returns>
    private ReturnExpression ParseReturn()
    {
        Match(TokenType.Return);
        AbstractEvaluableExpression value;
        
        try
        {
            value = ParseBinary();
        }
        catch
        {
            value = new VoidExpression(Current.Location);
        }
        
        return new ReturnExpression(value, value.Location);
    }

    /// <summary>
    /// Parse module expression
    /// <code>
    /// module (name)
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse array
    /// <code>
    /// [ 1, 2, 3, 4, 5 ] 
    /// </code>
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Parse array assign expression
    /// <code>
    /// array[i] = value
    /// </code>
    /// </summary>
    /// <returns></returns>
    private AssignArrayMemberExpression ParseArrayAssign()
    {
        var name = Current;

        MatchOrException(TokenType.Word).
            MatchOrException(TokenType.LeftSquareBracket);

        var index = ParseBinary();

        MatchOrException(TokenType.RightSquareBracket).
            MatchOrException(TokenType.Equals);

        var value = ParseBinary();

        return new AssignArrayMemberExpression(new VariableExpression(name.Value, _root, name.Location), index, value, name.Location);
    }
    
    /// <summary>
    /// Parse array member expression
    /// <code>
    /// array[i]
    /// </code>
    /// </summary>
    /// <returns></returns>
    private ArrayMemberExpression ParseArrayMember()
    {
        var name = Current;

        MatchOrException(TokenType.Word).
            MatchOrException(TokenType.LeftSquareBracket);

        var index = ParseBinary();

        MatchOrException(TokenType.RightSquareBracket);

        return new ArrayMemberExpression(new VariableExpression(name.Value, _root, name.Location), index, name.Location);
    }
    
    /// <summary>
    /// Parse import expression
    /// <code>
    /// import (name)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private ImportExpression ParseImport()
    {
        Match(TokenType.Import);

        var name = ParseBinary();

        return new ImportExpression(name, name.Location);
    }

    /// <summary>
    /// Parse create expression
    /// <code>
    /// create (name)
    /// </code>
    /// </summary>
    /// <returns></returns>
    private CreateExpression ParseCreate()
    {
        Match(TokenType.Create);

        var name = Current;

        MatchOrException(TokenType.Word);

        return new CreateExpression(name.Value, _root, name.Location);
    }

    /// <summary>
    /// Parse value on current token.
    /// Unknown values at parsing too (variables, functions, e.t.c).
    /// </summary>
    /// <returns></returns>
    private AbstractEvaluableExpression ParseValue()
    {
        var current = Current;
        var access = ParseAccess();

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

        if (Match(TokenType.Call))
        {
            return ParseCall();
        }

        if (Match(TokenType.Async))
        {
            MatchOrException(TokenType.Function);

            return new FunctionExpression(ParseFunctionDeclaration(access, true), current.Location);
        }

        if (Match(TokenType.Function))
        {
            return new FunctionExpression(ParseFunctionDeclaration(access), current.Location);
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

            if (Is(TokenType.LeftSquareBracket))
            {
                Position--;

                return ParseArrayMember();
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

    /// <summary>
    /// Parse object type on curren token: Array, String, Boolean, Number, Round Number, Enum Member (?), Enum (?), Type.
    /// </summary>
    /// <param name="anyIfNull">Return any type if type not found otherwise exception</param>
    /// <returns>Type or exception</returns>
    private ObjectTypeValue ParseObjectType(bool anyIfNull = false)
    {
        Match(TokenType.Colon);

        var current = Current;

        if (Match(TokenType.Array))
        {
            return ObjectTypeValue.Array;
        }
        
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

    /// <summary>
    /// Parse access on current token: Static, Readonly, Bindable.
    /// </summary>
    /// <returns></returns>
    private AccessType ParseAccess()
    {
        var accessType = AccessType.None;

        if (Match(TokenType.Static))
        {
            accessType |= AccessType.Static;
        }

        if (Match(TokenType.ReadOnly))
        {
            accessType |= AccessType.ReadOnly;
        }

        if (Match(TokenType.Bindable))
        {
            accessType |= AccessType.Bindable;
        }

        return accessType;
    }

    /// <summary>
    /// Return true and skip current token if it in given otherwise return false.
    /// </summary>
    /// <param name="tokenTypes"></param>
    /// <returns>Skipped or not</returns>
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

    /// <summary>
    /// Skip current token if it in given otherwise throw exception.
    /// </summary>
    /// <param name="tokenTypes"></param>
    /// <returns>Current instance</returns>
    private Parser MatchOrException(params TokenType[] tokenTypes)
    {
        if (!Is(tokenTypes))
        {
            ParserThrowHelper.ThrowTokenExpectedException(Configuration, Current.Location, tokenTypes);
        }

        Skip();

        return this;
    }

    /// <summary>
    /// Return true if current token in given.
    /// </summary>
    /// <param name="tokenTypes"></param>
    /// <returns></returns>
    private bool Is(params TokenType[] tokenTypes)
    {
        if (IsEnded)
        {
            return false;
        }

        return tokenTypes.Contains(Current.Type);
    }

    /// <summary>
    /// Return true if current token in given.
    /// If offset more than tokens counts, it will return false.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="tokenTypes"></param>
    /// <returns></returns>
    private bool IsWithOffset(int offset, params TokenType[] tokenTypes)
    {
        if (Position + offset > Tokens.Count - 1)
        {
            return false;
        }

        return tokenTypes.Contains(Tokens[Position + offset].Type);
    }

    /// <summary>
    /// Skip current token
    /// </summary>
    private void Skip()
    {
        Position++;
    }
}