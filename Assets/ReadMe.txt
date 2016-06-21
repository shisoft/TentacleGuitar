
谱面加载： Assets\ExternalCode\BeatMapStuff\ToBeatMapProgrammer.cs

玩家输入： Assets\ExternalCode\InputStuff\ToImputProgrammer.cs


测试代码都在 Assets\ForTest.cs 里，不想用这个测试可以直接删除 ForTest.cs 和 Hierarchy里的 ---- TestOnly ---- 


初步完成了0品，0品音符被定义为不同种类的音符，将NoteType设置为Zero即可添加0品音符。

关于修改游戏模式：
运行后，在Hierarchy里点击Stage，Inspector中最下面有个enum叫PlayMode，改成MouseAndKeyBoard就可以用户鼠标玩了。
每次Start时代码都会修改这个PlayMode，再改一次就行了。
