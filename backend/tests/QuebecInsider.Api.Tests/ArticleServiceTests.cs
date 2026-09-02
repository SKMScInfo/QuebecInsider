using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Services;
using Xunit;

namespace QuebecInsider.Api.Tests;

public sealed class ArticleServiceTests
{
    [Fact]
    public async Task SearchAsync_RespectsSelectedLanguage()
    {
        await using var db = CreateDb();
        await SeedData.InitializeAsync(db);
        var service = new ArticleService(db);

        var french = await service.SearchAsync("fr-CA", "français", CancellationToken.None);
        var english = await service.SearchAsync("en-CA", "français", CancellationToken.None);

        Assert.NotEmpty(french);
        Assert.Empty(english);
    }

    [Fact]
    public async Task GetArticleAsync_ReturnsTranslatedAlternateSlug()
    {
        await using var db = CreateDb();
        await SeedData.InitializeAsync(db);
        var service = new ArticleService(db);

        var article = await service.GetArticleAsync("fr-CA", "apprendre-francais-quebec-7-trucs", CancellationToken.None);

        Assert.NotNull(article);
        Assert.Equal("learn-french-quebec-7-strategies", article.AlternateCultureSlug);
        Assert.Contains("bibliothèque", article.InsiderTip);
    }

    [Fact]
    public async Task GetTrendingAsync_ReturnsAtLeastThreeSeededArticles()
    {
        await using var db = CreateDb();
        await SeedData.InitializeAsync(db);
        var service = new ArticleService(db);

        var articles = await service.GetTrendingAsync("fr-CA", CancellationToken.None);

        Assert.True(articles.Count >= 3);
        Assert.Contains(articles, article => article.Slug == "nouveau-quebec-avantages-bancaires");
    }

    private static QuebecInsiderDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<QuebecInsiderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new QuebecInsiderDbContext(options);
    }
}
