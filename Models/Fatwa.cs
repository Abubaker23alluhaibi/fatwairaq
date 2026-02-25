using System.Text.Json.Serialization;

namespace MyMauiApp.Models;

public class Fatwa
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Question { get; set; }

    /// <summary>الـ API يرجع المحتوى في حقل "content"</summary>
    [JsonPropertyName("content")]
    public string? Answer { get; set; }

    [JsonPropertyName("category")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("scholar")]
    public string? ScholarName { get; set; }
    public DateTime? Date { get; set; }
}

