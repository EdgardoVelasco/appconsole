### Crear un Azure Container Registry
az acr create --resource-group RGAZ204 --name acrvefe --sku Basic 

### Comando para crear la imagen dentro del ACR
az acr build --image clase/appstatic:1.0.0 --registry acrvefe --file Dockerfile .