# CSGO_LC

## 使用方法
1.下载 Visual Studio (无论什么版本)。
2.勾选'.NET桌面开发'选项。
3.载入编译运行即可。

## 辅助参数设置
参数设置位于源码顶部。以下是这些参数的详细说明：

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

