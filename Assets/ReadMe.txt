
谱面载入 ExternalCode\BeatMapStuff\ToBeatMapProgrammer.cs 

玩家输入 ExternalCode\InputStuff\ToImputProgrammer.cs （还没写）


游戏开始时 ForTest.cs 的 Start() 会调用自动读取歌曲并开始随机生成音符，以后把测试音乐也push过去

不想测试的话就把 ForTest 的 doTest 去钩

开启测试的话按 A D Q E 可以移动摄影机

Stage 100行左右的 #region -------- Public -------- 里的函数都是供外部召唤的工具，现在都改成 static 方法了

