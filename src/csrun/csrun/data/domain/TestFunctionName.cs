using System;

namespace csrun.data.domain
{
    internal class TestFunctionName
    {
        public TestFunctionName(string label) {
            label = label.Trim().Replace(" ", "_");
            this.Value = $"Test{Guid.NewGuid().ToString().Replace("-", "")}__{label}";
        }
        
        public string Value { get; }

        public static string ExtractLabel(string functionName) {
            var iLabel = functionName.IndexOf("__") + 2;
            if (iLabel >= functionName.Length) return "";
            return functionName.Substring(iLabel).Replace("_", " ");
        }
    }
}