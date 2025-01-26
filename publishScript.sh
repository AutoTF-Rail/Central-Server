dotnet publish -c Release -o out
docker login 172.17.0.8:5001
docker build -t 172.17.0.8:5001/repository/docker-autotf/central-server:latest .
docker push 172.17.0.8:5001/repository/docker-autotf/central-server:latest
