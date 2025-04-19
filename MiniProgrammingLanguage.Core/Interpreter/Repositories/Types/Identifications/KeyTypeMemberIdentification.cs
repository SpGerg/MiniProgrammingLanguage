using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;

public class KeyTypeMemberIdentification : ITypeMemberIdentification
{
    public required string Identificator { get; init; }
    
    public bool Is(ITypeMemberIdentification identification)
    {
        if (identification is not KeyTypeMemberIdentification)
        {
            return false;
        }

        return identification.Identificator == Identificator;
    }
}