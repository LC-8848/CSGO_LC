# CSGO_LC

## 介绍
该辅助是在闲余时间随手写的，功能不是很全，使用C#的overlay，自写了模拟鼠标自瞄算法(解决鼠标乱晃)，源码中的大部分代码都打了注释，这里也就不多说了，新手可以对照源码研究，此源码对应游戏是老版本单机Counter-Strike Global Offensive。[→下载链接← 密码:p4t0](https://pan.baidu.com/s/1li-fFDHelOZo0V8f_7kY-g?pwd=p4t0) ，压缩包中还附带了初学逆向时自写的易语言辅助，本来想一同开源的，奈何源码丢失了 >_< 论功能易语言版的辅助更完善(可实时更改辅助参数，读取了敌人骨骼数据，自瞄更精确，添加了‘被动技能’)，使用C#重新开发的目的只是想换个语言练练手，现在目的已经达到，对于辅助的开发就暂且告一段落了。

## 辅助参数设置
参数设置位于源码顶部。以下是辅助参数及详细说明：

```csharp
/* 辅助参数设置 */
public const bool isDraw = true; // 是否绘制
public const bool isAim = true; // 是否自瞄
public const bool isAimDraw = true; // 是否绘制自瞄范围
public const int AimRange = 250; // 自瞄范围
public const int Aimkey = 16; // 自瞄按键 1:左键 2:右键 4:中键 16:shift键 17:ctrl键 18:alt键 (更多按键请移步网上查询键代码)
public const int AimType = 1; // 0:写入内存 1:模拟鼠标
public const int Aimbody = 0; // 自瞄部位 0:头部 1:胸部
public const int AimSpeedx = 52; // x轴自瞄速度，值越大越锁人（仅模拟鼠标方式有效）
public const int AimSpeedy = 52; // y轴自瞄速度，值越大越锁人（仅模拟鼠标方式有效）
/* 结束辅助参数设置 */
```

## 使用方法
1.下载 Visual Studio (无论什么版本)。
2.安装依赖时勾选'.NET桌面开发'选项。
3.载入源码编译运行即可。
