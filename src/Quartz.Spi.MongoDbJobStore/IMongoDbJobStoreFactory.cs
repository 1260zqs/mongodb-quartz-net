using MongoDB.Driver;

public interface IMongoDbJobStoreFactory
{
    public IMongoDatabase GetDatabase();
}