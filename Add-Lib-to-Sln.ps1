param(
    [Parameter(Mandatory = $true)]
    [string]$Name
)

Write-Host "Creating class library: $Name"
dotnet new classlib --name $Name

Write-Host "Adding projects to solution"
dotnet sln add "$Name\$Name.csproj"



