namespace Pinger.Application.Records;

public record EndPointResponseRecord<T>(int StatusCode, T? Payload=default, string? Message=null);
