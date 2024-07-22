# ShipInitialization_for_SpaceEngineers
Grid Initialization  
This script will save the entitys in dictionary to customdata when press running;  
Only works for CURRENT GRID, plz place the PB on which's initialization needed;  
Includes thruster, battery, o2tank, h2tank, block that has inventory, antenna, beacon, power-generator, gyro;  
Run this script with "DockOn", it'll set battery to "Recharge", thruster to "turn-off", beacon to "turn-off", gyro to "turn-off" and vice versa for argument "Dockoff";  
If the type is like "IMyPowerProducer" or "IMyShipController", plz try to edit "setBlock" func directly.  
  
  
網格初始化腳本
該腳本會將PB所在網格中，字典內所含括的類別方塊之ID保存在自定義數據中
默認含括：推進器、電池、氧氣罐、氫氣罐、有庫存的方塊、天線、信標、發電模塊、陀螺儀  
使用“Dockon”參數運行此腳本，會將電池組設為“充電”、推進器組設為“關閉”、信標設為“關閉”、陀螺儀也設為“關閉”，使用“Dockoff”反之  
若要添加諸如“IMyPowerProducer”等大類，請直接編輯setBlock()函數。

