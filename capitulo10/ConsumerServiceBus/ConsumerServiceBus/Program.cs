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


//Código del Consumer
ServiceBusProcessor processor = client.CreateProcessor(queueName);

// timeout 
const int idleTimeOutMs = 3000;
System.Timers.Timer timer = new (idleTimeOutMs);

timer.Elapsed += async (s, e) =>
{
    Console.WriteLine($"no se ha recibido los mensajes ");
    await processor.StopProcessingAsync();
};


//add handler para procesar los mensajes
try
{
    //registro de función que se encarga de obtener los mensajes
    processor.ProcessMessageAsync += MessageHandler;


    processor.ProcessErrorAsync += ErrorHandler;

    //comenzamos a procesar los mensajes
    timer.Start();

    await processor.StartProcessingAsync();
    Console.WriteLine($"Procesador de mensajes iniciado");

    while (processor.IsProcessing) {
        await Task.Delay(500);
    }

    timer.Stop();

    Console.WriteLine("se detiene la obtención de mensajes");



}
finally {
    await processor.DisposeAsync();
}






//método que obtiene cada mensaje
async Task MessageHandler(ProcessMessageEventArgs args) { 

    string body = args.Message.Body.ToString();

    Console.WriteLine($"Mensaje Recibido {body}");

    //reiniciar el timer por cada mensaje 
    timer.Stop();
    timer.Start();

    //al obtener el mensaje que se elimine del queue
    await args.CompleteMessageAsync(args.Message);
}

Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

