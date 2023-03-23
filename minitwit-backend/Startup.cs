// using Minitwit7.data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Npgsql.EntityFrameworkCore;
// using Microsoft.AspNetCore.Builder;
// using System;

// using Prometheus;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();

// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<DataContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
// );

// // Add Prometheus middleware.
// builder.Services.AddMetrics();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseExceptionHandler("/Error");
// }

// app.UseStaticFiles();

// app.UseRouting();

// // Use Prometheus middleware.
// app.UseHttpMetrics();
// app.UseMetricServer();

// app.UseAuthorization();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllers();
// });

// using (var scope = app.Services.CreateScope())
// {
//     var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataContext>>();
//     var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
//     while (!dbContext.Database.CanConnect())
//     {
//         logger.LogInformation("Waiting for database...");
//         Thread.Sleep(1000);
//     }
//     dbContext.Database.EnsureCreated();
// }

// app.Run();