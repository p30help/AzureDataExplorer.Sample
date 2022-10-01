# AzureDataExplorer.Sample
A simple project by **.net6** for connecting with Azure Data Explorer database and run CRUD operations on it

**Before run the project you must do these steps:**

1- Create Azure Data Explorer Cluter in your azure portal

2- Create a database for your new cluster

There are different ways for connecting to Azure Data Explorer database but I used AppKey way so:

3- Register an application in Azure Active Directory (for more inforemation read https://learn.microsoft.com/en-us/azure/data-explorer/provision-azure-ad-app)

4- Grant your registered application access to the Azure Data Explorer database with running this query `.add database <DatabaseName> admins ('aadapp=<ApplicationId>')` 

**Notes:**
- You can find values for `AppClientId` and `AppTenantId` in *Azure portal* -> *Azure Active Directory* -> *App registrations* page
- You can value for `AppClientSecret` in *App registrations* -> *Certificates & secrets* page
- You can find values for `KustoUri` and `IngestUri` in *Azure Explorer Data* cluster page 

--------------------
## Helpfull links:
- https://learn.microsoft.com/en-us/azure/data-explorer/provision-azure-ad-app
- https://learn.microsoft.com/en-us/azure/data-explorer/net-sdk-ingest-data
- https://learn.microsoft.com/en-us/azure/data-explorer/create-cluster-database-portal
- https://www.youtube.com/watch?v=fSR_qCIFZSA
- https://www.youtube.com/playlist?list=PLWf6TEjiiuID2MN-xqN520aaNkzWMgsbr
