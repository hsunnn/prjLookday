using Microsoft.EntityFrameworkCore;
using prjLookday.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<LookdaysContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("lookdaysConnection")
));

//�K�[CORS�A��
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder => builder
        .WithOrigins("https://localhost:7221")
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// �K�[����A��
builder.Services.AddControllers();

var app = builder.Build();

//�ҥ�CORS����
app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Home/Error");
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); 

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();

