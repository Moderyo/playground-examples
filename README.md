# Moderyo Playground Examples

Official example integrations for the [Moderyo](https://moderyo.com) Content Moderation API.

Each folder contains a fully runnable example showing how to integrate Moderyo into your application.

## Languages

| Language | Folder | SDK Package |
|----------|--------|-------------|
| .NET / C# | [dotnet/](dotnet/) | [![NuGet](https://img.shields.io/nuget/v/Moderyo.svg)](https://www.nuget.org/packages/Moderyo) |
| Node.js / TypeScript | [nodejs/](nodejs/) | `@moderyo/sdk` |
| Python | [python/](python/) | `moderyo` |
| Go | [go/](go/) | `github.com/moderyo/moderyo-go` |
| Java / Spring | [java/](java/) | `com.moderyo:moderyo-java` |
| PHP / Laravel | [php/](php/) | `moderyo/moderyo-php` |
| Unity | [unity/](unity/) | [Moderyo Unity SDK](https://github.com/moderyo/moderyo-unity) |

## Quick Start

Every example follows the same pattern:

1. Set your API key as environment variable: `MODERYO_API_KEY`
2. Run the example
3. Send a POST request to `/moderate` with `{ "input": "your text" }`

### Run with Docker

Each example includes a `Dockerfile`:

```bash
cd dotnet/
docker build -t moderyo-example-dotnet .
docker run -e MODERYO_API_KEY=your-key -p 3000:3000 moderyo-example-dotnet
```

### Test

```bash
curl -X POST http://localhost:3000/moderate \
  -H "Content-Type: application/json" \
  -d '{"input": "Hello, this is a test message"}'
```

## SDK Repositories

| SDK | Repository |
|-----|------------|
| .NET | [moderyo/moderyo-dotnet](https://github.com/moderyo/moderyo-dotnet) |
| Node.js | [moderyo/moderyo-js](https://github.com/moderyo/moderyo-js) |
| Python | [moderyo/moderyo-python](https://github.com/moderyo/moderyo-python) |
| Go | [moderyo/moderyo-go](https://github.com/moderyo/moderyo-go) |
| Java | [moderyo/moderyo-java](https://github.com/moderyo/moderyo-java) |
| PHP | [moderyo/moderyo-php](https://github.com/moderyo/moderyo-php) |
| Unity | [moderyo/moderyo-unity](https://github.com/moderyo/moderyo-unity) |

## License

MIT License - see [LICENSE](LICENSE) for details.
