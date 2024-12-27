dotnet publish -c Release -o out
docker build -t 172.17.0.3:5001/repository/docker-autotf/central-server:latest .
docker push 172.17.0.3:5001/repository/docker-autotf/central-server:latest
