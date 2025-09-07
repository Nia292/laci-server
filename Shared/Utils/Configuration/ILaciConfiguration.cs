namespace LaciSynchroni.Shared.Utils.Configuration;

public interface ILaciConfiguration
{
    T GetValueOrDefault<T>(string key, T defaultValue);
    T GetValue<T>(string key);
    string SerializeValue(string key, string defaultValue);
}
