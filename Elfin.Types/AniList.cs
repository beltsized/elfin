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
        [JsonPropertyName("Character")]
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
        public int Id { get; set; }
        public AniListName Name { get; set; }
        public AniListImage Image { get; set; }
        public string Gender { get; set; }
        public string? Age { get; set; }
        public string? BloodType { get; set; }
        public int Favourites { get; set; }
        public string Description { get; set; }
        public string SiteUrl { get; set; }
        public AniListMedia Media { get; set; }
    }

    public class AniListName
    {
        public string Full { get; set; }
    }

    public class AniListImage
    {
        public string? Large { get; set; }
        public string? Medium { get; set; }
    }

    public class AniListMedia
    {
        public List<AniListMediaEdge> Edges { get; set; }
    }

    public class AniListMediaEdge
    {
        public int Id { get; set; }
    }
}