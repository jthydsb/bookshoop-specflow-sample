    using System;

    namespace BookShop.AcceptanceTests.Support
{
    public class ConfigurationDriver
    {
        public string Mode => "Integrated"; //Environment.GetEnvironmentVariable("Mode");
    }
}