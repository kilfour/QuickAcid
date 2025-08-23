# Path where your repositories are located
$reposRoot = "C:\Code"

# Path where logs will be stored
$logRoot = "C:\Code\RepoLogs"

# Ensure log directory exists
if (!(Test-Path -Path $logRoot)) {
    New-Item -ItemType Directory -Path $logRoot | Out-Null
}

# Create unique log file with timestamp
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$logFile = Join-Path $logRoot "git-pull-$timestamp.log"

# Start logging
"Git pull started at $(Get-Date)" | Tee-Object -FilePath $logFile

# Loop through all subfolders
Get-ChildItem -Path $reposRoot -Directory | ForEach-Object {
    $repoPath = $_.FullName
    if (Test-Path (Join-Path $repoPath ".git")) {
        "---- Repo: $repoPath ----" | Tee-Object -FilePath $logFile -Append
        try {
            Push-Location $repoPath
            git pull 2>&1 | Tee-Object -FilePath $logFile -Append
        }
        finally {
            Pop-Location
        }
        "" | Tee-Object -FilePath $logFile -Append
    }
    else {
        "Skipping $repoPath (not a git repo)" | Tee-Object -FilePath $logFile -Append
    }
}

"Git pull completed at $(Get-Date)" | Tee-Object -FilePath $logFile -Append

Write-Host "Done. Log file saved to $logFile"