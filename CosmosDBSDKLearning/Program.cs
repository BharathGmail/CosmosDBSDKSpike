using System;
using System.Configuration;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace CosmosDBSDKLearning
{
    class Program
    {
        static Uri collectionURI = UriFactory.CreateDocumentCollectionUri("myDB", "myStore");
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var cosmosdbendpoint = configuration["CosmodDBEndPoint"];
            var cosmosdbkey = configuration["CosmosDBMasterKey"];
            // CreateCollection(cosmosdbendpoint, cosmosdbkey);
            //  CreateDocumentsFromTypes(cosmosdbendpoint, cosmosdbkey);
            QueryDocuments(cosmosdbendpoint, cosmosdbkey);
            Console.ReadLine();
        }


        static void CreateCollection(string cosmosdbendpoint, string cosmosdbkey)
        {
            Uri dburi = UriFactory.CreateDatabaseUri("myDB");
            using (var client = new DocumentClient(new Uri(cosmosdbendpoint), cosmosdbkey))
            {
                IList<DocumentCollection> dblist = client.CreateDocumentCollectionQuery(dburi).ToList();

                foreach (DocumentCollection db in dblist)
                {
                    Console.WriteLine(string.Format("db id = {0}, db rid = {1}", db.Id, db.ETag));
                }
            }


        }


        static void ListDatabases(string cosmosdbendpoint, string cosmosdbkey)
        {
            IList<Database> dblist = null;
            using (var client = new DocumentClient(new Uri(cosmosdbendpoint), cosmosdbkey))
            {
                dblist = client.CreateDatabaseQuery().ToList();

                foreach (Database db in dblist)
                {
                    Console.WriteLine(string.Format("db id = {0}, db rid = {1}", db.Id, db.ResourceId));
                }
            }
        }


        async static void CreateDocuments(string cosmosdbendpoint, string cosmosdbkey)
        {
            IList<Database> dblist = null;
            dynamic DocumentA = new
            {
                id = "9999",
                name = "Lacha",
                address = new
                {
                    adressType = "Main Office",
                    addressLine1 = "1050 Oak Street",
                    location = new
                    {
                        city = "Chennai",
                        stateProvinceName = "TamilNadu"
                    },
                    postalCode = "600004",
                    countryRegionName = "India"
                }
            };
            dynamic DocumentB = new
            {
                id = "8888",
                name = "Chiruvuri",
                address = new
                {
                    adressType = "Branch Office",
                    addressLine1 = "1050 Ebony Street",
                    location = new
                    {
                        city = "Hyderebad",
                        stateProvinceName = "Telengana"
                    },
                    postalCode = "500050",
                    countryRegionName = "India"
                }
            };
            using (var client = new DocumentClient(new Uri(cosmosdbendpoint), cosmosdbkey))
            {
                var documentA = await client.CreateDocumentAsync(collectionURI, DocumentA);
                var documentB = await client.CreateDocumentAsync(collectionURI, DocumentB);
                var resourceA = documentA.Resource;
                var resourceB = documentB.Resource;
                Console.WriteLine("document a = " + resourceA + "\n" + "\n");
                Console.WriteLine("document b = " + resourceB + "\n" + "\n");
                return;
            }
        }

        async static void CreateDocumentsFromTypes(string cosmosdbendpoint, string cosmosdbkey)
        {
            Customer customer = new Customer
            {
                Id = "1001",
                Name = "Vaanchinaathan",
                Address = new Address
                {
                    AddressType = "Side Office",
                    AddressLine1 = "123 Madurai Street",
                    Location = new Location
                    {
                        City = "Madurai",
                        StateProvinceName = "TamilNadu"
                    },
                    PostalCode = "65341",
                    CountryRegionName = "India"

                }

            };

            using (var client = new DocumentClient(new Uri(cosmosdbendpoint), cosmosdbkey))
            {
                var documentA = await client.CreateDocumentAsync(collectionURI, customer);


                var resourceA = documentA.Resource;

                Console.WriteLine("document a = " + resourceA + "\n" + "\n");
            }

            return;
        }
        async static void QueryDocuments(string cosmosdbendpoint, string cosmosdbkey)
        {
            string sqlquery = "SELECT * from c where c.name = 'Vaanchinaathan'";
            var options = new FeedOptions { EnableCrossPartitionQuery = true };
            using (var client = new DocumentClient(new Uri(cosmosdbendpoint), cosmosdbkey))
            {
                IList<Customer> customers =  client.CreateDocumentQuery<Customer>(collectionURI,sqlquery,options).ToList();
                foreach (Customer c in customers)
                {
                    Console.WriteLine("Customer Name = " +   c.Name);
                    Console.WriteLine("\n City = " + c.Address.Location.City);
                }
                
               // var resourceA = documentA.Resource;

               // Console.WriteLine("document a = " + resourceA + "\n" + "\n");
            }

            return;
        }
       

    }
}
