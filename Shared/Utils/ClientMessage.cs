using LaciSynchroni.Common.Data.Enum;

namespace LaciSynchroni.Shared.Utils;
public record ClientMessage(MessageSeverity Severity, string Message, string UID);
