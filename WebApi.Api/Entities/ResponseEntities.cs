namespace WebApi.Entities;

public class ResponseEntities<T>
{
    public string StatusCode { get; set; }
    public string Message { get; set; }
    public T Result { get; set; }
}