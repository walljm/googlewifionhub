#nullable disable

using System;

namespace Parsing.Junos
{
    [Serializable]
    public class ParseException : Exception
    {
        public string Location { get; } = string.Empty;
        public string ErrorText { get; } = string.Empty;

        public ParseException(string message)
            : base(message)
        {
        }

        public ParseException(string message, Token token)
            : base(message + " " + token.LocationToString())
        {
            Location = token.LocationToString();
            ErrorText = message;
        }

        public ParseException(Token token)
            : base(token.ToString())
        {
            Location = token.LocationToString();
            ErrorText = token.Type + ": " + token.Value;
        }
    }
}
