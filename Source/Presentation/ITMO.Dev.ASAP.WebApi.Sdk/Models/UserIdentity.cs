namespace ITMO.Dev.ASAP.WebApi.Sdk.Models;

public record UserIdentity(string Token, DateTime ExpirationDateTime);