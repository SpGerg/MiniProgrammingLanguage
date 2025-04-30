using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks;

public class TasksRepository : ITasksRepository
{
    public IEnumerable<ITaskInstance> Entities => _entities;

    private readonly List<ITaskInstance> _entities = new();

    public void Add(ITaskInstance entity)
    {
        _entities.Add(entity);
    }

    public void AddRange(IEnumerable<ITaskInstance> entities)
    {
        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public bool Remove(ITaskInstance entity)
    {
        var result = _entities.RemoveAll(target => target.Task == entity.Task);

        if (result > -1)
        {
            entity.Token.Cancel();
        }

        return result > -1;
    }

    public ITaskInstance Get(int id)
    {
        return _entities.FirstOrDefault(entity => entity.Task.Id == id);
    }

    public void Clear()
    {
        foreach (var entity in _entities)
        {
            entity.Token.Cancel();
        }

        _entities.Clear();
    }
}