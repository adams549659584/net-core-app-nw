# App seed with ASP.NET Core MVC and Angular.

## 重命名项目

修改 `rename.sh` 文件中的 `COMPANY_NAME` 、 `PROJ_NAME` 以及 `CONTEXT_ROOT` ， 然后再 bash 命令行中执行。

## 数据库

依次执行 `database` 目录下的脚 SQL 本， 创建数据库， 并修改 `server/src/NetCoreApp.Api/hibernate.config` 和 `server/smart-code.yml` 中的数据库连接串。

## 运行与测试

1. 在命令行中打开 `server/test/NetCoreApp.Test` ， 输入 `dotnet test` ， 并查看测试结果， 应该能看到 `Test Run Successful.` 的输出；
2. 在命令行中打开 `server/src/NetCoreApp.Api` ， 输入 `dotnet watch run` ， 看到如下输出：

   ```
   Now listening on: http://localhost:5000
   Now listening on: https://localhost:5001
   Application started. Press Ctrl+C to shut down.
   ```

3. 打开浏览器， 浏览 `http://localhost:5000/swagger` 即可看到 swagger api 界面。

## 代码生成

1. 克隆 `https://github.com/beginor/SmartCode.git` 至本地目录， 并打开命令行， 切换到 `src/SmartCode.CLI` 目录；
2. 复制 `server/smart-code.yml` 的完整路径；
3. 根据业务需求创建数据表；
3. 在第一步的窗口中输入命令 `dotnet run <上一步复制的配置文件地址>`;

## 建议使用的工具

- VSCode 
