using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums;

public class UserEnumInstance : IEnumInstance
{
    public required string Name { get; init; }
    
    public required FunctionBodyExpression Root { get; init; }
    
    public required IReadOnlyDictionary<string, int> Members { get; init; }
    
    public bool TryGetByName(string name, out int value)
    {
        return Members.TryGetValue(name, out value);
    }

    public bool TryGetByValue(int value, out string name)
    {
        var member = Members.FirstOrDefault(member => member.Value == value);

        if (member.Equals(default))
        {
            name = null;
            return false;
        }

        name = member.Key;
        return true;
    }

    public bool TryChange(ProgramContext programContext, IRepositoryInstance repositoryInstance, Location location,
        out AbstractLanguageException exception)
    {
        exception = new CannotAccessException(Name, location);
        return false;
    }
    
    public EnumValue Create()
    {
        return new EnumValue(this);
    }
}