namespace csrun.data.dto
{
    class ResultLogDto {}
    
    class CompilerErrorLogDto : ResultLogDto {
        public string[] Errors;
    }
    
    class RuntimeFailureLogDto : ResultLogDto {
        public string Message;
    }
    
    class TestResultsLogDto : ResultLogDto
    {
        public class TestFailureDto {
            public string Label;
            public string Message;
        }
        
        public (bool success, string label)[] Results;
        public TestFailureDto[] Failures;
    }
}