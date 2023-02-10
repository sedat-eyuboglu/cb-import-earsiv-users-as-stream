# Import Data Into Couchbase From Large XML File in a Zip File
Demonstrates importing data into Couchbase from a XML file in zip file. Reads data from xml as stream to make memory usage stable.

## Publish
To publish as single native windows binary. 
```
dotnet publish -c release -o publish -r win-x64 --self-contained true /p:PublishReadyToRun=true /p:PublishSingleFile=true /p:DebugType=embedded
```
