namespace ThatIntegrationEngine.Core.Components
{
    public enum ExecutionState
    {
        Success = 0,
        SuccessWithErrors,        
        FailedCanRetry,
        Failed,
        ProcessCompatExec
    }
}
