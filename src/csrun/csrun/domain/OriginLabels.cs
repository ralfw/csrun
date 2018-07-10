namespace csrun.domain.runtime
{
    internal class OriginLabels
    {
        private const string START_LABEL_PREFIX = "//#origin";
        private const string END_LABEL = "//#endorigin";
        
        public static (string startLabel, string endLabel) Create(string filename, int lineNumber) {
            return ($"{START_LABEL_PREFIX} {lineNumber},{filename}",
                END_LABEL);
        }

        public static bool TryParse(string line, out string filename, out int lineNumber) {
            filename = "";
            lineNumber = -1;
            if (!line.Trim().StartsWith(START_LABEL_PREFIX)) return false;

            line = line.Substring(START_LABEL_PREFIX.Length);
            var parts = line.Split(',');
            lineNumber = int.Parse(parts[0]);
            filename = parts[1].Trim();
            return true;
        }
    }
}