using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class TableExpression : AbstractEvaluableExpression
{
    public TableExpression(IReadOnlyDictionary<string, AbstractEvaluableExpression> members, Location location) : base(location)
    {
        Members = members;
    }

    public IReadOnlyDictionary<string, AbstractEvaluableExpression> Members { get; }
    
    public override AbstractValue Evaluate(ProgramContext programContext)
    {
        var members = new Dictionary<ITypeMemberIdentification, ITypeMemberValue>();

        foreach (var member in Members)
        {
            switch (member.Value)
            {
                case FunctionDeclarationExpression functionDeclarationExpression:
                {
                    var functionIdentification = new FunctionTypeMemberIdentification
                    {
                        Identifier = member.Key
                    };
                
                    var variableIdentification = new FunctionTypeMemberIdentification
                    {
                        Identifier = member.Key
                    };
                
                    members.Add(functionIdentification, new TypeMemberValue
                    {
                        Type = ObjectTypeValue.Function,
                        Value = functionDeclarationExpression.Create(programContext.Module).Create(),
                        Instance = null
                    });
                
                    members.Add(variableIdentification, new TypeMemberValue
                    {
                        Type = ObjectTypeValue.Function,
                        Value = functionDeclarationExpression.Create(programContext.Module).Create(),
                        Instance = null
                    });
                    break;
                }
                default:
                {
                    var identification = new KeyTypeMemberIdentification
                    {
                        Identifier = member.Key
                    };

                    var value = member.Value.Evaluate(programContext);
                    
                    members.Add(identification, new TypeMemberValue
                    {
                        Type = ObjectTypeValue.Any,
                        Value = value,
                        Instance = null
                    });
                    break;
                }
            }
        }
        
        return new TableValue(members);
    }
}