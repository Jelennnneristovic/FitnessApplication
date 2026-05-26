var builder = WebApplication.CreateBuilder(args);

// === CORS (za budući frontend) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// SAMO ZA DEVELOPMENT - prihvati self-signed sertifikate servisa
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) =>
    {
        handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
    });

var app = builder.Build();

app.UseCors("AllowAll");

// Mapiraj reverse proxy
app.MapReverseProxy();

app.Run();