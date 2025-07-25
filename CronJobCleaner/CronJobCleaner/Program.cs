using CronJobCleaner.Data;
using CronJobCleaner.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ?? Veritaban� servisi (InMemory)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("LogDB"));

// ?? BackgroundService ekleniyor
builder.Services.AddHostedService<CleanupService>();

// ?? Swagger ve Controller ayarlar�
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ?? Test verisi ekleniyor
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(db);
}

// ?? HTTP pipeline ayarlar�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
