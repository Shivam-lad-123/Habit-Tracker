#!/usr/bin/env bash
# sessionStart hook — logs session start to GitHub Actions output
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, source, initialPrompt

set -euo pipefail

PAYLOAD=$(cat)
LOGGED_AT=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# Extract sessionId using grep and sed
SESSION_ID=$(echo "$PAYLOAD" | grep -o '"sessionId"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "")
if [ -z "$SESSION_ID" ]; then
  SESSION_ID=$(echo "$PAYLOAD" | grep -o '"session_id"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "")
fi

# Extract source
SOURCE=$(echo "$PAYLOAD" | grep -o '"source"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "copilot")
if [ -z "$SOURCE" ]; then
  SOURCE="copilot"
fi

# Log to GitHub Actions (stdout)
echo "[SESSION START] {\"logged_at\":\"$LOGGED_AT\",\"event\":\"sessionStart\",\"session_id\":\"$SESSION_ID\",\"source\":\"$SOURCE\"}"
