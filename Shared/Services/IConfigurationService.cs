using LaciSynchroni.Shared.Utils.Configuration;

namespace LaciSynchroni.Shared.Services;

public interface IConfigurationService<T> where T : class, ILaciConfiguration
{
    bool IsMain { get; }

    event EventHandler ConfigChangedEvent;

    T1 GetValue<T1>(string key);
    T1 GetValueOrDefault<T1>(string key, T1 defaultValue);
    string ToString();
}
