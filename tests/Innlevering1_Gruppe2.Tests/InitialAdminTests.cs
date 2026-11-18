using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Reachout.Services;

namespace Reachout.Tests;

public class InitialAdminTests
{
    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public async Task InitializeAsync_DoesNotAccessDatabaseOutsideDevelopment(string environmentName)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["InitialAdmin:Email"] = "admin@test.no",
                ["InitialAdmin:Password"] = "adminadminadmin",
                ["LocalPreview"] = "true"
            }).Build();

        // En null database vil feile dersom opprettelsen forsøkes i disse miljøene.
        await InitialAdmin.InitializeAsync(null!, configuration, new TestEnvironment(environmentName));
    }

    private sealed class TestEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "Reachout.Tests";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
