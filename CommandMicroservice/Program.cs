
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRazorPages();
builder.Services.AddHostedService<MQTTService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();
app.UseRouting();

app.UseCors("CORS");

app.MapRazorPages();
app.MapHub<EventHub>("/eventHub");

app.Run();
