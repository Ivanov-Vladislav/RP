@echo off

SET APP_PATH=%~dp0..\Valuator\
SET NGINX_PATH=%~dp0..\nginx\

start /d%APP_PATH% dotnet run --no-build --urls "http://localhost:5001"
start /d%APP_PATH% dotnet run --no-build --urls "http://localhost:5002"

start /d%NGINX_PATH% nginx.exe
