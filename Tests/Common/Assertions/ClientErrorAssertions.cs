namespace Backend.Tests.Common.Assertions;

public static class ClientErrorAssertions
{
    public static async Task MentionsFieldAsync(HttpResponseMessage response, params string[] expectedFragments)
    {
        Assert.InRange((int)response.StatusCode, 400, 499);
        var body = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));

        var normalizedBody = NormalizeForMatch(body);
        var matchFound = expectedFragments.Any(fragment =>
            normalizedBody.Contains(NormalizeForMatch(fragment), StringComparison.Ordinal));

        Assert.True(
            matchFound,
            $"Expected error body to mention one of [{string.Join(", ", expectedFragments)}], but got: {body}");
    }

    private static string NormalizeForMatch(string input)
    {
        var chars = input.Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray();

        return new string(chars);
    }
}
