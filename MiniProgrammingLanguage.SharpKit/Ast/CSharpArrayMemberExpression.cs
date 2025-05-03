using System;
using MiniProgrammingLanguage.Core;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.SharpKit.Factory;

namespace MiniProgrammingLanguage.SharpKit.Ast;

public class CSharpArrayMemberExpression : AbstractEvaluableExpression
{
    public CSharpArrayMemberExpression(Array array, int index, Location location) : base(location)
    {
        Array = array;
        Index = index;
    }
    
    public Array Array { get; }
    
    public int Index { get; }

    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var type = TypesFactory.Create(Array.GetValue(Index), programContext, out var implementModule);
        programContext.Import(implementModule);
        
        return type;
    }
}