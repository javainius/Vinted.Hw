using Vinted.Hw.Persistence;
using Vinted.Hw.Persistence.Interfaces;
using Vinted.Hw.Services;
using Vinted.Hw.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

string accumulatedDiscountTermFile = configuration["RepositoryPaths:AccumulatedDiscountTermRepository"];
string freeShipmentTermFile = configuration["RepositoryPaths:FreeShipmentTermRepository"];
string shippingPriceFile = configuration["RepositoryPaths:ShippingPriceRepository"];
string transactionRepositoryFile = configuration["RepositoryPaths:TransactionRepository"];


string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), accumulatedDiscountTermFile);

builder.Services.AddScoped<IAccumulatedDiscountTermRepository>(provider =>
    new AccumulatedDiscountTermRepository(accumulatedDiscountTermFile));
builder.Services.AddScoped<IFreeShipmentTermRepository>(provider =>
    new FreeShipmentTermRepository(freeShipmentTermFile));
builder.Services.AddScoped<IShippingPriceRepository>(provider =>
    new ShippingPriceRepository(shippingPriceFile));
builder.Services.AddScoped<ITransactionRepository>(provider =>
    new TransactionRepository(transactionRepositoryFile));
builder.Services.AddScoped<IParsingService, ParsingService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
