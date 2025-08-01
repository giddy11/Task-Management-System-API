using TaskManagement.Domain.Projects;

namespace TaskManagement.Application.Contracts.Persistence;

public interface IProjectRepository : IRepository<Project>
{

}

//public interface IProjectRepository<TEntity> : IRepository<TEntity> where TEntity : Project
//{

//}