namespace FileToEntityLib
{
    /// <summary>
    /// Define as possíveis oeprações de comparação realizadas pela aplicação
    /// </summary>
    public enum OperatorType
    {
        Equal = 0,
        Lesser = 1,
        Greater = 2,
        LesserOrEqual = 3,
        GreaterOrEqual = 4,
        NotEquals = 5,
        Contains = 6,
        NotContains = 7,
        ContainsValue = 8,
        IsMatch = 9
    }
}