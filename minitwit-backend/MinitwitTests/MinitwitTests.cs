
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore.Sqlite;
using Xunit;
using Minitwit7.Controllers;
using Minitwit7.data;


public class DatabaseFixture
{

    private readonly SQLiteConnection connection;
    private readonly SimulatorController simCon;
    public DatabaseFixture(SimulatorController simCon)
    {
        this.simCon = simCon;
        // Setup
        this.connection = new SQLiteConnection("Data Source =:memory:");
        this.connection.Open();
    }

    public DataContext CreateContext()
    {
        var result = new DataContext(new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(this.connection)
            .Options);
        result.Database.EnsureDeleted();
        result.Database.EnsureCreated();
        return result;
    }


}

public class MinitwitTests : IDisposable
{
    private readonly DataContext context;
    private readonly SimulatorController simCon;

    public MinitwitTests()
    {
        var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseInMemoryDatabase(databaseName: "MiniTwitDatabase");

            var dbContextOptions = builder.Options;
            context = new TestDataContext(dbContextOptions);
            // Delete existing db before creating a new one
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
    }

    private static T GetObjectResultContent<T>(ActionResult<T> result)
    {
        return (T) ((ObjectResult) result.Result).Value;
    }

    public void Dispose()
    {
        // Teardown
        context.Dispose();
    }


}