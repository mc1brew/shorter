namespace Kvin.Shorter
{
    public static class Constants
    {
        public static class ConfigKeys
        {
            //References the connection string for Mongo
            public static readonly string MongoConnectionString = "MongoConnectionString";

            //The address to route to when look up fails.  This should be the location to suggest adding an address.
            public static readonly string AddUrlPage = "AddUrlPage";
        }
    }
}