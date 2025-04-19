using MiniProgrammingLanguage.Core.Interpreter.Repositories.Interfaces;

namespace MiniProgrammingLanguage.Core.Interpreter.Repositories.Tasks.Interfaces;

public interface ITasksRepository : IRepository<ITaskInstance>
{
    ITaskInstance Get(int id);
}