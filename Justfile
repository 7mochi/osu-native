test:
    dotnet test -c Release

fmt:
    dotnet format

fmt-check:
    dotnet format --verify-no-changes
