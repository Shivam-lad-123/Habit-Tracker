#!/usr/bin/env bash
# preToolUse hook — logs tool execution to GitHub Actions output
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, toolName, toolArgs

set -euo pipefail

PAYLOAD=$(cat)
LOGGED_AT=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# Extract toolName using grep and sed
TOOL_NAME=$(echo "$PAYLOAD" | grep -o '"toolName"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "")
if [ -z "$TOOL_NAME" ]; then
  TOOL_NAME=$(echo "$PAYLOAD" | grep -o '"tool_name"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "unknown")
fi
if [ -z "$TOOL_NAME" ]; then
  TOOL_NAME="unknown"
fi

# Extract sessionId
SESSION_ID=$(echo "$PAYLOAD" | grep -o '"sessionId"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "")
if [ -z "$SESSION_ID" ]; then
  SESSION_ID=$(echo "$PAYLOAD" | grep -o '"session_id"[[:space:]]*:[[:space:]]*"[^"]*"' | head -1 | grep -o ':"[^"]*"' | sed 's/:"//;s/"$//' || echo "")
fi

# Log to GitHub Actions (stdout)
echo "[TOOL USE] {\"logged_at\":\"$LOGGED_AT\",\"event\":\"preToolUse\",\"session_id\":\"$SESSION_ID\",\"tool_name\":\"$TOOL_NAME\",\"raw\":$PAYLOAD}"
