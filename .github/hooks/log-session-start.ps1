$ErrorActionPreference = 'Stop'

# sessionStart hook — logs session start to GitHub Actions output
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

# Log to GitHub Actions (stdout)
$LogEntry = "{`"logged_at`":`"$LoggedAt`",`"event`":`"sessionStart`",`"session_id`":`"$SessionId`",`"source`":`"$Source`"}"
Write-Host "[SESSION START] $LogEntry"
