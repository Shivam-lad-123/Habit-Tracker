#!/usr/bin/env bash
# sessionStart hook — records when a new agent session begins
# Logs to: logs/sessions.log
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, source, initialPrompt

set -euo pipefail

PAYLOAD=$(cat)
LOGGED_AT=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# Extract fields using python3 or jq
if command -v python3 &>/dev/null; then
  SESSION_ID=$(echo "$PAYLOAD" | python3 -c "import sys, json; d = json.load(sys.stdin); print(d.get('sessionId', d.get('session_id', '')))" 2>/dev/null || echo "")
  SOURCE=$(echo "$PAYLOAD" | python3 -c "import sys, json; d = json.load(sys.stdin); print(d.get('source', 'copilot'))" 2>/dev/null || echo "copilot")
elif command -v jq &>/dev/null; then
  SESSION_ID=$(echo "$PAYLOAD" | jq -r '.sessionId // .session_id // ""')
  SOURCE=$(echo "$PAYLOAD" | jq -r '.source // "copilot"')
else
  SESSION_ID=""
  SOURCE="copilot"
fi

mkdir -p logs

# Log: {logged_at, event, session_id, source}
printf '{"logged_at":"%s","event":"sessionStart","session_id":"%s","source":"%s"}\n' \
  "$LOGGED_AT" "$SESSION_ID" "$SOURCE" >> logs/sessions.log
