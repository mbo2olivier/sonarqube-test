using Serilog;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// --- Lab 8 : Simulation d'un "Leaked Secret" ---
// ATTENTION : Ne jamais faire cela en production !
const string GOOGLE_API_KEY = "AIzaSyA1234567890-fake-google-key";

// --- Configuration Serilog ---
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.MapGet("/", () => 
{
    Log.Information("Accès à la racine avec la clé : {Key}", GOOGLE_API_KEY);
    
    var data = new { Message = "Lab 8 : Quality Gate", Status = "Vulnerable" };
    // Utilisation d'une version ancienne de Newtonsoft.Json (v11.0.1)
    string json = JsonConvert.SerializeObject(data);
    
    return Results.Content($@"
        <html>
            <body style='font-family: sans-serif; text-align: center; padding-top: 50px;'>
                <h1 style='color: #d93025;'>CNSS AppSec Training - Lab 8</h1>
                <p>Cette application contient des vulnérabilités critiques détectables par pipeline.</p>
                <code style='background: #eee; padding: 10px;'>{json}</code>
            </body>
        </html>", "text/html");
});

// Endpoint simulant une vulnérabilité de type Path Traversal pour SonarQube
app.MapGet("/api/files", (string fileName) => 
{
    // VULNÉRABILITÉ : Aucune validation du nom de fichier (Path Traversal)
    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
    if (!File.Exists(path)) return Results.NotFound();
    
    var content = File.ReadAllText(path);
    return Results.Text(content);
});

app.Run();
