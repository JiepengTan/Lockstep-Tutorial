#  Lockstep Tutorial

### 前言
	本教程的目标是普及帧同步技术,含基本帧同步，以及预测回滚式帧同步，不含ECS
配套的Blog 
[配套的视频教程][3]


### 教程大纲
#### 阶段一: 基础帧同步
0. 大纲最  [code][30]  [video][10]
1. 环境搭建
2. 帧同步开发注意事项  [code][32]  [video][12]
3. 服务器，回放，客户端模式，基础框架，移动  [code][33]  [video][13]
4. 不同步的检测与定位  [code][34]  [video][14]
5. 帧同步逻辑编写  [code][35]  [video][15]
6. 碰撞检测&技能系统  [code][36]  [video][16]

#### 阶段二：预测&回滚式 
7. 帧同步预测回滚框架演示  [code][37]  [video][17]
8. 预测回滚式框架概要 [code][38]  [video][18]
9. 多平台,多实例 框架设计 [code][39]  [video][19]
10. 多平台,多实例 框架实现  [code][40]  [video][20]
11. "回滚" 基本生命期&数据的备份与还原  [code][41]  [video][21]
12. "预测" 实现&守望先锋网络方案比对  [code][42]  [video][22]
13. "预测" 自动伸缩的预测缓冲区  [code][43]  [video][23]
14. 预测回滚中的不同步的检测  [code][44]  [video][24]

#### 阶段三：服务器相关处理
15. 重构:逻辑代码剥离
16. 关键数据服务器逻辑分离
17. 服务器运行游戏逻辑



最终大概效果
<p align="center"> <img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/LockstepPlatform/LPD_11_Network.gif?raw=true" width="512"/></p>

#### **References：** 
- 使用的帧同步库 [https://github.com/JiepengTan/LockstepEngine][1]
- 简单的帧同步ARPG Demo [https://github.com/JiepengTan/LockstepEngine_ARPGDemo][2]

#### **QQ 群：** 
- 帧同步技术交流  839944367


 [1]: https://github.com/JiepengTan/LockstepEngine
 [2]: https://github.com/JiepengTan/LockstepEngine_ARPGDemo
 [3]: https://space.bilibili.com/308864667/channel/detail?cid=86562
 [4]: https://github.com/JiepengTan/LockstepMath
 [5]: https://github.com/JiepengTan/LockstepCollision
 [6]: https://github.com/JiepengTan/LockstepPlatform/releases
 [7]: https://github.com/sschmid/Entitas-CSharp/releases
 [8]: https://github.com/JiepengTan/LockstepPathFinding
 [9]: https://github.com/JiepengTan/LockstepBehaviorTree
 [10]: https://www.bilibili.com/video/av64643156
 [11]: https://www.bilibili.com/video/av64681509
 [12]: https://www.bilibili.com/video/av64681509
 [13]: https://www.bilibili.com/video/av64688312
 [14]: https://www.bilibili.com/video/av64716600
 [15]: https://www.bilibili.com/video/av64739012
 [16]: https://www.bilibili.com/video/av64899372
 [17]: https://www.bilibili.com/video/av66791686
 [18]: https://www.bilibili.com/video/av66821535
 [19]: https://www.bilibili.com/video/av66822773
 [20]: https://www.bilibili.com/video/av66822584
 [21]: https://www.bilibili.com/video/av66860995
 [22]: https://www.bilibili.com/video/av66902132
 [23]: https://www.bilibili.com/video/av67045101
 [24]: https://www.bilibili.com/video/av67085710
 [25]: https://www.bilibili.com/video/av66822584
 [26]: https://www.bilibili.com/video/av66822584
 [27]: https://www.bilibili.com/video/av66822584
 [28]: https://www.bilibili.com/video/av66822584
 [29]: https://www.bilibili.com/video/av66822584
 [30]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.0.1
 [31]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.1.1
 [32]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.1.1
 [33]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.1.1
 [34]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.2.1
 [35]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.3.1
 [36]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.3.1
 [37]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.1
 [38]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.1
 [39]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.1
 [40]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.1
 [41]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [40]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [42]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [43]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [44]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.3
 [45]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.3
 [46]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [47]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [48]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2
 [49]: https://github.com/JiepengTan/Lockstep-Tutorial/releases/tag/v0.4.2


