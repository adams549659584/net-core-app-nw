FROM mcr.microsoft.com/dotnet/runtime-deps:7.0
ENV ASPNETCORE_PATHBASE=/net-core-app \
    ASPNETCORE_URLS=http://+:5000 \
    TZ=Asia/Shanghai
ADD dist /net-core-app
VOLUME [ "/net-core-app/config", "/net-core-app/log", "/net-core-app/wwwroot" ]
WORKDIR /net-core-app
ENTRYPOINT ["./NetCoreApp"]
