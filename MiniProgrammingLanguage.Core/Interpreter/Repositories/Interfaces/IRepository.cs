using System.Collections.Generic;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;

public interface IRepository<T>
{
    IEnumerable<T> Entities { get; }

    void Add(T entity);

    bool Remove(T entity);

    void Clear();
}