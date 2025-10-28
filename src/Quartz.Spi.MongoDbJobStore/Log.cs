namespace Quartz.Spi.MongoDbJobStore;

public static class Log
{
    private static Serilog.ILogger logger = Serilog.Log.ForContext<MongoDbJobStore>();

    public static void Verbose(string messageTemplate, params object[] propertyValues)
    {
        logger.Verbose(messageTemplate, propertyValues);
    }

    public static void Debug(string messageTemplate, params object[] propertyValues)
    {
        logger.Debug(messageTemplate, propertyValues);
    }

    public static void Information(string messageTemplate)
    {
        logger.Information(messageTemplate);
    }

    public static void Information(string messageTemplate, params object[] propertyValues)
    {
        logger.Information(messageTemplate, propertyValues);
    }

    public static void Warning(string messageTemplate, params object[] propertyValues)
    {
        logger.Warning(messageTemplate, propertyValues);
    }

    public static void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.Error(exception, messageTemplate, propertyValues);
    }
}