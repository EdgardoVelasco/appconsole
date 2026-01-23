using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;


Console.WriteLine("Azure Storage Account");

//crear autenticación por defecto az login
DefaultAzureCredentialOptions options=new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true

};

//esperar los resultados del procesamiento de nuestro BlobStorage
await ProcessAsync();



Console.WriteLine("\n pulsa enter para terminar la ejecución");
Console.ReadLine();

async Task ProcessAsync(){
    //crear el cliente de Blob
    string accountName="storageacc31232";
    DefaultAzureCredential credential=new DefaultAzureCredential(options);
    string blobServiceEndpoint=$"https://{accountName}.blob.core.windows.net";
    BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), credential);

    //crear contenedor
    string containerName="wtblob" + Guid.NewGuid().ToString();
    Console.WriteLine($"Creando container .... {containerName}");
    BlobContainerClient containerClient= await blobServiceClient.CreateBlobContainerAsync(containerName);

    // Check if the container was created successfully
    if (containerClient != null)
    {
        Console.WriteLine("Container created successfully, press 'Enter' to continue.");
        Console.ReadLine();
    }
    else
    {
        Console.WriteLine("Failed to create the container, exiting program.");
        return;
    }

    //Crear un archivo para cargarlos a nuestro BlobStorage
    Console.WriteLine("Creando archivo local......");
    string localPath="./data/";
    string fileName="testf"+Guid.NewGuid().ToString()+".txt";
    string localFilePath=Path.Combine(localPath, fileName);

    await File.WriteAllTextAsync(localFilePath, "bienvenidos a la clase de Azure");
    Console.WriteLine("Archivo creado ...... Enter para continuar....");
    Console.ReadLine();

    //cargar el archivo en BlobStorage
    BlobClient blobClient= containerClient.GetBlobClient(fileName);

    Console.WriteLine("Archivo cargandose a BlobStorage .....");

    using(FileStream uploadFileStream = File.OpenRead(localFilePath)){

        await blobClient.UploadAsync(uploadFileStream);
        uploadFileStream.Close();
    }

    bool blobExists = await blobClient.ExistsAsync();

    if(blobExists){
       Console.WriteLine("Archivo cargado a BlobStorage exitosamente, enter para continuar....");
       Console.ReadLine();
    }else{
        Console.WriteLine("Error al cargar el archivo, enter para terminar....");
        return;
    }

    // Listar Blobs del container en el Storage Account
    Console.WriteLine("listando archivos del container .....");
    await foreach(BlobItem item in containerClient.GetBlobsAsync()){
       Console.WriteLine("\t"+ item.Name);
    }

    Console.WriteLine("Pulsa enter para continuar ....");
    Console.ReadLine();
    
}


