namespace TimeToLive.Services;

public interface IMySingletonService
{
    string GetInstanceAddress();
}

public class MySingletonService : IMySingletonService
{
    public string GetInstanceAddress()
    {
        return this.GetHashCode().ToString();
    }
}

