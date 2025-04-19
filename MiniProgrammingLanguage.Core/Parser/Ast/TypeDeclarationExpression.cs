using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Enums;
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
        var members = Members.Select(expression => expression.Create()).ToList();

        programContext.Types.Add(new TypeInstance
        {
            Name = Name,
            Members = members,
            Root = Root
        });

        return new ObjectTypeValue(Name, ValueType.Type);
    }
}