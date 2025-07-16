namespace TaskManagement.Application.Utils;

public class OperationResponse
{
    internal OperationResponse() { }

    public OperationResponse AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
        return this;
    }

    public OperationResponse AddErrors(IEnumerable<string> errorMessages)
    {
        if (errorMessages == null) return this;
        Errors.AddRange(errorMessages);
        return this;
    }

    public static OperationResponse FailedResponse(StatusCode errorCode = StatusCode.InternalServerError)
    {
        return new OperationResponse() { IsSuccessful = false, Code = errorCode };
    }

    public static OperationResponse SuccessfulResponse()
    {
        return new OperationResponse() { IsSuccessful = true, Code = StatusCode.OK };
    }

    public static OperationResponse CreatedResponse()
    {
        return new OperationResponse() { IsSuccessful = true, Code = StatusCode.Created };
    }

    public List<string> Errors { get; } = new List<string>();

    public bool IsSuccessful { get; set; }
    public StatusCode Code { get; set; }
}

public class OperationResponse<T> : OperationResponse
{
    internal OperationResponse() { }

    public static new OperationResponse<T> FailedResponse(StatusCode code = StatusCode.InternalServerError)
    {
        return new OperationResponse<T>() { IsSuccessful = false, Code = code };
    }
    public static OperationResponse<T> FailedResponse(T data, StatusCode code = StatusCode.InternalServerError)
    {
        return new OperationResponse<T>() { IsSuccessful = false, Data = data, Code = code };
    }
    public static OperationResponse<T> SuccessfulResponse(T output)
    {
        return new OperationResponse<T>() { IsSuccessful = true, Data = output, Code = StatusCode.OK };
    }
    public static OperationResponse<T> CreatedResponse(T output)
    {
        return new OperationResponse<T>() { IsSuccessful = true, Data = output, Code = StatusCode.Created };
    }

    public new OperationResponse<T> AddErrors(IEnumerable<string> errorMessages)
    {
        if (errorMessages == null) return this;
        Errors.AddRange(errorMessages);
        return this;
    }

    public new OperationResponse<T> AddError(string errorMessage)
    {
        Errors.Add(errorMessage);
        return this;
    }

    public T Data { get; private set; } = default!;
}
