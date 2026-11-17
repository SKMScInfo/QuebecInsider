-- ============================================================================
-- QUEBECINSIDER — DATABRICKS DELTA LAKEHOUSE (ARCHITECTURE MEDALLION)
-- Complémentaire au Data Warehouse Snowflake (OLAP BI)
-- ============================================================================

-- 1. COUCHE BRONZE : Ingestion brute temps réel depuis .NET 10 / RabbitMQ
CREATE TABLE IF NOT EXISTS  workspace.default.bronze_article_views (
    article_id INT,
    culture STRING,
    slug STRING,
    viewed_at_utc TIMESTAMP
) USING DELTA;

-- 2. COUCHE SILVER : Nettoyage, déduplication et enrichissement temporel
CREATE OR REPLACE TABLE main.default.silver_article_views
USING DELTA AS
SELECT
    article_id,
    UPPER(TRIM(culture)) AS culture_code,
    LOWER(TRIM(slug)) AS slug,
    viewed_at_utc,
    DATE_TRUNC('hour', viewed_at_utc) AS view_hour_utc
FROM  workspace.default.bronze_article_views
WHERE article_id IS NOT NULL AND slug IS NOT NULL;

-- 3. COUCHE GOLD : Agrégation analytique & Feature Store pour recommandation IA
CREATE OR REPLACE TABLE main.default.gold_trending_features
USING DELTA AS
SELECT
    article_id,
    slug,
    culture_code,
    COUNT(*) AS total_views,
    MAX(viewed_at_utc) AS last_viewed_at_utc
FROM main.default.silver_article_views
GROUP BY article_id, slug, culture_code
ORDER BY total_views DESC;