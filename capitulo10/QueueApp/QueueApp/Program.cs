using Azure;
using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;



//conexión a Storage Queue service
string queueName = "myqueue"+Guid.NewGuid().ToString();
string storageAccountName = "storageacc15953";

DefaultAzureCredentialOptions options = new() { 
  ExcludeEnvironmentCredential = true,
  ExcludeManagedIdentityCredential = true
};

QueueClient queueClient = new QueueClient(
    new Uri($"https://{storageAccountName}.queue.core.windows.net/{queueName}"),
    new DefaultAzureCredential(options)
);


//Creando queue
Console.WriteLine($"Creando queue :{queueName}");
await queueClient.CreateAsync();

Console.WriteLine("queue creado, pulsa enter para continuar ...");
Console.ReadLine();


//enviar mensajes
await queueClient.SendMessageAsync("Mensaje 1");
await queueClient.SendMessageAsync("Mensaje 2");

//enviar el mensaje y salvar el mensaje para usarlo después
SendReceipt receipt = await queueClient.SendMessageAsync("Mensaje 3");

Console.WriteLine($"3 Mensaje se han añadido al queue!!!!... enter para continuar...");
Console.ReadLine();


// obtener los mensajes sin eliminar del queue
foreach (var message in (await queueClient.PeekMessagesAsync(maxMessages:10)).Value) 
{ 
    Console.WriteLine($"Data {message.MessageText}");

}

Console.WriteLine("Pulsa ente para continuar .....");
Console.ReadLine();

//actualizar mensaje 3

await queueClient.UpdateMessageAsync(receipt.MessageId, receipt.PopReceipt, "Mensaje 3 Actualizado");

Console.WriteLine("Mensaje 3 actualizado, pulsa enter para continuar ...");
Console.ReadLine();

//validando si la actualización sucedio
foreach (var message in (await queueClient.PeekMessagesAsync(maxMessages: 10)).Value)
{
    Console.WriteLine($"Data {message.MessageText}");

}

Console.WriteLine("Pulsa ente para continuar .....");
Console.ReadLine();


//Eliminando los mensajes del queue

foreach (var message in (await queueClient.ReceiveMessagesAsync(maxMessages:10)).Value) {

    Console.WriteLine($"Procesando mensaje: {message.MessageText}");

    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
}

Console.WriteLine("Mensajes eliminados del queue-");
Console.WriteLine("Pulsa enter para continuar....");
Console.ReadLine();


//Eliminar queue al finalizar
Console.WriteLine($"eliminando queue {queueClient.Name}");
await queueClient.DeleteAsync();




