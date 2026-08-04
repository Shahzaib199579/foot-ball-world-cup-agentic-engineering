using System.Net;
using System.Net.Http.Json;
using WorldCupScoreboard.Api.Contracts;

namespace WorldCupScoreboard.Api.Tests;

public class GetHistoryEndpointTests
{
    [Fact]
    public async Task GetHistory_WithMoreMatchesThanOnePage_ReturnsCorrectPageContents()
    {
        using var factory = new ScoreboardApiFactory();
        var client = factory.CreateClient();

        for (var i = 0; i < 12; i++)
        {
            await client.PostAsJsonAsync(
                "/matches", new StartMatchRequest($"Home{i}", $"Away{i}", DateTime.UtcNow, $"Stadium{i}"));
        }

        var page1Response = await client.GetAsync("/matches/history?page=1");
        var page2Response = await client.GetAsync("/matches/history?page=2");

        Assert.Equal(HttpStatusCode.OK, page1Response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, page2Response.StatusCode);
        var page1 = await page1Response.Content.ReadFromJsonAsync<MatchResponse[]>();
        var page2 = await page2Response.Content.ReadFromJsonAsync<MatchResponse[]>();
        Assert.Equal(10, page1!.Length);
        Assert.Equal(2, page2!.Length);
    }

    [Fact]
    public async Task GetHistory_WithOutOfRangePage_Returns200WithEmptyArray()
    {
        using var factory = new ScoreboardApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/matches/history?page=999");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var matches = await response.Content.ReadFromJsonAsync<MatchResponse[]>();
        Assert.NotNull(matches);
        Assert.Empty(matches!);
    }

    [Fact]
    public async Task GetHistory_WithInvalidPage_Returns400WithInvalidPageError()
    {
        using var factory = new ScoreboardApiFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/matches/history?page=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.Equal("invalid_page", error!.ErrorCode);
    }
}
