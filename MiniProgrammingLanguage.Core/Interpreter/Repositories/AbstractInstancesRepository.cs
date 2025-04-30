using System;
using System.Collections.Generic;
using System.Linq;
using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;
using MiniProgrammingLanguage.Core.Parser.Ast;
using MiniProgrammingLanguage.Core.Parser.Ast.Enums;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories;

public abstract class AbstractInstancesRepository<T> : IInstancesRepository<T> where T : class, IInstance
{
    public IEnumerable<T> Entities => GlobalEntities;

    public IReadOnlyDictionary<FunctionBodyExpression, List<T>> Instances => BodiesEntities;

    protected Dictionary<FunctionBodyExpression, List<T>> BodiesEntities { get; } = new();

    protected List<T> GlobalEntities { get; } = new();

    public void Add(T entity, Location location, bool isCheckExisting = true)
    {
        if (entity.Module is not "global" && isCheckExisting)
        {
            var existing = Get(entity.Root, entity.Name, entity.Module, location);

            if (existing is not null)
            {
                InterpreterThrowHelper.ThrowDuplicateNameException(entity.Name, location);
            }
        }

        if (entity.Root is null)
        {
            GlobalEntities.Add(entity);

            return;
        }

        if (!BodiesEntities.TryGetValue(entity.Root, out var list))
        {
            BodiesEntities[entity.Root] = list = new List<T>();
        }

        list.Add(entity);
    }

    public void Add(T entity)
    {
        Add(entity, Location.Default);
    }

    public bool Remove(T entity)
    {
        if (entity.Root is null)
        {
            return GlobalEntities.Remove(entity);
        }

        return BodiesEntities.TryGetValue(entity.Root, out var instances) && instances.Remove(entity);
    }

    public void AddRange(IEnumerable<T> entities, bool isCheckExisting = true)
    {
        foreach (var entity in entities)
        {
            Add(entity, Location.Default, isCheckExisting);
        }
    }

    public void AddRange(IEnumerable<T> entities)
    {
        AddRange(entities, true);
    }

    public bool AddOrSet(ProgramContext programContext, T entity, Location location)
    {
        var instance = Get(entity.Root, entity.Name, programContext.Module, location);

        if (instance is null)
        {
            Add(entity, location, false);

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
        var instance = Get(entity.Root, entity.Name, programContext.Module, location);

        if (instance.TryChange(programContext, entity, location, out var exception))
        {
            return;
        }

        throw exception;
    }

    public T Get(FunctionBodyExpression functionBody, string name, string module, Location location)
    {
        var currentBody = functionBody;

        while (currentBody is not null)
        {
            if (BodiesEntities.TryGetValue(currentBody, out var instances))
            {
                foreach (var instance in instances.Where(instance => instance.Name == name))
                {
                    return instance;
                }
            }

            currentBody = currentBody.Root;
        }

        var entity = GlobalEntities.FirstOrDefault(entity => entity.Name == name);

        if (entity is not null && !entity.Access.HasFlag(AccessType.Static) && entity.Module != module)
        {
            InterpreterThrowHelper.ThrowCannotAccessException(entity.Name, location);
        }

        return entity;
    }

    public void Clear(FunctionBodyExpression functionBodyExpression)
    {
        if (functionBodyExpression is null)
        {
            Clear();

            return;
        }

        BodiesEntities.Remove(functionBodyExpression);
    }

    public void Clear()
    {
        GlobalEntities.Clear();
    }
}