namespace DSS.Interfaces
{
    public interface ILogService
    {
        public void LogMethodExit(string nameOfMethod);
        public void LogFunctionExit(string nameOfFunction);
    }
}
