using Vinted.Hw.Persistence;
using Vinted.Hw.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
