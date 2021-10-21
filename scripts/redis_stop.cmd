@echo off

reg delete "HKCU\Environment" /v DB_RUS /f
reg delete "HKCU\Environment" /v DB_EU /f
reg delete "HKCU\Environment" /v DB_OTHER /f

taskkill /f /im redis-server.exe