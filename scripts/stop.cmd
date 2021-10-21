@echo off

taskkill /f /IM rankCalculator.exe
taskkill /f /IM eventsLogger.exe
taskkill /f /IM valuator.exe

cd ..\nginx\
nginx -s stop
