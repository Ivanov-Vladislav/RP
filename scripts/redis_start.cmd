@echo off

setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

start redis-server
start redis-server --port 6000
start redis-server --port 6001
start redis-server --port 6002 