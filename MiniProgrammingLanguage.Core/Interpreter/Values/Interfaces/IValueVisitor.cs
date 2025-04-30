using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

public interface IValueVisitor
{
    bool Visit(TypeValue typeValue);

    bool Visit(ArrayValue arrayValue);

    bool Visit(BooleanValue booleanValue);

    bool Visit(FunctionValue functionValue);

    bool Visit(NoneValue noneValue);

    bool Visit(TypeInstanceValue typeInstanceValue);

    bool Visit(CSharpObjectValue cSharpObjectValue);

    bool Visit(NumberValue numberValue);

    bool Visit(ObjectTypeValue objectTypeValue);

    bool Visit(RoundNumberValue roundNumberValue);

    bool Visit(EnumValue enumValue);

    bool Visit(StringValue stringValue);

    bool Visit(EnumMemberValue enumMemberValue);

    bool Visit(VoidValue voidValue);
}