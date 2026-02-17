// Moderyo Go Backend Example — Chi router
package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"

	moderyo "github.com/moderyo/moderyo-go/v2"
)

type ModerateInput struct {
	Input             string  `json:"input"`
	Mode              string  `json:"mode,omitempty"`
	Risk              string  `json:"risk,omitempty"`
	PlayerID          string  `json:"player_id,omitempty"`
	SkipProfanity     bool    `json:"skip_profanity,omitempty"`
	SkipThreat        bool    `json:"skip_threat,omitempty"`
	SkipMaskedWord    bool    `json:"skip_masked_word,omitempty"`
	LongTextMode      bool    `json:"long_text_mode,omitempty"`
	LongTextThreshold float64 `json:"long_text_threshold,omitempty"`
}

func main() {
	apiKey := os.Getenv("MODERYO_API_KEY")
	if apiKey == "" {
		log.Fatal("MODERYO_API_KEY is required")
	}

	client := moderyo.NewClient(apiKey)
	mux := http.NewServeMux()

	mux.HandleFunc("POST /api/moderate", func(w http.ResponseWriter, r *http.Request) {
		var input ModerateInput
		if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
			http.Error(w, `{"error":"invalid JSON"}`, http.StatusBadRequest)
			return
		}
		if input.Input == "" {
			http.Error(w, `{"error":"input field is required"}`, http.StatusBadRequest)
			return
		}

		opts := &moderyo.ModerationOptions{
			Mode:     input.Mode,
			Risk:     input.Risk,
			PlayerID: input.PlayerID,
		}

		req := &moderyo.ModerationRequest{
			Input:          input.Input,
			SkipProfanity:  &input.SkipProfanity,
			SkipThreat:     &input.SkipThreat,
			SkipMaskedWord: &input.SkipMaskedWord,
			LongTextMode:   &input.LongTextMode,
		}

		result, err := client.ModerateWithOptions(r.Context(), req, opts)
		if err != nil {
			w.WriteHeader(http.StatusInternalServerError)
			json.NewEncoder(w).Encode(map[string]string{"error": err.Error()})
			return
		}

		respData := map[string]interface{}{
			"blocked": result.IsBlocked(),
			"flagged": result.IsFlagged(),
			"scores":  result.Scores,
		}
		if result.PolicyDecision != nil {
			respData["decision"] = result.PolicyDecision.Decision
			respData["reason"] = result.PolicyDecision.Reason
		}
		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(respData)
	})

	mux.HandleFunc("GET /health", func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(map[string]string{
			"status":  "ok",
			"sdk":     "moderyo-go",
			"version": "2.0.7",
		})
	})

	port := os.Getenv("PORT")
	if port == "" {
		port = "4004"
	}
	fmt.Printf("Go example server running on http://localhost:%s\n", port)
	log.Fatal(http.ListenAndServe(":"+port, mux))
}
