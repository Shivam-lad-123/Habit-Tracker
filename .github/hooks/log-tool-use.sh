#!/usr/bin/env bash
# preToolUse hook — logs every tool execution
# Logs to: logs/tool-executions.jsonl
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, toolName, toolArgs

set -euo pipefail

PAYLOAD=$(cat)
LOGGED_AT=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

# Extract fields using python3 or jq
if command -v python3 &>/dev/null; then
  TOOL_NAME=$(echo "$PAYLOAD" | python3 -c "import sys, json; d = json.load(sys.stdin); print(d.get('toolName', d.get('tool_name', 'unknown')))" 2>/dev/null || echo "unknown")
  SESSION_ID=$(echo "$PAYLOAD" | python3 -c "import sys, json; d = json.load(sys.stdin); print(d.get('sessionId', d.get('session_id', '')))" 2>/dev/null || echo "")
elif command -v jq &>/dev/null; then
  TOOL_NAME=$(echo "$PAYLOAD" | jq -r '.toolName // .tool_name // "unknown"')
  SESSION_ID=$(echo "$PAYLOAD" | jq -r '.sessionId // .session_id // ""')
else
  TOOL_NAME="unknown"
  SESSION_ID=""
fi

mkdir -p logs

# Log: {logged_at, event, session_id, tool_name, raw_payload}
printf '{"logged_at":"%s","event":"preToolUse","session_id":"%s","tool_name":"%s","raw":%s}\n' \
  "$LOGGED_AT" "$SESSION_ID" "$TOOL_NAME" "$PAYLOAD" >> logs/tool-executions.jsonl
