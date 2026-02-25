using Turbo.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR for real-time Turbo Streams
builder.Services.AddSignalR();

// Add Turbo Stream Broadcaster service
builder.Services.AddTurboStreamBroadcaster();

// Add Razor Pages for view rendering
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map SignalR Hub for Turbo Streams
app.MapTurboStreamsHub("/hubs/turbo-streams");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
