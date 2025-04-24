namespace DeliveryAssignmentEngine.Domain.Services;

public class AssignmentResult
{
    public bool IsSuccessful { get; }
    public string ErrorMessage { get; }

    private AssignmentResult(bool isSuccessful, string errorMessage)
    {
        IsSuccessful = isSuccessful;
        ErrorMessage = errorMessage;
    }

    public static AssignmentResult Successful() => new(true, string.Empty);
    public static AssignmentResult Failed(string errorMessage) => new(false, errorMessage);
}
