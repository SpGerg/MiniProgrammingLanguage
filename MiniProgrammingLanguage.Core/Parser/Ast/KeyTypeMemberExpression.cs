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
        Name = name;
        Parent = parent;
        Type = type;
    }

    public string Name { get; }
    
    public string Parent { get; }
    
    public ObjectTypeValue Type { get; }
    
    public ITypeMember Create()
    {
        return new TypeMemberInstance
        {
            Parent = Parent,
            Identification = new KeyTypeMemberIdentification
            {
                Identificator = Name
            },
            Type = Type,
            Default = null,
            IsReadonly = false
        };
    }
}