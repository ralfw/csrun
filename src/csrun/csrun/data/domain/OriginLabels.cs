namespace csrun.data.domain
{
    /*
     * Origin labels are surround csrun code which gets inserted into csharp template code.
     * See also: Rendering{}, FailureMapper{}
     */
    internal class OriginLabels
    {
        private const string START_LABEL_PREFIX = "//#origin";
        private const string END_LABEL = "//#endorigin";
        
        private readonly string _filename;
        private readonly int _lineNumber;

        public OriginLabels(string filename, int lineNumber) {
            _filename = filename;
            _lineNumber = lineNumber;
        }
        

        public string StartLabel => $"{START_LABEL_PREFIX} {_lineNumber},{_filename}";
        public string EndLabel => END_LABEL;

        
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