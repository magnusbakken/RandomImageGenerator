namespace RandomImageGenerator.SafeList;

public record SafeListOptions
{
    public IList<string> Addresses { get; init; } = new List<string>();
}
