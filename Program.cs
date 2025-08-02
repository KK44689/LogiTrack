var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<LogiTrackContext>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // TestController is a custom controller for testing purposes
    var testController = new TestController();

    // Call test methods to demonstrate functionality
    // Console.WriteLine("Running TestDisplayInfo methods...");
    // testController.TestDisplayInfo();

    // Console.WriteLine("Running TestGetOrderSummary methods...");
    // testController.TestGetOrderSummary();

    // Console.WriteLine("Running TestRemoveItem methods...");
    // testController.TestRemoveItem();

    // Console.WriteLine("Running TestDatabaseContext methods...");
    // testController.TestDatabaseContext();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();