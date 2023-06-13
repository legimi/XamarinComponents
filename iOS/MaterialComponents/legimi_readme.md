Zbudowane z .net sdk: 7.0.202

Aby zbudować paczkę należy uruchomić w tym folderze komendę 
``` 
dotnet cake --target=externals
```

a następnie
```
dotnet pack source/MaterialComponents/MaterialComponents.csproj --configuration Release --output nuget -p:packageVersion=92.1.0-legimi-maui
```