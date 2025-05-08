using System;
using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;

public class UserTypeInstance : ITypeInstance
{
    public required string Name { get; init; }

    public required string Module { get; init; }

    public required IReadOnlyList<ITypeMember> Members { get; set; }

    public required FunctionBodyExpression Root { get; init; }

    public Type Type { get; set; }

    public AccessType Access { get; init; }

    public bool TryChange(ProgramContext programContext, IInstance instance, Location location,
        out AbstractLanguageException exception)
    {
        if (instance is not ITypeInstance structureInstance)
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
        var result = new Dictionary<ITypeMemberIdentification, ITypeMemberValue>();

        foreach (var member in Members)
        {
            if (member is TypeVariableMemberInstance { IsFunctionInstance: true })
            {
                continue;
            }

            if (member is ITypeLanguageVariableMember languageMember)
            {
                result.Add(member.Identification, new TypeLanguageVariableMemberValue(languageMember)
                {
                    Type = member.Type
                });

                continue;
            }
            
            if (member is TypeLanguageFunctionMemberInstance functionMember)
            {
                var value = functionMember.Create();

                result.Add(member.Identification, new TypeLanguageFunctionMemberValue(functionMember)
                {
                    Type = ObjectTypeValue.Function
                });

                result.Add(new KeyTypeMemberIdentification
                {
                    Identifier = member.Identification.Identifier
                }, new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Instance = member,
                    Value = value
                });

                continue;
            }

            if (member is TypeFunctionMemberInstance typeFunctionMemberInstance)
            {
                var value = typeFunctionMemberInstance.Create();

                result.Add(member.Identification, new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Instance = member,
                    Value = value
                });

                result.Add(new KeyTypeMemberIdentification
                {
                    Identifier = member.Identification.Identifier
                }, new TypeMemberValue
                {
                    Type = ObjectTypeValue.Function,
                    Instance = member,
                    Value = value
                });

                continue;
            }

            result.Add(member.Identification, new TypeMemberValue
            {
                Type = member.Type,
                Instance = member,
                Value = NoneValue.Instance
            });
        }

        var typeValue = new TypeValue(this, result);

        return typeValue;
    }
}