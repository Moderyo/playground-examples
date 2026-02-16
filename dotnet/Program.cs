// Moderyo .NET Backend Example — ASP.NET Core Minimal API
using Moderyo;
using Moderyo.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var apiKey = Environment.GetEnvironmentVariable("MODERYO_API_KEY") ?? "";
var client = new ModeryoClient(apiKey);

app.MapPost("/api/moderate", async (HttpContext ctx) =>
{
    var body = await ctx.Request.ReadFromJsonAsync<ModerateInput>();
    if (body == null || string.IsNullOrEmpty(body.Input))
        return Results.BadRequest(new { error = "input field is required" });

    try
    {
        var options = new ModerationOptions
        {
            Mode = body.Mode ?? "enforce",
            Risk = body.Risk ?? "balanced",
            PlayerId = body.PlayerId ?? "",
        };

        var request = new ModerationRequest
        {
            Input = body.Input,
            SkipProfanity = body.SkipProfanity,
            SkipThreat = body.SkipThreat,
            SkipMaskedWord = body.SkipMaskedWord,
            LongTextMode = body.LongTextMode,
        };

        var result = await client.ModerateAsync(request, options);

        return Results.Ok(new
        {
            blocked = result.IsBlocked,
            flagged = result.IsFlagged,
            decision = result.PolicyDecision?.DecisionValue,
            reason = result.PolicyDecision?.Reason,
            scores = result.Scores,
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message);
    }
});

app.MapGet("/health", () => new { status = "ok", sdk = "moderyo-dotnet", version = "2.0.7" });

app.Run();

// ─── Input DTO ───
record ModerateInput(
    string Input,
    string? Mode = null,
    string? Risk = null,
    string? PlayerId = null,
    bool SkipProfanity = false,
    bool SkipThreat = false,
    bool SkipMaskedWord = false,
    bool LongTextMode = false
);
