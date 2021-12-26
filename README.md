# Mathematica

1 定点数Q16。 1~4维向量，3x3矩阵，4x4矩阵，四元数及其TRS.  函数的命名和行为尽量与Unity数学库一致。  
2 基于定点数的几何部分，其中包围盒实现AABB,OBB，Sphere，Capsule。任意旋转的包围盒，两两之间的碰撞检测。目前胶囊与AABB、OBB之间的检测效率较低。未对y轴始终朝上的普遍情形做特殊优化。  
3 基于定点数的物理部分，实现的BVH树效率不好，暂不上传了。R树优化GC后再传。To be continued.
