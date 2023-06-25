namespace ApiComposition.Api.ServiceModel;

public class ArrayRequest<T>
{
    public IEnumerable<T>? Items { get; set; }
}