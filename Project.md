# DeepNetDungeon
# 程序集结构
* WTools：WaterCylinder工具类，存放工具类代码，依赖UnityEngine
* Scripts：基础代码，依赖UnityEngine和WTools
* EntityBehaiviors：实体行为代码，存放Entity类的子类，依赖Scripts
* Effects：效果代码，存放Effect的子类，依赖Scripts
## 基类：
### Entity
    实体类
    一般游戏内交互的对象都继承自实体类
### Block
    砖块类
    不包含实体类的通用属性，只包含基础属性或者仅响应物理效果的实体
    比如地图上的获得块，部分陷阱等。
### Item
    物品类
    可以变成掉落物，或者依附到实体上
### Effect
    效果类
    效果是依附在实体上的对象，基于依附的实体产生某些特定的事件。
# 文件结构：

# 游戏设定：

## 实体属性

实体通用的属性
* 生命值上限MaxHealth
* 生命值Health
* 护甲值Armor
* 护盾值Shield
* 减伤倍率ReductionRatio
* 攻击力Attack
* 攻击速度AttackSpeed
* 暴击率Crit
* 暴击倍率CritRatio
* 错误指数Error
- 一些属性解释：
护甲值：通过计算得出护甲减伤比例，可以先按照数值减少，那之后按比例减少普通伤害，无法减伤特殊伤害和真实伤害。护甲减伤比例为
    （护甲值 + 平均伤害值）/ 平均伤害值
其中平均伤害值在全局参数里定义，根据游戏内容进行平衡性调整。
护盾值：除了穿甲伤害之外，优先扣除护盾值
减伤倍率：固定减伤倍率，-100%-100%之间，可以按比例减少除真实伤害之外的所有伤害。
暴击倍率：基础为1.5
错误指数：错误指数越高，越容易发生错误事件。大部分道具都有错误事件相应的效果
## 玩家
1. 操作
玩家操作分为移动、普通攻击和技能。在不同的平台上有不同的具体实现。
例如在键鼠中，玩家的移动可以由鼠标完全代理，左键普通攻击，右键移动，鼠标中键释放技能。
只有携带特定的技能道具时才能使用技能。
2. 玩家基本属性
玩家继承实体的通用属性，除此之外还有独立的属性：
分数Score：意义不是很大的数值，用于整体流程体现
组件数量ComponentCount：玩家携带的组件数量，相当于金币数。
## 伤害类型
一个伤害对象拥有n个tag，伤害计算时根据tag有不同的效果
* 普通伤害Normal：没有特殊效果的伤害。
* 特殊伤害Super：无视护甲值的伤害。
* 护甲伤害Armor：对护甲值造成损伤，不同于效果造成的护甲值减少，这将直接永久降低基础护甲值。
* 护盾伤害ShieldOnly：只对护甲造成伤害。
* 穿甲伤害ShieldCross：无视护盾值的伤害。
* 真实伤害True：无视护甲值和减伤倍率的伤害。
* 错误伤害Error：根据错误指数浮动伤害。
* 暴击伤害Crit：计算过暴击倍率的暴击伤害。
* 弹幕伤害Bullet：弹幕伤害。
* 接触伤害Touch：实体接触伤害。
## 物品
物品分为三种，拾取及时效果道具、持有效果道具、主动使用道具。
物品对应的复杂事件由Effect处理，数值变化类由Bag类处理。
物品对象是程序对象，对应的游戏对象是DropItem，DropItem处理当物品为掉落物时的逻辑。

# 工作流程说明
## 创建实体流程
大部分实体使用AB包加载，小部分使用Resources加载。
注意使用热更新脚本的实体prefab需要装载EntityBehaivorLoader，而不是原本的Entity脚本
1. Resources
与Unity的Resources加载没有区别，实体prefab的脚本不需要热更新
2. AB包
在AssetBundle文件夹下创建prefab，选中prefab打包到StreamingAssets文件夹中
实体挂在的脚本放在EntityBehavior文件夹下
打包游戏本体时记得使用HybirdCLR热更新EntityBehaivior，将dll.bytes文件放在StreamingAssets文件夹下
可以运行工程根目录下的HybirdCLRFileReplace.bat来快速替换文件，在bat文件里修改需要替换的程序集名称
* tips
大部分Bullet类实体不需要额外加载，直接在对应的实体脚本里定义其行为即可。
## 创建物品
物品使用json描述，使用Unity自带的json工具加载
1. ItemDTO
    用于对齐存储的json数据，其中的sprite和事件使用字符串描述。
    事件字符串采用如下形式描述。sprite传入指定的sprite查询url从SpriteManager里获取sprite。
```
[事件类型]:[事件名称]([事件参数])
事件类型分文Effect，Event，Global
Effect意思是给实体添加指定名称的effect，并传递参数
Event是在Item类里定义的简易事件，通过方法名调用。
Global是全局定义的物品事件。
```
2. ItemManager

   物品管理器兼物品数据库，用于从文件中加载物品数据，那之后存储在游戏场景中，玩家可以直接通过database["dbname"]去访问物品数据。

3. Item

   物品类，是实际操作的物品对象。在逻辑实现层面，物品对象全局只有一个，通过Bag系统去规定物品的数量和所属。

   Item对象包含属性物品的唯一标识：物品名称itemName，物品信息itemInfo，spriteUrl，物品标签，物品的捡起时事件，使用时事件，丢弃时事件。

4. 工作流：

   在StreamingAssets文件夹下的ItemInfo文件夹，根据不同的dbname选择文件夹，然后在里面新建json文件或者在原json文件里新增数据

   数据格式参考原有的json文件，或者参考以下格式

   ``````json
   {
       "list": [
           {   
               "itemName": "物品名称",
               "itemInfo": "物品描述",
               "spriteUrl": "Dbname:test",
               "tags": "None ",
               "OnPick": "",
               "OnUse": "",
               "OnDrop": ""
           },
           {
               "itemName": "物品名称2",
               "itemInfo": "测试物品,捡起时添加事件",
               "spriteUrl": "",
               "tags": "None",
               "OnPick": "Event:ShowInfo()",
               "OnUse": "Event:Log(测试物品的使用事件测试)",
               "OnDrop": ""
           }
       ]
   }
   ``````

   关于事件如何定义参考ItemDTO的事件定义。
   
   至此变完成了Item的新增
   
   使用ItemManager.Init("dbname");来创建并开始加载新的Item数据库，已经加载的数据库不会再次加载
   
   使用ItemManager.datebase["dbname"];来获取指定名称的数据库db
   
   对db操作可以实现检查加载状态，获取数据库内容，重新加载数据库，卸载并释放数据库资源等
   
## 图像资源

   使用SpriteManager管理图像资源，图像资源默认放在StreamingAssets/Images目录下，和ItemManager一样通过不同dbname进行管理。

其使用和Itemmanager无二，将图像资源放在指定的库名文件夹下，就可以通过指定的库名和资源名称去加载资源

同样也是使用Init方法进行资源的预加载。

# TODO:
* 地图生成