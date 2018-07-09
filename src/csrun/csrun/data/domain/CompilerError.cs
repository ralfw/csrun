namespace csrun.data.domain
{
    internal class CompilerError
    {
        public string Filename;
        public int LineNumber;
        public int ColumnNumber;
        public string Description;
    }
}