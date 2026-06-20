using FinanceService.Infrastructure;
using FinanceService.Infrastructure.Persistence.Repositories;
using FinanceService.Application.Interfaces;
using Shared.Infrastructure;
using Shared.Presentation;
using Shared.Presentation.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(FinanceService.Application.DTO.AllCurrenciesResponse).Assembly))
    .AddAuthorization()
    .AddFinanceData(builder.Configuration)
    .AddSingleton<ICurrencyRepository, CurrencyRepository>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddControllers();

var app = builder.Build();

app.UseSwaggerForDevelopment("Finance");

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();