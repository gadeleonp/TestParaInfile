var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar HttpClient como servicio
builder.Services.AddHttpClient();

builder.Services.AddSession(); // Agregar soporte para el manejo de sesiones

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

// Habilitar el uso de sesiones
app.UseSession();

app.UseAuthorization();

// Configurar las rutas predeterminadas para los controladores generados
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuario}/{action=Registro}/{id?}");

app.Run();


