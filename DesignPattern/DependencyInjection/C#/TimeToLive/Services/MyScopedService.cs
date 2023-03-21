namespace TimeToLive.Services;

public interface IMyScopedService
{
    string GetInstanceAddress();
}

public class MyScopedService : IMyScopedService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}
