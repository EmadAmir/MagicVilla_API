using MagicVilla_Web;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(MappingConfig));
//Adding HttpClient on VillaService
builder.Services.AddHttpClient<IVillaService, VillaService>();
//Registering VillaService to dependency injection
//AddScoped means
//For a single request it will have one object of VillaService
//even if it is requested 10 times it will use the same object lifetime
builder.Services.AddScoped<IVillaService, VillaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
