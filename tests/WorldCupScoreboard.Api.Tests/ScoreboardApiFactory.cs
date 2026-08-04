using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WorldCupScoreboard;
using WorldCupScoreboard.Tests.Fakes;

namespace WorldCupScoreboard.Api.Tests;

public class ScoreboardApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IScoreboard>();
            services.AddSingleton<IScoreboard>(new Scoreboard(new InMemoryMatchRepository()));
        });
    }
}
