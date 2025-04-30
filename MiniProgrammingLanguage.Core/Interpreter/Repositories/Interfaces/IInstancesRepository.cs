using System.Collections.Generic;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;

public interface IInstancesRepository<T> : IRepository<T> where T : IInstance
{
    IReadOnlyDictionary<FunctionBodyExpression, List<T>> Instances { get; }

    void Add(T entity, Location location, bool isCheckExisting = true);

    void AddRange(IEnumerable<T> entities, bool isCheckExisting = true);

    bool AddOrSet(ProgramContext programContext, T entity, Location location);

    void Set(ProgramContext programContext, T entity, Location location);

    T Get(FunctionBodyExpression functionBody, string name, string module, Location location);

    void Clear(FunctionBodyExpression functionBodyExpression);
}