namespace DSS.Interfaces
{
    public interface ILogService
    {
        public void LogMethodExit(string nameOfMethod);
        public void LogMethodInvocation(string nameOfMethod);
        public void LogFunctionExit(string nameOfFunction, Guid correlationId);
        public void LogFunctionInvocation(string nameOfFunction);
        public void LogUnableToLocateInHeader(string nameOfHeader);
    }
}
