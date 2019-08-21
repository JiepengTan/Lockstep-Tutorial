#  Lockstep Tutorial

### 前言
	本教程的目标是普及帧同步技术,含基本帧同步，以及预测回滚式帧同步，不含ECS
配套的Blog 
[配套的视频教程][10]

### 视频项目版本对应 关系
#### 阶段一
视频id | 项目版本号 
:-: | :-: 
0 | none  
1 | v0.0.1
2 | v0.1.1
3 | v0.1.1
4 | v0.1.1
5 | v0.2.1


### 教程大纲
#### 阶段一: 基础帧同步
0. [最终目标预览,以及注意点][12]
1. [环境搭建][11]
2. [简单服务器][13]
3. [基本位置同步][13]
4. 不同步的检测与定位
5. 逻辑的基本同步
6. 碰撞检测库的使用
7. 添加技能

#### 阶段二：预测&回滚式 
0. 预测回滚式框架概要讲解
1. 重构：全局变量对支持回滚
2. 碰撞检测系统的备份
3. 行为树的备份
4. 逻辑的备份
6. 回滚系统的处理

#### 阶段三：服务器相关处理
0. 重构:逻辑代码剥离
1. 关键数据服务器逻辑分离
2. 服务器运行游戏逻辑



最终大概效果
<p align="center"> <img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/LockstepPlatform/LPD_11_Network.gif?raw=true" width="512"/></p>

#### **References：** 
- 使用的帧同步库 [https://github.com/JiepengTan/LockstepEngine][1]
- 简单的帧同步ARPG Demo [https://github.com/JiepengTan/LockstepEngine_ARPGDemo][2]


 [1]: https://github.com/JiepengTan/LockstepEngine
 [2]: https://github.com/JiepengTan/LockstepEngine_ARPGDemo
 [3]: https://github.com/sschmid/Entitas-CSharp
 [4]: https://github.com/JiepengTan/LockstepMath
 [5]: https://github.com/JiepengTan/LockstepCollision
 [6]: https://github.com/JiepengTan/LockstepPlatform/releases
 [7]: https://github.com/sschmid/Entitas-CSharp/releases
 [8]: https://github.com/JiepengTan/LockstepPathFinding
 [9]: https://github.com/JiepengTan/LockstepBehaviorTree
 [10]: https://space.bilibili.com/308864667/channel/detail?cid=86562
 [11]: https://www.bilibili.com/video/av64643363
 [12]: https://www.bilibili.com/video/av64681509
 [13]: https://www.bilibili.com/video/av64688312
 [14]: https://www.bilibili.com/video/av64688312
 [15]: https://www.bilibili.com/video/av64688312