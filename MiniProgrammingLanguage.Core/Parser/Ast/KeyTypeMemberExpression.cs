using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Identifications;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Types.Interfaces;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser.Ast.Interfaces;

namespace MiniProgrammingLanguage.Core.Parser.Ast;

public class KeyTypeMemberExpression : AbstractExpression, ITypeMemberExpression
{
    public KeyTypeMemberExpression(string name, string parent, ObjectTypeValue type, Location location) : base(location)
    {
        Parent = parent;
        Name = name;
        Type = type;
    }
    
    public string Parent { get; }

    public string Name { get; }

    public ObjectTypeValue Type { get; }
    
    public ITypeMember Create()
    {
        return new TypeVariableMemberInstance
        {
            Parent = Parent,
            Identification = new KeyTypeMemberIdentification
            {
                Identifier = Name
            },
            Type = Type,
            Default = new NoneValue(),
            IsReadonly = false
        };
    }
}