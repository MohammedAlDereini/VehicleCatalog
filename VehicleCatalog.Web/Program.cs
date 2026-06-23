using VehicleCatalog.Handler.DI;

var builder = WebApplication.CreateBuilder(args);

// Presentation
builder.Services.AddControllersWithViews();
builder.Services.AddResponseCompression();
builder.Services.AddHealthChecks();

// CQRS handlers, mediator, validation, and the NHTSA client
builder.Services.AddHandlerServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapHealthChecks("/health");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
