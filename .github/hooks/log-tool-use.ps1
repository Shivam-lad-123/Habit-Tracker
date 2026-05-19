$ErrorActionPreference = 'Stop'

# preToolUse hook — logs every tool execution
# Logs to: logs/tool-executions.jsonl
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, toolName, toolArgs

$Payload = $Input | Out-String
$LoggedAt = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

try {
    $Data = $Payload | ConvertFrom-Json
    $ToolName = if ($Data.toolName) { $Data.toolName } elseif ($Data.tool_name) { $Data.tool_name } else { "unknown" }
    $SessionId = if ($Data.sessionId) { $Data.sessionId } elseif ($Data.session_id) { $Data.session_id } else { "" }
} catch {
    $ToolName = "unknown"
    $SessionId = ""
}

$null = New-Item -ItemType Directory -Force -Path "logs"

# Log: {logged_at, event, session_id, tool_name, raw_payload}
$LogEntry = "{`"logged_at`":`"$LoggedAt`",`"event`":`"preToolUse`",`"session_id`":`"$SessionId`",`"tool_name`":`"$ToolName`",`"raw`":$Payload}"
Add-Content -Path "logs/tool-executions.jsonl" -Value $LogEntry
