using Azure.Messaging.ServiceBus;
using Azure.Identity;

//conexión al servicio
string svcBusNameSpace = "svcbus30123.servicebus.windows.net";
string queueName = "myqueue";

DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

ServiceBusClient client = new(svcBusNameSpace,
    new DefaultAzureCredential(options));


//Creando el producer (sender)

ServiceBusSender sender = client.CreateSender(queueName);

using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();


const int numOfMessages = 3;

for (int i = 1; i <= numOfMessages; i++)
{

    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Mensaje {i}")))
    {
        throw new Exception("El mensaje es muy largo!!!");
    }
}

//Enviar mensajes al queue
try
{
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"un batch de {numOfMessages} mensaje se ha enviado");

}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}
finally
{

    await sender.DisposeAsync();
}





