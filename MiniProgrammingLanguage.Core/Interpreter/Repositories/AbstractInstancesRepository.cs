using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories;

public abstract class AbstractInstancesRepository<T> : IInstancesRepository<T> where T : class, IRepositoryInstance
{
    public IEnumerable<T> Entities => _globalEntities;

    public IReadOnlyDictionary<FunctionBodyExpression, List<T>> Instances => _entities;

    private readonly Dictionary<FunctionBodyExpression, List<T>> _entities = new();

    private readonly List<T> _globalEntities = new();

    public void Add(T entity)
    {
        if (entity.Root is null)
        {
            _globalEntities.Add(entity);
            
            return;
        }
        
        if (!_entities.TryGetValue(entity.Root, out var instances))
        {
            _entities[entity.Root] = instances = new List<T>();
        }
        
        instances.Add(entity);
    }

    public bool Remove(T entity)
    {
        if (entity.Root is null)
        {
            return _globalEntities.Remove(entity);
        }
        
        return _entities.TryGetValue(entity.Root, out var instances) && instances.Remove(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            Add(entity);
        }
    }

    public bool AddOrSet(ProgramContext programContext, T entity, Location location)
    {
        var instance = Get(entity.Root, entity.Name, location);

        if (instance is null)
        {
            Add(entity);
            
            return true;
        }

        if (instance.TryChange(programContext, entity, location, out var exception))
        {
            return false;
        }

        throw exception;
    }
    
    public void Set(ProgramContext programContext, T entity, Location location)
    {
        var instance = Get(entity.Root, entity.Name, location);
    
        if (instance.TryChange(programContext, entity, location, out var exception))
        {
            return;
        }

        throw exception;
    }

    public T Get(FunctionBodyExpression functionBody, string name, Location location)
    {
        if (functionBody is null)
        {
            return _globalEntities.FirstOrDefault(e => e.Name == name);
        }

        if (!_entities.TryGetValue(functionBody, out var instances))
        {
            return Get(functionBody.Root, name, location);
        }
        
        var result = instances.FirstOrDefault(i => i.Name == name);
        return result ?? Get(functionBody.Root, name, location);
    }
    
    public void Clear()
    {
        _entities.Clear();
    }
}