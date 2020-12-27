#nullable disable

namespace Parsing.Junos
{
    public enum TokenType
    {
        ObjectStop,
        ObjectStart,
        ArrayStop,
        ArrayStart,
        LineStop,
        Word,
        Comment,
        Error
    }
}
