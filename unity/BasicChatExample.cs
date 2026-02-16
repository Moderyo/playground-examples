// Moderyo Unity Example — Basic Chat Moderation
//
// Unity projeleri Docker'da çalışmaz, bu yüzden bu dizin
// sadece referans kod örnekleri içerir.
//
// Unity SDK'yı projenize eklemek için:
// 1. Unity Package Manager → Add package from git URL
// 2. https://github.com/moderyo/moderyo-unity.git
// 3. Moderyo > Settings'den API key'inizi girin

using UnityEngine;
using Moderyo;

/// <summary>
/// Basit chat moderasyonu örneği.
/// Herhangi bir GameObject'e ekleyin ve ModeryoConfig referansını atayın.
/// </summary>
public class BasicChatExample : MonoBehaviour
{
    [SerializeField] private ModeryoConfig config;

    private ModeryoClient _client;

    void Start()
    {
        _client = new ModeryoClient(config);
        Debug.Log("Moderyo client initialized");
    }

    // Async kullanım
    public async void CheckMessageAsync(string message)
    {
        var result = await _client.ModerateAsync(message);

        if (result.IsBlocked())
        {
            Debug.Log($"BLOCKED: {result.PolicyDecision?.Reason}");
        }
        else if (result.IsFlagged())
        {
            Debug.Log($"FLAGGED: Toxicity={result.Scores?.Toxicity:F2}");
        }
        else
        {
            Debug.Log("Message is safe");
        }
    }

    // Coroutine kullanım (eski Unity uyumluluğu)
    public void CheckMessageCoroutine(string message)
    {
        var options = new ModerationOptions
        {
            Mode = "enforce",
            Risk = "conservative",
            PlayerId = "player_42"
        };

        var operation = _client.Moderate(message, options);
        StartCoroutine(WaitForResult(operation));
    }

    private System.Collections.IEnumerator WaitForResult(ModerationOperation operation)
    {
        yield return operation;

        if (operation.HasError)
        {
            Debug.LogError($"Error: {operation.Error}");
            yield break;
        }

        var result = operation.Result;
        Debug.Log($"Decision: {result.PolicyDecision?.DecisionValue}");
        Debug.Log($"Scores: toxicity={result.Scores?.Toxicity:F2}, violence={result.Scores?.Violence:F2}");

        // Tetiklenen kategorileri göster
        var triggered = result.Categories?.GetTriggered();
        if (triggered != null && triggered.Count > 0)
        {
            Debug.Log($"Triggered categories: {string.Join(", ", triggered)}");
        }
    }
}
