using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;

//url app configuration
string endpoint="https://appconfig7547.azconfig.io";

//azure credential options

DefaultAzureCredentialOptions credentialOptions= new(){
    ExcludeEnvironmentCredential=true,
    ExcludeManagedIdentityCredential=true
};

//crear cliente de appconfig
var builder= new ConfigurationBuilder();

builder.AddAzureAppConfiguration(options=>
{
    options.Connect(new Uri(endpoint), 
    new DefaultAzureCredential(credentialOptions));
});

//obtenemos la configuracion

try{
    var config= builder.Build();
    Console.WriteLine(config["Dev:conStr"]);


}catch(Exception ex){
   Console.WriteLine($"Error al obtener la config {ex.Message}");
}








