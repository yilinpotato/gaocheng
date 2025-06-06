## mycppgame
>高级程序语言设计——游戏项目  

我们决定设计一个`肉鸽`类型游戏。  
主要玩法与地图设计借鉴了游戏`undertale`和`以撒的结合`。可以稍微了解一下以增进理解。

游戏流程为：游戏开始界面->游戏开始前->游戏主地图->游戏战斗场景 and 游戏特殊事件场景->游戏结束总结面板

 ***
### 1. 游戏开始界面

* **游戏logo背景**
* **开始游戏按键**
* **设置按键**
* **退出按键**

### 2. 游戏开始前
* **随机种子生成或设置（决定地图，道具获取）**
* **选择武器（如果工作量太大，就还是设计只有一种武器）**
* **抽取随机初始物资**
游戏中主要的参数（物资）有：
`武器`（如果有）（可以改变攻击模式），
`攻速`，
`攻击力`，
`移速`,
`道具`（提供永久特殊效果），
`血量`（为零后游戏结束），
`金钱`（在`游戏特殊事件场景`购买其他物资，在`游戏结束总结面板`中额外算分），

* **选择`游戏难度`**
  
### 3. 游戏主地图



* **随机生成无向图作为地图**
  地图有分为多种房间，每个房间作为无向图的一个节点，节点的边代表玩家可经过此道路。
   >地图包含：初始房间，战斗房间，事件房间，boss房间  

  其中限制所有房间都相互连接（每个节点不孤立）
  每个节点最多有三个边（有特殊要求按照特殊要求）
  战斗房间有5-7个，只有两条或三条边，到达后转到`游戏战斗场景`。
  事件房间有两个，到达后转到`游戏特殊事件场景`。
  boss房间只有1个，且只有一条边，到达后转到敌人为boss的`游戏战斗场景`。

  击败boss后到达下一层，再次重新生成地图并提高数值，设计x（依据`游戏难度`决定）层后游戏通关。
* **可以查看角色道具、数值的可呼出面板**
* **可以实现在此界面的保存并退出（回到`游戏开始界面`）**
  
### 4. 游戏战斗场景
* **战斗机制**
  `wasd`躲避敌人发射的弹幕，`e`释放技能(默认为增加移速，道具可以替换或增强)，`space`发动攻击（有cd，攻击时会降低移速），击败敌人后战斗结束。
* **战斗结束后获得`金钱`，有几率获得`道具`**
* **敌人由`敌人列表`中随机选择，boss由`boss列表`随机选择**
### 5. 游戏特殊事件场景
* 有不同的特殊事件：
  
 | 事件名称 | 描述 | 
| :----- | :------: | 
| 商店 |  用`金钱`换取`道具`(商品时随机的)（价格在固定范围波动）   |  
| 对话  |  一段剧情，获得几个选择，选择带来`特定道具`或一些`特殊效果`   |  
| 特殊战斗  |  与特殊敌人开始特殊战斗   | 
| 。。。  |  。。。   | 
### 6. 游戏结束总结面板
* 统计分数
* 继承一个道具与部分金钱
* test4