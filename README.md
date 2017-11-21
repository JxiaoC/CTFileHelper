#CtFile

>非官方API，慎用。

程序内置了打码平台（dama2.com），每次使用之前需要初始化城通网盘的账号数据和打码平台的账号数据。

登录方法(Login)中已经做了缓存，如果之前已经登录过，且登录未失效，将会直接返回之前的登录状态，否则将会进行自动登录；登录的验证码交由打码平台完成。

`new DaMa.DaMaTuInit(username, password);`

`new Login(username, password);`

>登录成功后,程序将会把cookie写入`PublicConfig.Cookie`中,并且往本地的Cache文件夹写入缓存文件

###创建文件夹
>在使用之前,需要先初始化数据

`string Data = Folder.Create("FolderName", "描述");`


###通过文件夹名称获取文件夹ID
>在使用之前,需要先初始化数据

`int ID = Folder.GetFolderID("2017-04-06");`


###上传文件到文件夹
>在使用之前,需要先初始化数据

`new UpFile().Up("d:\\temp.jpg", "文件夹名称");`