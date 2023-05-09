var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// [START gae_flex_dotnet_static_files]
app.UseDefaultFiles();
app.UseStaticFiles();
// [END gae_flex_dotnet_static_files]

app.MapGet("/", () => "Hello World!");
app.Run();
