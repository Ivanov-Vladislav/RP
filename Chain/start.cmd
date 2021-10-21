cd chain

dotnet build

start "first" dotnet run --no-build 8000 localhost 8001 true
start "second" dotnet run --no-build 8001 localhost 8002
start "third" dotnet run --no-build 8002 localhost 8003
start "fourth" dotnet run --no-build 8003 localhost 8004
start "fifth" dotnet run --no-build 8004 localhost 8005
start "sixth" dotnet run --no-build 8005 localhost 8000