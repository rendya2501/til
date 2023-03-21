namespace TimeToLive.Services;

public interface IMyTransientService
{
    string GetInstanceAddress();
}

public class MyTransientService : IMyTransientService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}