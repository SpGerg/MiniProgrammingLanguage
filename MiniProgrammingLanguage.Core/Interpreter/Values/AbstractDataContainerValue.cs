using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Values;

public abstract class AbstractDataContainerValue : AbstractValue
{
    public abstract ITypeMemberValue Get(ITypeMemberIdentification identification);

    public abstract void Set(ProgramContext programContext, ITypeMemberIdentification identification, AbstractValue abstractValue, Location location);
    
    public ITypeMemberValue Get(string identifier)
    {
        ITypeMemberIdentification identification;
        
        if (!identifier.EndsWith("()"))
        {
            identification = new KeyTypeMemberIdentification { Identifier = identifier };
        }
        else
        {
            identification = new FunctionTypeMemberIdentification { Identifier = identifier };
        }

        return Get(identification);
    }

    protected AbstractDataContainerValue(string name) : base(name)
    {
    }
}