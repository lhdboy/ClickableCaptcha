using WebDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICaptchaService, ClickableCaptchaService>();

builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
