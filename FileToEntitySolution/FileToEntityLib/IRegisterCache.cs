namespace FileToEntityLib
{
    public interface IRegisterCache<T> : IAction where T : IAction
    {
        string CacheName { get; set; }

        string CacheValue { get; set; }

        T Register(string name);
    }
}