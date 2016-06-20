
谱面载入看 ExternalCode\BeatMapStuff\ToBeatMapProgrammer.cs 

玩家输入看 ExternalCode\InputStuff\ToImputProgrammer.cs （还没写）

游戏开始时 Stage.cs 的 Start() 会调用自动读取歌曲并开始随机生成音符，这次把测试音乐也push过去了
不想测试的话就把 Stage.cs 60行 附近标记了TestOnly的代码注释掉

开启测试的话按A D Q E 可以移动摄影机
