using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AzureRAGSystem.Domain.DomainModels;

public class BaseEntity
{
    [Key]
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }
}