# EHentaiAPI
.Net porting implementation of EhViewer

# 注意
此项目是从[EhViewer](https://github.com/seven332/EhViewer)(实际采用[NekoInverter](https://gitlab.com/NekoInverter/EhViewer)版本)项目，里面的EH相关请求API移植并独立出来的。<br>
此项目将用于Wbooru的新的插件项目,如果可以我也会继续维护以及开发相关功能。

# 如何食用？
[![](https://img.shields.io/badge/nuget-EHentaiAPI%20-blue)](https://www.nuget.org/packages/EHentaiAPI)
* [API接口](https://github.com/MikiraSora/EHentaiAPI/blob/master/EHentaiAPI/Client/EhClient.cs#L92)
* [实际使用](https://github.com/MikiraSora/EHentaiAPI/blob/master/EHentaiAPI.TestConsole/Program.cs)
* 也提供部分接口的单元测试来确保此轮子基本功能是否正常

# 计划
* 优化调用过程
* 将部分接口的常量改成枚举形式
* 留出更多的可定制的接口
* 整更多的接口(如果Wbooru需要)
