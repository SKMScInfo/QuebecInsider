using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Domain;

namespace QuebecInsider.Api.Data;

public sealed class QuebecInsiderDbContext(DbContextOptions<QuebecInsiderDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleTranslation> ArticleTranslations => Set<ArticleTranslation>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryTranslation> CategoryTranslations => Set<CategoryTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasIndex(category => category.Key)
            .IsUnique();

        modelBuilder.Entity<CategoryTranslation>()
            .HasIndex(translation => new { translation.Culture, translation.Slug })
            .IsUnique();

        modelBuilder.Entity<ArticleTranslation>()
            .HasIndex(translation => new { translation.Culture, translation.Slug })
            .IsUnique();

        modelBuilder.Entity<Article>()
            .HasMany(article => article.Translations)
            .WithOne(translation => translation.Article)
            .HasForeignKey(translation => translation.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasMany(category => category.Translations)
            .WithOne(translation => translation.Category)
            .HasForeignKey(translation => translation.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
