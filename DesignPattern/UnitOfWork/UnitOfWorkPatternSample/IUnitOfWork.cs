namespace UnitOfWorkPatternSample;

public interface IUnitOfWork : IDisposable
{
    ICourseRepository Courses {get;}
    IAutherRepository Authors {get;}
    int Complete();
}