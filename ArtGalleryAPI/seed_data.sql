-- Seed Data for Aboriginal Art Gallery Database
-- Run these queries to verify the data after running the application

-- View all art types
SELECT * FROM "ArtTypes";

-- View all artists
SELECT * FROM "Artists";

-- View all artifacts with relationships
SELECT
    a."Id",
    a."Title",
    a."Medium",
    a."Price",
    art."FirstName" || ' ' || art."LastName" AS "ArtistName",
    at."Name" AS "ArtTypeName"
FROM "Artifacts" a
JOIN "Artists" art ON a."ArtistId" = art."Id"
JOIN "ArtTypes" at ON a."ArtTypeId" = at."Id";

-- View available artifacts
SELECT * FROM "Artifacts" WHERE "IsAvailable" = true;

-- View artifacts by region
SELECT
    art."Title",
    art."Region"
FROM "Artifacts" a
JOIN "Artists" art ON a."ArtistId" = art."Id"
WHERE art."Region" LIKE '%Northern Territory%';

-- View total value of artworks per artist
SELECT
    ar."FirstName" || ' ' || ar."LastName" AS "ArtistName",
    COUNT(a."Id") AS "TotalWorks",
    COALESCE(SUM(a."Price"), 0) AS "TotalValue"
FROM "Artists" ar
LEFT JOIN "Artifacts" a ON ar."Id" = a."ArtistId"
GROUP BY ar."Id", ar."FirstName", ar."LastName";
