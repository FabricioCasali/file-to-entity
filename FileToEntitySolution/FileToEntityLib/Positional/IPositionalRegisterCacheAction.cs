namespace FileToEntityLib.Positional
{
    public interface IPositionalRegisterCacheAction : IRegisterCache<IPositionalRegisterCacheAction>
    {
        int StartPosition { get; set; }

        int Size { get; set; }

        IPositionalRegisterCacheAction SetPosition(int start, int by);
    }
}