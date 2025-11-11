using System.Text.Json.Serialization;
using ServiceUtils.Serialization;

namespace ServiceUtils.Tests.Dto;

public class NewObjDto
{
    [JsonConverter(typeof(FlexibleObjectConverter))]
    public object? Value { get; set; }
}