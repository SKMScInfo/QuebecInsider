using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Domain;
using QuebecInsider.Api.Dtos;

namespace QuebecInsider.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminController(
    QuebecInsiderDbContext db,
    IOutputCacheStore cacheStore) : ControllerBase
{
    // GET: api/admin/dashboard (Statistiques + Liste des abonnés Newsletter)
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken ct)
    {
        var totalArticles = await db.Articles.CountAsync(ct);
        var totalViews = await db.Articles.SumAsync(a => a.PopularityScore, ct);
        var subscribers = await db.NewsletterSubscribers
            .AsNoTracking()
            .OrderByDescending(s => s.SubscribedAtUtc)
            .Select(s => new { s.Id, s.Email, s.Culture, s.SubscribedAtUtc })
            .ToListAsync(ct);

        return Ok(new
        {
            TotalArticles = totalArticles,
            TotalPopularityScore = totalViews,
            SubscriberCount = subscribers.Count,
            Subscribers = subscribers
        });
    }

    // POST: api/admin/articles (Créer un article bilingue)
    [HttpPost("articles")]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequestDto req, CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Key == req.CategoryKey, ct);
        if (category is null)
        {
            return BadRequest(new { error = $"Catégorie '{req.CategoryKey}' introuvable." });
        }

        var article = new Article
        {
            CategoryId = category.Id,
            ImageUrl = req.ImageUrl,
            AuthorName = req.AuthorName,
            ReadingMinutes = req.ReadingMinutes,
            PopularityScore = req.InitialPopularityScore,
            PublishedAt = DateTimeOffset.UtcNow,
            Translations =
            [
                new ArticleTranslation
                {
                    Culture = "fr-CA",
                    Title = req.French.Title,
                    Slug = req.French.Slug,
                    Description = req.French.Description,
                    Body = req.French.Body,
                    SeoTitle = req.French.SeoTitle,
                    SeoDescription = req.French.SeoDescription,
                    Keywords = req.French.Keywords,
                    InsiderTip = req.French.InsiderTip
                },
                new ArticleTranslation
                {
                    Culture = "en-CA",
                    Title = req.English.Title,
                    Slug = req.English.Slug,
                    Description = req.English.Description,
                    Body = req.English.Body,
                    SeoTitle = req.English.SeoTitle,
                    SeoDescription = req.English.SeoDescription,
                    Keywords = req.English.Keywords,
                    InsiderTip = req.English.InsiderTip
                }
            ]
        };

        db.Articles.Add(article);
        await db.SaveChangesAsync(ct);

        await cacheStore.EvictByTagAsync("articles", ct);
        await cacheStore.EvictByTagAsync("trending", ct);

        return Created($"/api/fr-CA/articles/{req.French.Slug}", new
        {
            article.Id,
            FrenchUrl = $"/fr/articles/{req.French.Slug}",
            EnglishUrl = $"/en/articles/{req.English.Slug}"
        });
    }

    // DELETE: api/admin/articles/{id} (Supprimer un article)
    [HttpDelete("articles/{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id, CancellationToken ct)
    {
        var article = await db.Articles.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article is null)
        {
            return NotFound(new { error = $"Article #{id} introuvable." });
        }

        db.Articles.Remove(article);
        await db.SaveChangesAsync(ct);

        await cacheStore.EvictByTagAsync("articles", ct);
        await cacheStore.EvictByTagAsync("trending", ct);

        return Ok(new { message = $"Article #{id} supprimé avec succès." });
    }

    // POST: api/admin/cache/evict (Purger Redis)
    [HttpPost("cache/evict")]
    public async Task<IActionResult> EvictCache(CancellationToken ct)
    {
        await cacheStore.EvictByTagAsync("articles", ct);
        await cacheStore.EvictByTagAsync("trending", ct);
        await cacheStore.EvictByTagAsync("categories", ct);
        return Ok(new { message = "Tous les caches Redis ont été purgés par le contrôleur MVC Admin." });
    }
}