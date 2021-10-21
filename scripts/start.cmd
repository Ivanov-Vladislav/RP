@echo off

SET VALUATOR_PATH=%~dp0..\Valuator\
SET RANK_CALCULATOR_PATH=%~dp0..\RankCalculator\
SET EVENTS_LOGGER_PATH=%~dp0..\EventsLogger\
SET NGINX_PATH=%~dp0..\nginx\

%VALUATOR_PATH% dotnet build
%RANK_CALCULATOR_PATH% dotnet build
%EVENTS_LOGGER_PATH% dotnet build

start /d %VALUATOR_PATH% dotnet run --no-build --urls "http://localhost:5001"
start /d %VALUATOR_PATH% dotnet run --no-build --urls "http://localhost:5002"

start /d %RANK_CALCULATOR_PATH% dotnet run --no-build
start /d %RANK_CALCULATOR_PATH% dotnet run --no-build

start /d %EVENTS_LOGGER_PATH% dotnet run --no-build
start /d %EVENTS_LOGGER_PATH% dotnet run --no-build

start /d %NGINX_PATH% nginx.exe
