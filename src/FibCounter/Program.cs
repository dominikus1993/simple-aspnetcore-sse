using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDoc();
builder.Services.AddFastEndpoints();

var app = builder.Build();


app.UseCors(b =>
{
    b.AllowAnyHeader();
    b.AllowAnyMethod();
    b.AllowAnyOrigin();
});
app.UseAuthorization();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerGen();

app.Run();

