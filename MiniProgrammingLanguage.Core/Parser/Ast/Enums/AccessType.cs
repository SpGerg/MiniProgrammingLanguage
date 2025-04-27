using System;

namespace MiniProgrammingLanguage.Core.Parser.Ast.Enums;

[Flags]
public enum AccessType : byte
{
    None,
    Static,
    ReadOnly,
    Bindable
}