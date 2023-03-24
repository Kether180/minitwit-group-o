using Minitwit7.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using System;



using Prometheus;


var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();


// Use Prometheus middleware.
// Capture metrics about all received HTTP requests.

app.UseHttpMetrics();
app.UseMetricServer();

app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    // Enable the /metrics page to export Prometheus metrics.
    //http://localhost:9090/
    //Open http://localhost:9090/metrics to see the metrics.


    endpoints.MapMetrics();
});


using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    while (!dbContext.Database.CanConnect())
    {
        logger.LogInformation("Waiting for database...");
        Thread.Sleep(1000);
    }
    dbContext.Database.EnsureCreated();
}


app.UseAuthorization();

app.MapControllers();

app.Run();


