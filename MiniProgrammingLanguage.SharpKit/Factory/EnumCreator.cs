using System;
using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Enums;
using MiniProgrammingLanguage.Core.Interpreter.Values.EnumsValues;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.SharpKit.Factory;

public static class EnumCreator
{
    public static EnumValue Create(Type type)
    {
        var members = new Dictionary<string, int>();
        
        var names = type.GetEnumNames();
        var values = type.GetEnumValues();

        for (var i = 0; i < names.Length; i++)
        {
            var name = names[i];
            var value = (int) values.GetValue(i);
            
            members.Add(name, value);
        }

        var enumInstance = new UserEnumInstance
        {
            Name = type.Name,
            Type = type,
            Module = type.Assembly.GetName().Name,
            Members = members,
            Access = AccessType.Static,
            Root = null
        };

        return new EnumValue(enumInstance);
    }
}