#nullable disable

namespace Parsing.Junos
{
    public class Token
    {
        public string Value;
        public TokenType Type;
        public int Line;
        public int Column;

        public string LocationToString()
        {
            return "Line: " + (Line + 1) + " Column: " + (Column + 1);
        }

        public override string ToString()
        {
            return Value + " " + LocationToString();
        }
    }
}
