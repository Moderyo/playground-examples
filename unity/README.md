# Unity Example

Unity projeleri Docker container'da çalışmaz. Bu dizin, Unity SDK'nın nasıl kullanıldığını gösteren **referans kod örnekleri** içerir.

## Kurulum

1. Unity projenizde **Package Manager** açın
2. **Add package from git URL** seçin
3. URL: `https://github.com/moderyo/moderyo-unity.git`
4. **Moderyo > Settings** menüsünden API key'inizi girin

## Dosyalar

- `BasicChatExample.cs` — Async ve Coroutine ile basit chat moderasyonu

## SDK Özellikleri

- 27 kategori desteği
- Async + Coroutine dual API
- `ModeryoConfig` ScriptableObject ile yapılandırma
- `ChatFilter`, `UsernameValidator`, `ReportSystem` hazır bileşenleri
- Offline mod desteği (bağlantı kesilirse)
- Unity Editor test penceresi (Moderyo > Test Window)

Detaylı dokümantasyon: [moderyo-unity](https://github.com/moderyo/moderyo-unity)
