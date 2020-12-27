#nullable disable

using System.Collections.Generic;
using System.Text;

namespace Parsing.Junos
{
    public class Tag
    {
        public TagTypes TagType { get; set; } = TagTypes.Object;

        private string _Name = string.Empty;
        public string Name { get { return _Name; } set { _Name = value.Trim(); } }

        public string Value { get; set; } = string.Empty;

        public Stack<Tag> Children { get; set; } = new Stack<Tag>();

        public override string ToString()
        {
            return Name + ": " + TagType;
        }

        public string ToJSON(string indent = "")
        {
            var innerIndent = $"{indent}    ";

            var sb = new StringBuilder();
            if (TagType == TagTypes.Word)
            {
                sb.Append($"{innerIndent}\"{Value}\"");
            }
            else if (TagType == TagTypes.Property)
            {
                sb.Append($"{innerIndent}\"{Name}\": \"{Value}\"");
            }
            else if (TagType == TagTypes.Object)
            {
                sb.Append($"{innerIndent}\"{Name}\": {{\n");
                foreach (var tag in Children)
                {
                    sb.Append(tag.ToJSON(innerIndent));
                    sb.Append(",\n");
                }
                sb.Remove(sb.Length - 2, 1); // remove the last comma
                sb.Append($"{innerIndent}}}");
            }
            else if (TagType == TagTypes.Array)
            {
                sb.Append($"{innerIndent}\"{Name}\": [\n");
                foreach (var tag in Children)
                {
                    sb.Append(tag.ToJSON(innerIndent));
                    sb.Append(",\n");
                }

                sb.Remove(sb.Length - 2, 1); // remove the last comma
                sb.Append($"{innerIndent}]");
            }

            return sb.ToString();
        }
    }
}