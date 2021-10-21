@echo off

SET VALUATOR_PATH=%~dp0..\Valuator\
SET RANK_CALCULATOR_PATH=%~dp0..\RankCalculator\
SET NGINX_PATH=%~dp0..\nginx\

start /d%VALUATOR_PATH% dotnet run --no-build --urls "http://localhost:5001"
start /d%VALUATOR_PATH% dotnet run --no-build --urls "http://localhost:5002"
start /d%RANK_CALCULATOR_PATH% dotnet run --no-build
start /d%RANK_CALCULATOR_PATH% dotnet run --no-build
start /d%NGINX_PATH% nginx.exe
