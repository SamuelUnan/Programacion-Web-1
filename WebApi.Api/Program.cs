using WebApi.Interface;
using WebApi.Implementation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddSingleton<IEstudianteService, EstudianteService>();
builder.Services.AddScoped<IMaestroService, MaestroService>();
builder.Services.AddScoped<IMateriaService, MateriaService>();
builder.Services.AddScoped<IFacturaService, FacturaService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
