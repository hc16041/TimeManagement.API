using TimeManagement.Infrastructure;
using TimeManagement.Application;
using TimeManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Capas limpias
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Importante para tu Frontend Angular)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});


var app = builder.Build();

// =================================================================
// 2. EJECUTAR SEEDING (ESTO ES LO QUE FALTABA)
// =================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Llamamos al método Seed que creamos antes
        DbInitializer.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}
// =================================================================

// 3. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// AGREGA ESTO: Redirección de la raíz a Swagger
app.MapGet("/", async context =>
{
    context.Response.Redirect("/swagger/index.html");
    await Task.CompletedTask;
});

app.UseCors("AngularClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
