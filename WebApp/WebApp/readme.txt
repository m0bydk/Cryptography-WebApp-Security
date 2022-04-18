Commands:
    dotnet ef migrations add InitialDbCreation
    dotnet ef database update
    dotnet aspnet-codegenerator controller -name RSAsController -actions -m RSA -dc ApplicationDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
    
    dotnet ef database drop