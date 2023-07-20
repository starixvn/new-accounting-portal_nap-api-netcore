cd D:\SourceCode\Tools-Git\NAP.AuthWebApi
docker build -t napauthapi:1.0 .

docker run -d -p 8095:80 -p 5000:5000 --name napauthapi napauthapi:1.0 

