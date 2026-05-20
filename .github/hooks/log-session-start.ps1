$ErrorActionPreference = 'Stop'

# sessionStart hook — records when a new agent session begins
# Logs to: .github/logs/sessions.log
# Receives JSON via stdin with fields: sessionId, timestamp, cwd, source, initialPrompt

$Payload = $Input | Out-String
$LoggedAt = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

try {
    $Data = $Payload | ConvertFrom-Json
    $SessionId = if ($Data.sessionId) { $Data.sessionId } elseif ($Data.session_id) { $Data.session_id } else { "" }
    $Source = if ($Data.source) { $Data.source } else { "copilot" }
} catch {
    $SessionId = ""
    $Source = "copilot"
}

$null = New-Item -ItemType Directory -Force -Path ".github/logs"

# Log: {logged_at, event, session_id, source}
$LogEntry = "{`"logged_at`":`"$LoggedAt`",`"event`":`"sessionStart`",`"session_id`":`"$SessionId`",`"source`":`"$Source`"}"
Add-Content -Path ".github/logs/sessions.log" -Value $LogEntry
