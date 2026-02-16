import express from 'express';
import { Moderyo } from '@moderyo/sdk';
import dotenv from 'dotenv';

dotenv.config();

const app = express();
app.use(express.json());

// ─── Initialize Moderyo Client ───
const client = new Moderyo({
  apiKey: process.env.MODERYO_API_KEY || '',
});

// ─── POST /api/moderate ───
app.post('/api/moderate', async (req, res) => {
  try {
    const { input, mode, risk, playerId, skipProfanity, skipThreat, skipMaskedWord, longTextMode, longTextThreshold } = req.body;

    if (!input) {
      return res.status(400).json({ error: 'input field is required' });
    }

    const result = await client.moderate(
      {
        input,
        skipProfanity,
        skipThreat,
        skipMaskedWord,
        longTextMode,
        longTextThreshold,
      },
      { mode, risk, playerId }
    );

    return res.json({
      blocked: result.action === 'block',
      flagged: result.action === 'flag' || result.flagged,
      decision: result.policyDecision?.decision,
      reason: result.policyDecision?.reason,
      scores: result.scores,
      detectedPhrases: result.detectedPhrases,
      categories: result.categories,
    });
  } catch (err: unknown) {
    const message = err instanceof Error ? err.message : 'Unknown error';
    return res.status(500).json({ error: message });
  }
});

// ─── GET /health ───
app.get('/health', (_req, res) => {
  res.json({ status: 'ok', sdk: 'moderyo-js', version: '2.0.7' });
});

// ─── Start ───
const PORT = process.env.PORT || 4001;
app.listen(PORT, () => {
  console.log(`Node.js example server running on http://localhost:${PORT}`);
  console.log('POST /api/moderate — Moderate text content');
  console.log('GET  /health      — Health check');
});
