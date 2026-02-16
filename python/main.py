"""
Moderyo Python Backend Example
FastAPI + moderyo SDK
"""
import os
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from moderyo import Moderyo

app = FastAPI(title="Moderyo Python Example", version="1.0.0")

client = Moderyo(api_key=os.getenv("MODERYO_API_KEY", ""))


class ModerateRequest(BaseModel):
    input: str
    mode: str = "enforce"
    risk: str = "balanced"
    player_id: str = ""
    skip_profanity: bool = False
    skip_threat: bool = False
    skip_masked_word: bool = False
    long_text_mode: bool = False
    long_text_threshold: float = 0.3


@app.post("/api/moderate")
async def moderate(req: ModerateRequest):
    try:
        result = client.moderate(
            req.input,
            mode=req.mode,
            risk=req.risk,
            player_id=req.player_id or None,
            skip_profanity=req.skip_profanity,
            skip_threat=req.skip_threat,
            skip_masked_word=req.skip_masked_word,
            long_text_mode=req.long_text_mode,
            long_text_threshold=req.long_text_threshold,
        )

        return {
            "blocked": result.is_blocked,
            "flagged": result.is_flagged,
            "decision": result.policy_decision.decision if result.policy_decision else None,
            "reason": result.policy_decision.reason if result.policy_decision else None,
            "scores": result.scores.model_dump() if result.scores else None,
            "detected_phrases": [
                {"text": p.text, "label": p.label}
                for p in (result.detected_phrases or [])
            ],
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))


@app.get("/health")
async def health():
    return {"status": "ok", "sdk": "moderyo-python", "version": "2.0.7"}


if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=4002)
