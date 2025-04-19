using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class TypeInstance : ITypeInstance
{
    public required string Name { get; init; }
    
    public required IReadOnlyList<ITypeMember> Members { get; set; }

    public required FunctionBodyExpression Root { get; init; }

    public bool TryChange(ProgramContext programContext, IRepositoryInstance repositoryInstance, Location location,
        out AbstractLanguageException exception)
    {
        if (repositoryInstance is not ITypeInstance structureInstance)
        {
            exception = new CannotAccessException(Name, location);
            return false;
        }

        Members = structureInstance.Members;

        exception = null;
        return true;
    }
    
    public TypeValue Create(IReadOnlyDictionary<ITypeMemberIdentification, AbstractValue> values = null)
    {
        var result = new Dictionary<ITypeMemberIdentification, TypeMemberValue>();
        
        if (values is not null)
        {
            foreach (var member in Members)
            {
                var value = values.FirstOrDefault(value => value.Key.Is(member.Identification));
                
                if (!value.Equals(default))
                {
                    result.Add(member.Identification, new TypeMemberValue
                    {
                        Type = member.Type,
                        Value = value.Value
                    });
                    
                    continue;
                }
                
                result.Add(member.Identification, new TypeMemberValue
                {
                    Type = member.Type,
                    Value = new NoneValue()
                });
            }
        }
        else
        {
            foreach (var member in Members)
            {
                result.Add(member.Identification, new TypeMemberValue
                {
                    Type = member.Type,
                    Value = new NoneValue()
                });
            }
        }

        return new TypeValue(Name, result);
    }
}