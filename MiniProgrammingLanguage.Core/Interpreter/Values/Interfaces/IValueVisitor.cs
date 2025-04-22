using MiniProgrammingLanguage.Core.Interpreter.Values.Type;

namespace MiniProgrammingLanguage.Core.Interpreter.Values.Interfaces;

public interface IValueVisitor
{
    bool Visit(TypeValue typeValue);
    
    bool Visit(ArrayValue arrayValue);
    
    bool Visit(BooleanValue booleanValue);
    
    bool Visit(FunctionValue functionValue);
    
    bool Visit(NoneValue noneValue);
    
    bool Visit(NumberValue numberValue);
    
    bool Visit(ObjectTypeValue objectTypeValue);
    
    bool Visit(RoundNumberValue roundNumberValue);
    
    bool Visit(StringValue stringValue);
    
    bool Visit(VoidValue voidValue);
}