namespace API.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        // ove sve opcije parametre u appsettings.json, ubaceno je u appsettings.json da ne bi islo na github jer imamo key-eve, na github nam ide appsettings.Development.json 
    }
}
