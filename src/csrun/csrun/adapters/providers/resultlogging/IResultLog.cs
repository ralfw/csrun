namespace csrun.adapters.providers.resultlogging
{
    internal interface IResultLog {
        void DisplayCompilerErrors(string[] errors);
        void DisplayRuntimeFailure(string failure);
        void DisplayTestFailure(string label, string failure);
        void DisplayTestResults((bool success, string label)[] results);
    }
}