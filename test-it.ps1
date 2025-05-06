$projects = Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -like "*Tests*.csproj" }

foreach ($proj in $projects) {
    Write-Host "`n== Running tests for $($proj.Name) =="
    dotnet test $proj.FullName --logger "console;verbosity=detailed"
}
