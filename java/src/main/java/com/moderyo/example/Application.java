// Moderyo Java Backend Example — Spring Boot
package com.moderyo.example;

import com.moderyo.ModeryoClient;
import com.moderyo.models.*;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

@SpringBootApplication
@RestController
public class Application {

    private final ModeryoClient client;

    public Application() {
        String apiKey = System.getenv("MODERYO_API_KEY");
        this.client = new ModeryoClient(apiKey != null ? apiKey : "");
    }

    public static void main(String[] args) {
        SpringApplication.run(Application.class, args);
    }

    @PostMapping("/api/moderate")
    public ResponseEntity<?> moderate(@RequestBody ModerateInput input) {
        if (input.input == null || input.input.isEmpty()) {
            return ResponseEntity.badRequest().body(Map.of("error", "input field is required"));
        }

        try {
            ModerationOptions options = ModerationOptions.builder()
                    .mode(input.mode != null ? input.mode : "enforce")
                    .risk(input.risk != null ? input.risk : "balanced")
                    .playerId(input.playerId)
                    .build();

            ModerationRequest request = ModerationRequest.builder()
                    .input(input.input)
                    .skipProfanity(input.skipProfanity)
                    .skipThreat(input.skipThreat)
                    .skipMaskedWord(input.skipMaskedWord)
                    .longTextMode(input.longTextMode)
                    .build();

            ModerationResult result = client.moderate(request, options);

            return ResponseEntity.ok(Map.of(
                    "blocked", result.isBlocked(),
                    "flagged", result.needsReview(),
                    "decision", result.getPolicyDecision() != null ? result.getPolicyDecision().getDecision() : "ALLOW",
                    "reason", result.getPolicyDecision() != null && result.getPolicyDecision().getReason() != null ? result.getPolicyDecision().getReason() : ""
            ));
        } catch (Exception e) {
            return ResponseEntity.internalServerError().body(Map.of("error", e.getMessage()));
        }
    }

    @GetMapping("/health")
    public Map<String, String> health() {
        return Map.of("status", "ok", "sdk", "moderyo-java", "version", "2.0.7");
    }

    // ─── Input DTO ───
    public static class ModerateInput {
        public String input;
        public String mode;
        public String risk;
        public String playerId;
        public boolean skipProfanity;
        public boolean skipThreat;
        public boolean skipMaskedWord;
        public boolean longTextMode;
    }
}
