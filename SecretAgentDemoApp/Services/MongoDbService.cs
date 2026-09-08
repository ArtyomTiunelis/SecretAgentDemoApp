using MongoDB.Driver;
using PromoApp.Api.Models;

namespace PromoApp.Api.Services;

public class MongoDbService
{
    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDb:ConnectionString is not configured.");
        var databaseName = configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException("MongoDb:DatabaseName is not configured.");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        Users = database.GetCollection<User>("users");
        ErrorLogs = database.GetCollection<ErrorLog>("error_logs");
    }

    public IMongoCollection<User> Users { get; }

    public IMongoCollection<ErrorLog> ErrorLogs { get; }
}
