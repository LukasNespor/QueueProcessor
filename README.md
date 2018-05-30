# Queue processor in ACI
> Note: Docker container used with conjunction with article https://www.nespor.cloud

This [Docker container](https://hub.docker.com/r/112567/queueprocessor) can be hosted in Azure Container Instances (ACI). 
It connects to existing Storage Account and create some queue messages (for testing purpose) 
and process them later.

## How to use
To use commands starting with ``az`` you have to install [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) which is available for all platforms.

```
az login  
az group create -g containers -l westeurope
az group deployment create --template-file path-to\template.json -g containers --parameters instanceName=unique-aci-name accountName=storage-account-name accountKey=storage-account-key queueName=name
az container show -n unique-aci-name -g containers -o table
```

Then you can delete the container
```
az container delete -n unique-aci-name -g containers
```

> Note: ``-g`` is alias for ``--resource-group`` and ``-n`` for ``--name``