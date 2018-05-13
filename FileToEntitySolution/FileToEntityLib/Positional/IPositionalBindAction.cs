namespace FileToEntityLib.Positional
{
    public interface IPositionalBindAction : IBindAction<IPositionalBindAction>
    {
        int Size { get; set; }
        int StartPosition { get; set; }
        string CacheName { get; set; }
        CustomMask CustomMask { get; set; }
        CustomType CustomType { get; set; }
        string Mask { get; set; }
        string PropertyToBind { get; set; }
        string Type { get; set; }
        bool UseCache { get; }
        bool UseCustomType { get; }
        string Value { get; set; }

        IPositionalBindAction SetPosition(int start, int by);
    }
}