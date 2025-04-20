using System.Data;
using MiniProgrammingLanguage.Core.Interpreter.Exceptions;

namespace MiniProgrammingLanguage.Core.Interpreter;

public static class InterpreterThrowHelper
{
    public static void ThrowArgumentExceptedException(string name, Location location)
    {
        throw new ArgumentExceptedException(name, location);
    }

    public static void ThrowFunctionNotFoundException(string name, Location location)
    {
        throw new FunctionNotFoundException(name, location);
    }
    
    public static void ThrowCannotCastException(string from, string to, Location location)
    {
        throw new CannotCastException(from, to, location);
    }
    
    public static void ThrowInvalidReturnTypeException(string function, string excepted, string returned, Location location)
    {
        throw new InvalidReturnTypeException(function, excepted, returned, location);
    }
    
    public static void ThrowVariableNotFoundException(string variable, Location location)
    {
        throw new VariableNotFoundException(variable, location);
    }
    
    public static void ThrowIncorrectTypeException(string expected, string received, Location location)
    {
        throw new IncorrectTypeException(expected, received, location);
    }

    public static void ThrowCannotAccessException(string name, Location location)
    {
        throw new CannotAccessException(name, location);
    }

    public static void ThrowMemberNotFoundException(string type, string member, Location location)
    {
        throw new MemberNotFoundException(type, member, location);
    }

    public static void ThrowTypeNotFoundException(string name, Location location)
    {
        throw new TypeNotFoundException(name, location);
    }

    public static void ThrowWrongImportModuleException(string module, Location location)
    {
        throw new WrongImportModuleException(module, location);
    }

    public static void ThrowCyclicImportException(string module, Location location)
    {
        throw new CyclicImportException(module, location);
    }

    public static void ThrowDuplicateNameException(string name, Location location)
    {
        throw new Exceptions.DuplicateNameException(name, location);
    }
}