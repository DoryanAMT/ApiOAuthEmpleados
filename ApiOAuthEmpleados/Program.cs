using ApiOAuthEmpleados.Data;
using ApiOAuthEmpleados.Helpers;
using ApiOAuthEmpleados.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionServicesOAuth helper =
    new HelperActionServicesOAuth(builder.Configuration);
//  ESTA INSTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ
//  PARA QUE NUESTRA APLICACION PUEDA VALIDA CON TODO LO QUE HA CREADO
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
//  HABILITAMOS LA SEGURIDAD UTILIZANDO LA CLASE HELPER
builder.Services.AddAuthentication(helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<RepositoryHospitals>();
string connectionString = builder.Configuration.GetConnectionString("SqlAzure");
builder.Services.AddDbContext<HospitalContext>
    (options => options.UseSqlServer(connectionString));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ApiEmpleadosOAuth");
        options.RoutePrefix = "";
    });

app.UseAuthentication();
//  DEBEMOS RESPETAR EL ORDEN DE LA CONFIGURACION 
app.UseAuthorization();

app.MapControllers();

app.Run();
