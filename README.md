# SQLHelper

[![Build status](https://ci.appveyor.com/api/projects/status/h687ovca9m893ww1?svg=true)](https://ci.appveyor.com/project/JaCraig/sqlhelper)

SQLHelper is a simple class to help with running queries against a database.

## Basic Usage

The system relies on an IoC wrapper called [Canister](https://github.com/JaCraig/Canister). While Canister has a built in IoC container, it's purpose is to actually wrap your container of choice in a way that simplifies setup and usage for other libraries that don't want to be tied to a specific IoC container. SQLHelper uses it to detect and pull in various info. As such you must set up Canister in order to use SQLHelper:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
                    .RegisterSQLHelper()
                    .Build();
					
This is required prior to using the SQLHelper class for the first time. Once Canister is set up, you can use the SQLHelper class:

    var Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();
    var Instance = new SQLHelper(Configuration, SqlClientFactory.Instance, "ConnectionString");
	
The SQLHelper class takes in a IConfiguration class, a DbProviderFactory class, and the connection string. The connection string can just be the name of a connection string in your configuration object. Once an instance is set up, you can create a batch, add queries, and then execute them.

    var Results = Instance.CreateBatch()
                		   .AddQuery(CommandType.Text,"SELECT * FROM [TestDatabase].[dbo].[TestTable]")
                		   .AddQuery(CommandType.Text,"SELECT * FROM [TestDatabase].[dbo].[TestTable2]")
                		   .AddQuery(CommandType.Text,"SELECT * FROM [TestDatabase].[dbo].[TestTable3]")
                		   .Execute();
						   
The Results object then holds the results for all 3 queries and is returned as IList<IList<dynamic>>. So in order to get the results from the queries:

    var FirstQueryResults = Results[0];
	var SecondQueryResults = Results[1];
	var ThirdQueryResults = Results[2];

It is also possible to convert the results from the dynamic type to a class type that you specify:

    var TestTableClasses = FirstQueryResults.Select(x => (TestTableClass)x).ToList();
	
The type will be converted automatically for you with no special type conversion required.  SQLHelper also has an ExecuteScalar function:

    var Result = Instance.ExecuteScalar<int>();
	
This will either return the first value of the first set of results OR it will return the number of rows that were effected depending on whether or not the query was a select or not.

## Installation

The library is available via Nuget with the package name "SQLHelper.DB". To install it run the following command in the Package Manager Console:

Install-Package SQLHelper.DB

## Build Process

In order to build the library you will require the following as a minimum:

1. Visual Studio 2015 with Update 3
2. .Net Core 1.0 SDK

Other than that, just clone the project and you should be able to load the solution and build without too much effort.