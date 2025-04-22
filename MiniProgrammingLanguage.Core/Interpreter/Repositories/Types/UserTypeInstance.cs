using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class UserTypeInstance : ITypeInstance
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

    public ITypeMember Get(ITypeMemberIdentification identification)
    {
        return Members.FirstOrDefault(member => member.Identification.Is(identification));
    }
    
    public TypeValue Create()
    {
        var result = new Dictionary<ITypeMemberIdentification, TypeMemberValue>();
        
        foreach (var member in Members)
        {
            if (member is TypeVariableMemberInstance typeMember && typeMember.IsFunctionInstance)
            {
                continue;
            }
            
            if (member is TypeFunctionMemberInstance typeFunctionMemberInstance)
            {
                var value = typeFunctionMemberInstance.Create();
                
                result.Add(member.Identification, new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Value = value
                });
                    
                result.Add(new KeyTypeMemberIdentification
                {
                    Identifier = member.Identification.Identifier
                }, new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Value = value
                });
                    
                continue;
            }
                
            result.Add(member.Identification, new TypeMemberValue
            {
                Type = member.Type,
                Value = new NoneValue()
            });
        }

        return new TypeValue(Name, result);
    }
}