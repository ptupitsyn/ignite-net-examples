# Peer assembly loading with Docker-based server

1. Run server with:

```
docker run -v `pwd`/config/:/ignite-cfg gridgain/community-dotnet:8.7.24 -ConfigFileName=/ignite-cfg/peer.xml
```

2. Run client with:

```
cd ComputeTest
dotnet run
``` 
