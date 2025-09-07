namespace LaciSynchroni.StaticFilesServer.Utils;

public record UserRequest(Guid RequestId, string User, List<string> FileIds)
{
    public bool IsCancelled { get; set; } = false;
}