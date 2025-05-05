param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

$testName = "${Name}.Tests"

Write-Host "Creating class library: $Name"
dotnet new classlib --name $Name

Write-Host "Creating xUnit test project: $testName"
dotnet new xunit --name $testName

Write-Host "Adding projects to solution"
dotnet sln add "$Name\$Name.csproj"
dotnet sln add "$testName\$testName.csproj"

Write-Host "Referencing $Name from $testName"
dotnet add "$testName\$testName.csproj" reference "$Name\$Name.csproj"


