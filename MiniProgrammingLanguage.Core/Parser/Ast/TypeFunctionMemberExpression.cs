using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class TypeFunctionMemberExpression : AbstractExpression, ITypeMemberExpression
{
    public TypeFunctionMemberExpression(string parent, string name, FunctionArgument[] arguments, ObjectTypeValue returnValue, bool isAsync,
        IEnumerable<string> attributes, AccessType accessType, Location location) : base(location)
    {
        Parent = parent;
        Name = name;
        Arguments = arguments;
        Return = returnValue;
        IsAsync = isAsync;
        Attributes = attributes;
        Access = accessType;
    }
    
    public string Parent { get; }
    
    public string Name { get; }
    
    public FunctionArgument[] Arguments { get; }
    
    public ObjectTypeValue Return { get; }
    
    public bool IsAsync { get; }
    
    public IEnumerable<string> Attributes { get; }
    
    public AccessType Access { get; }

    public ITypeMember Create(string module)
    {
        if (Access.HasFlag(AccessType.Bindable))
        {
            return new TypeLanguageFunctionMemberInstance
            {
                Parent = Parent,
                Module = module,
                IsAsync = IsAsync,
                Identification = new FunctionTypeMemberIdentification
                {
                    Identifier = Name
                },
                Arguments = Arguments,
                Return = Return,
                Access = Access,
                Attributes = Attributes
            };
        }
        
        return new TypeFunctionMemberInstance
        {
            Parent = Parent,
            Module = module,
            IsAsync = IsAsync,
            Identification = new FunctionTypeMemberIdentification
            {
                Identifier = Name
            },
            Arguments = Arguments,
            Return = Return,
            Access = Access,
            Attributes = Attributes
        };
    }
}