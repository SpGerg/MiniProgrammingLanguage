using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class TypeDeclarationExpression : AbstractEvaluableExpression, IStatement
{
    public TypeDeclarationExpression(string name, IReadOnlyList<ITypeMemberExpression> members, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Members = members;
        Root = root;
    }
    
    public string Name { get; }
    
    public IReadOnlyList<ITypeMemberExpression> Members { get; }
    
    public FunctionBodyExpression Root { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var members = new List<ITypeMember>();

        foreach (var member in Members)
        {
            var result = member.Create(programContext.Module);
            
            if (result is TypeFunctionMemberInstance typeFunctionMemberInstance)
            {
                var value = typeFunctionMemberInstance.Create();
                var variableMember = new TypeVariableMemberInstance
                {
                    Parent = Name,
                    Module = programContext.Module,
                    Type = result.Type,
                    Identification = new KeyTypeMemberIdentification
                    {
                        Identifier = result.Identification.Identifier
                    },
                    Default = value,
                    IsReadonly = true,
                    IsFunctionInstance = true
                };

                members.Add(variableMember);
            }
            
            members.Add(result);
        }

        programContext.Variables.Add(new UserVariableInstance
        {
            Name = Name,
            Module = programContext.Module,
            Value = new ObjectTypeValue(Name, ValueType.Type),
            Type = new ObjectTypeValue(Name, ValueType.Type),
            Root = Root
        });
        
        programContext.Types.Add(new UserTypeInstance
        {
            Name = Name,
            Module = programContext.Module,
            Members = members,
            Root = Root
        });

        return new ObjectTypeValue(Name, ValueType.Type);
    }
}