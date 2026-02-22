<?php
/**
 * Moderyo PHP Backend Example — Slim Framework
 */
require __DIR__ . '/vendor/autoload.php';

use Moderyo\ModeryoClient;
use Moderyo\ModeryoConfig;
use Slim\Factory\AppFactory;
use Psr\Http\Message\ResponseInterface as Response;
use Psr\Http\Message\ServerRequestInterface as Request;

$app = AppFactory::create();
$app->addBodyParsingMiddleware();

$apiKey = getenv('MODERYO_API_KEY') ?: '';
$client = new ModeryoClient(new ModeryoConfig(['apiKey' => $apiKey]));

$app->post('/api/moderate', function (Request $request, Response $response) use ($client) {
    $body = $request->getParsedBody();
    $input = $body['input'] ?? '';

    if (empty($input)) {
        $response->getBody()->write(json_encode(['error' => 'input field is required']));
        return $response->withStatus(400)->withHeader('Content-Type', 'application/json');
    }

    try {
        $result = $client->moderate($input, [
            'mode' => $body['mode'] ?? 'enforce',
            'risk' => $body['risk'] ?? 'balanced',
            'playerId' => $body['player_id'] ?? '',
            'skipProfanity' => $body['skip_profanity'] ?? false,
            'skipThreat' => $body['skip_threat'] ?? false,
            'skipMaskedWord' => $body['skip_masked_word'] ?? false,
            'longTextMode' => $body['long_text_mode'] ?? false,
        ]);

        $data = [
            'blocked' => $result->isBlocked,
            'flagged' => $result->isFlagged,
            'decision' => $result->policyDecision->decision ?? null,
            'reason' => $result->policyDecision->reason ?? null,
            'scores' => $result->scores ?? null,
        ];

        $response->getBody()->write(json_encode($data));
        return $response->withHeader('Content-Type', 'application/json');
    } catch (\Exception $e) {
        $response->getBody()->write(json_encode(['error' => $e->getMessage()]));
        return $response->withStatus(500)->withHeader('Content-Type', 'application/json');
    }
});

$app->post('/api/moderate/batch', function (Request $request, Response $response) use ($client) {
    $body = $request->getParsedBody();
    $inputs = $body['inputs'] ?? [];

    if (empty($inputs) || !is_array($inputs)) {
        $response->getBody()->write(json_encode(['error' => 'inputs array is required']));
        return $response->withStatus(400)->withHeader('Content-Type', 'application/json');
    }

    try {
        $result = $client->moderateBatch($inputs, [
            'mode' => $body['mode'] ?? 'enforce',
            'risk' => $body['risk'] ?? 'balanced',
        ]);

        $items = [];
        foreach ($result->results as $r) {
            $items[] = [
                'blocked' => $r->isBlocked,
                'flagged' => $r->isFlagged,
                'decision' => $r->policyDecision->decision ?? null,
                'reason' => $r->policyDecision->reason ?? null,
            ];
        }

        $data = [
            'total' => count($result->results),
            'blockedCount' => count($result->getBlocked()),
            'flaggedCount' => count($result->getFlagged()),
            'results' => $items,
        ];

        $response->getBody()->write(json_encode($data));
        return $response->withHeader('Content-Type', 'application/json');
    } catch (\Exception $e) {
        $response->getBody()->write(json_encode(['error' => $e->getMessage()]));
        return $response->withStatus(500)->withHeader('Content-Type', 'application/json');
    }
});

$app->get('/health', function (Request $request, Response $response) {
    $response->getBody()->write(json_encode([
        'status' => 'ok',
        'sdk' => 'moderyo-php',
        'version' => '2.0.7',
    ]));
    return $response->withHeader('Content-Type', 'application/json');
});

$app->run();
