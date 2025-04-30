using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Functions;
using MiniProgrammingLanguage.Core.Interpreter.Values;
using MiniProgrammingLanguage.Core.Parser;
using MiniProgrammingLanguage.SharpKit.Exceptions;

namespace MiniProgrammingLanguage.SharpKit.Functions;

public static class RequireDependencyFunction
{
    private const string Prefix = "cs.";
    
    public static LanguageFunctionInstance Create()
    {
        return new LanguageFunctionInstanceBuilder()
            .SetName("require_dependency")
            .SetArguments(new FunctionArgument("dependency", ObjectTypeValue.String))
            .SetBind(RequireDependency)
            .Build();
    }

    public static AbstractValue RequireDependency(LanguageFunctionExecuteContext context)
    {
        var argument = context.Arguments.First();
        var content = argument.AsString(context.ProgramContext, context.Location);

        foreach (var module in context.ProgramContext.Imported)
        {
            if (!module.StartsWith(Prefix))
            {
                continue;
            }

            var originalName = module.Remove(0, Prefix.Length);

            if (originalName != content)
            {
                continue;
            }

            return new VoidValue();
        }

        throw new ModuleNotFoundException(content, context.Location);
    }
}