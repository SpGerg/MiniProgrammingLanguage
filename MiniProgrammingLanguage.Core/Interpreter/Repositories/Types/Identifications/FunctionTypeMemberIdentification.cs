using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;

public class FunctionTypeMemberIdentification : ITypeMemberIdentification
{
    public required string Identifier { get; init; }
    
    public bool Is(ITypeMemberIdentification identification)
    {
        if (identification is not FunctionTypeMemberIdentification)
        {
            return false;
        }

        return identification.Identifier == Identifier;
    }
}