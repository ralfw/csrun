using System;
using System.Linq;

namespace csrun.data.domain
{
    internal class TestMethodName
    {
        public TestMethodName(string label) {
            label = new string(label.Trim().Select(DefuseInvalidChars).ToArray());
            this.Value = $"Test{Guid.NewGuid().ToString().Replace("-", "")}__{label}";

            char DefuseInvalidChars(char c)
                => "@abcdefghijklmnopqrstuvwxyzäöüß".IndexOf(char.ToLower(c)) >= 0 ? c : '_';
        }
        
        public string Value { get; }

        public static string ExtractLabel(string functionName) {
            var iLabel = functionName.IndexOf("__") + 2;
            if (iLabel >= functionName.Length) return "";
            return functionName.Substring(iLabel).Replace("_", " ");
        }
    }
}