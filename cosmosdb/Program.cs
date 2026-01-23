using Microsoft.Azure.Cosmos;
using dotenv.net;

string databaseName="myDatabase";
string containerName="myContainer";

//cargar las variables de ambiente .env
DotEnv.Load();
var envVars=DotEnv.Read();
string cosmosDbAccountUrl=envVars["DOCUMENT_ENDPOINT"];
string accountKey=envVars["ACCOUNT_KEY"];

//validación de variables de ambiente
if(string.IsNullOrEmpty(cosmosDbAccountUrl) || string.IsNullOrEmpty(accountKey))
{
   Console.WriteLine("hay que definir las variables de ambiente en el archivo .env");
   return;
}

//Crear cliente de CosmosDB
CosmosClient client=new(
    accountEndpoint: cosmosDbAccountUrl,
    authKeyOrResourceToken: accountKey
);

//operaciones de CosmosDB
try{
    //Crear una base de datos si no existe
    Database database= await client.CreateDatabaseIfNotExistsAsync(databaseName);
    Console.WriteLine($"Base de datos creada o obtenida {database.Id}");

    //Crear un container ("~tabla")
    Container container = await database.CreateContainerIfNotExistsAsync(
        id: containerName,
        partitionKeyPath: "/id"
    );
    Console.WriteLine($"Container creado u obtenido {container.Id}");

    //definimos un producto que vamos a almacenar en CosmosDB
    Product newItem = new Product
    {
        id=Guid.NewGuid().ToString(),
        name="Zote",
        description="jabón multiusos México",
        amount=1
    };

    //añadimos el item en el container
    ItemResponse<Product> createResponse = await container.CreateItemAsync(
        item: newItem,
        partitionKey: new PartitionKey(newItem.id)
    );

    Console.WriteLine($"Item insertado en CosmosDB id: {createResponse.Resource.id}");



}catch(CosmosException ex){
    Console.WriteLine($"db error {ex.StatusCode}-{ex.Message}");

}catch(Exception ex){
   Console.WriteLine($"error inesperado {ex.Message}");
}


//clase que representa un producto
public class Product
{
    public string? id {get; set;}
    public string? name {get; set;}
    public string? description {get; set;}
    public int? amount {get; set;}
}











