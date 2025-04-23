using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Variables;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class EnumDeclarationExpression : AbstractEvaluableExpression, IStatement
{
    public EnumDeclarationExpression(string name, IReadOnlyDictionary<string, int> members, FunctionBodyExpression root, Location location) : base(location)
    {
        Name = name;
        Members = members;
        Root = root;
    }
    
    public string Name { get; }
    
    public IReadOnlyDictionary<string, int> Members { get; }
    
    public FunctionBodyExpression Root { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var enumInstance = new UserEnumInstance
        {
            Name = Name,
            Members = Members,
            Root = Root
        };

        var enumValue = enumInstance.Create();
        
        programContext.Enums.Add(enumInstance, Location);
        programContext.Variables.Add(new UserVariableInstance
        {
            Name = Name,
            ObjectType = new ObjectTypeValue(Name, ValueType.Enum),
            Value = enumValue,
            Root = Root
        }, Location);

        return enumValue;
    }
}