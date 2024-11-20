using Microsoft.EntityFrameworkCore;
using WebArchiver;
using WebArchiver.Data;
using WebArchiver.Data.Repository;
using WebArchiver.Interfaces;
using WebArchiver.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvcCore();

builder.Services.AddScoped<IPageService,PageService>();

builder.Services.AddDbContext<PagesContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("pages"));
});
builder.Services.AddScoped<IPagesRepository,PagesRepository>();
builder.Services.AddScoped<IStylesRepository,StylesRepository>();


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
app.UseStaticFiles();
app.UseDefaultFiles();


//  Migrate in code 
using var scope = app.Services.CreateScope(); // create a scope for this
var services = scope.ServiceProvider;
var context = services.GetRequiredService<PagesContext>(); // get the db context from the scope service
try
{
    await context.Database.MigrateAsync(); //apply migration if pending
}
catch(Exception ex)
{

}

app.Run();
