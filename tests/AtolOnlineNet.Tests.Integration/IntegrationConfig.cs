using System.IO;

namespace AtolOnlineNet.Tests.Integration;

/// <summary>
/// Loads ATOL Online integration credentials from environment variables, falling back to a <c>.env</c>
/// file walked up from the test output directory. All values are optional; when absent, integration
/// tests short-circuit so the suite stays green without secrets.
/// </summary>
internal static class IntegrationConfig
{
    private static readonly Dictionary<string, string> DotEnv = LoadDotEnv();

    public static string? Login => Get("ATOL_ONLINE_TEST_LOGIN");

    public static string? Password => Get("ATOL_ONLINE_TEST_PASSWORD");

    public static string? GroupCode => Get("ATOL_ONLINE_TEST_GROUP_CODE");

    public static string? Inn => Get("ATOL_ONLINE_TEST_INN");

    public static string? PaymentAddress => Get("ATOL_ONLINE_TEST_PAYMENT_ADDRESS");

    public static string BaseUrl => Get("ATOL_ONLINE_TEST_BASE_URL") ?? "https://testonline.atol.ru/possystem/";

    public static bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Login) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(GroupCode);

    public static AtolOnlineClientOptions CreateOptions() => new()
    {
        Login = Login!,
        Password = Password!,
        GroupCode = GroupCode!,
        BaseAddress = new Uri(BaseUrl),
        ApiVersion = "v4",
    };

    private static string? Get(string key)
    {
        var fromEnv = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        return DotEnv.TryGetValue(key, out var value) ? value : null;
    }

    private static Dictionary<string, string> LoadDotEnv()
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null)
        {
            var path = Path.Combine(dir.FullName, ".env");
            if (File.Exists(path))
            {
                foreach (var raw in File.ReadAllLines(path))
                {
                    var line = raw.Trim();
                    if (line.Length == 0 || line.StartsWith('#'))
                    {
                        continue;
                    }

                    var eq = line.IndexOf('=');
                    if (eq <= 0)
                    {
                        continue;
                    }

                    var key = line[..eq].Trim();
                    var value = line[(eq + 1)..].Trim().Trim('"');
                    if (key.Length > 0 && !result.ContainsKey(key))
                    {
                        result[key] = value;
                    }
                }

                break;
            }

            dir = dir.Parent;
        }

        return result;
    }
}
