using System.Text.Json.Serialization;

namespace Elfin.Types
{
    public class AniListResponse
    {
        [JsonPropertyName("data")]
        public AniListData Data { get; set; }
    }

    public class AniListData
    {
        [JsonPropertyName("characters")]
        public AniListCharacters? Characters { get; set; }
        [JsonPropertyName("character")]
        public AniListCharacter? Character { get; set; }
    }

    public class AniListCharacters
    {
        [JsonPropertyName("results")]
        public AniListCharacterResult[] Results { get; set; }
    }

    public class AniListCharacterResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class AniListCharacter
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public AniListName Name { get; set; }
        [JsonPropertyName("image")]
        public AniListImage Image { get; set; }
        [JsonPropertyName("gender")]
        public string Gender { get; set; }
        [JsonPropertyName("age")]
        public string? Age { get; set; }
        [JsonPropertyName("bloodType")]
        public string? BloodType { get; set; }
        [JsonPropertyName("favourites")]
        public int Favourites { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("siteUrl")]
        public string SiteUrl { get; set; }
        [JsonPropertyName("media")]
        public AniListMedia Media { get; set; }
    }

    public class AniListName
    {
        [JsonPropertyName("full")]
        public string Full { get; set; }
    }

    public class AniListImage
    {
        [JsonPropertyName("large")]
        public string? Large { get; set; }
        [JsonPropertyName("medium")]
        public string? Medium { get; set; }
    }

    public class AniListMedia
    {
        [JsonPropertyName("edges")]
        public List<AniListMediaEdge> Edges { get; set; }
    }

    public class AniListMediaEdge
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}