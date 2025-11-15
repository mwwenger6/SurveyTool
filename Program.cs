using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SurveyTool.Services;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

//Packages installed:
//  Swashbuckle.AspNetCore.SwaggerGen,
//  Microsoft.EntityFrameworkCore.InMemory,
//  Swashbuckle.AspNetCore.SwaggerUI

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services.AddDbContext<SurveyDbContext>(opt => opt.UseInMemoryDatabase("SurveyDb"));
builder.Services.AddScoped<SurveyDataTools>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Survey API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();
