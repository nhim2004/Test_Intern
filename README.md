# Magic Bloom: Sort Water - Unity Prototype

## 📝 Mô tả
Đây là prototype gameplay của game "Magic Bloom: Sort Water" - game giải đố sắp xếp nước màu trong ống nghiệm với hệ thống level, move limit, time limit và tính toán độ khó tự động.

## 🎮 Gameplay
- Chọn một ống nghiệm có nước
- Chọn ống nghiệm khác để đổ nước vào
- Chỉ có thể đổ nước cùng màu lên nhau
- Mục tiêu: Sắp xếp sao cho mỗi ống chỉ chứa một màu
- **Mới**: Giới hạn số bước di chuyển (Move Limit) và thời gian (Time Limit) trên các level cao

## 🛠️ Tính năng

### Core Features
- ✅ Logic game chính xác và mượt mà
- ✅ 13+ levels (có thể tạo tự động)
- ✅ Màn Win/Lose screen
- ✅ Hệ thống Undo/Reset
- ✅ Đếm số bước đi

### Advanced Features
- ✅ Animation và effects cho các hành động
- ✅ Sound effects cho mọi tương tác
- ✅ Particle effects (splash, celebration)
- ✅ Smooth transitions và visual feedback
- ✅ Hint system (gợi ý nước cờ)
- ✅ Level generator với độ khó tùy chỉnh
- ✅ Object pooling cho tối ưu performance

### Recent Updates (Level System v2)
- ✅ **Move Limit Panel**: Hiển thị số lượt đi còn lại với visual feedback (progress bar, color changes)
- ✅ **Time Limit System**: Timer đếm ngược, dùng `Time.realtimeSinceStartup` để tránh pause bug
- ✅ **Level Calculator**: Tính toán minimum moves và maximum moves cho phép dựa trên difficulty
- ✅ **Auto-Play Button**: Hiện khi level có move limit
- ✅ **Star Requirements**: Hiển thị điểm tối ưu (Your Best) từ lần chơi trước

### Design Patterns Used
- **Singleton Pattern**: GameManager, AudioManager, ParticleEffectManager
- **Object Pooling**: Tối ưu hiệu suất cho particles và effects
- **State Pattern**: Game states (Playing, Won, Lost, Paused)
- **Observer Pattern**: Event-driven architecture cho UI updates
- **Editor Tools**: Menu items để tạo/setup levels tự động

## 📂 Cấu trúc Project

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── WaterColor.cs         - Dữ liệu màu nước
│   │   ├── Bottle.cs              - Logic ống nghiệm
│   │   ├── LevelData.cs           - Dữ liệu level (difficulty, moves, time)
│   │   ├── BottleAnimator.cs      - Animation cho ống
│   │   └── CameraController.cs    - Điều khiển camera
│   ├── Managers/
│   │   ├── GameManager.cs         - Quản lý game chính, move counting, time tracking
│   │   ├── AudioManager.cs        - Quản lý âm thanh
│   │   └── SaveManager.cs         - Lưu/tải best scores
│   ├── Game/
│   │   └── LevelCalculator.cs     - Tính toán min/max moves theo difficulty
│   ├── UI/
│   │   ├── UIManager.cs           - Quản lý UI, panel animations
│   │   ├── MoveLimitPanel.cs      - Panel hiển thị move limit
│   │   ├── LevelRequirementsPanel.cs - Hiển thị yêu cầu level
│   │   └── HintSystem.cs          - Hệ thống gợi ý
│   ├── Effects/
│   │   ├── WaterEffect.cs         - Hiệu ứng nước
│   │   └── ParticleEffectManager.cs - Quản lý particles
│   ├── Editor/
│   │   ├── LevelGenerator.cs      - Tạo levels 11-13, 21-30, 31-50
│   │   ├── LevelSetupToolWithCalculation.cs - Setup levels với calculated stats
│   │   ├── CreateMoveLimitPanelTool.cs - Tạo Move Limit Panel trong scene
│   │   └── AutoAssignPanelsToUI.cs - Auto-assign panels vào UIManager
│   └── Utilities/
│       ├── ObjectPool.cs          - Object pooling
│       └── SimpleTween.cs         - Animation helper
├── Levels/
│   ├── Level_01.asset
│   ├── Level_02.asset
│   └── ... (tối đa 50 levels)
├── Prefabs/
│   ├── Bottle.prefab
│   ├── WaterSegment.prefab
│   └── LevelButton.prefab
├── Scenes/
│   └── Game.unity
└── Audio/
```

## 🎨 Assets Requirements

### Sprites Needed
- Bottle sprites (BOTTLE_01 to BOTTLE_06)
- Water segment sprite
- Button sprites
- Background
- UI elements

### Audio Needed
- Select sound
- Pour sound
- Win sound
- Error sound
- Background music

### Particles (Optional)
- Splash effect
- Celebration effect
- Sparkle effect

## 🚀 Setup Instructions

1. **Unity Version**: Unity 2022.3 hoặc Unity 6
2. **Clone project** từ GitHub
3. **Mở project** trong Unity
4. **Import assets** vào các thư mục tương ứng:
   - Sprites → Assets/Sprites/
   - Audio → Assets/Audio/
   - Prefabs → Assets/Prefabs/
5. **Setup Scene**:
   - Tạo GameManager GameObject
   - Tạo AudioManager GameObject
   - Tạo UI Canvas
   - Assign references trong Inspector

## 🎯 How to Play

1. **Chọn level** từ menu
2. **Click vào ống nghiệm** có nước để chọn
3. **Click vào ống khác** để đổ nước
4. **Ràng buộc**:
   - Chỉ có thể đổ nước cùng màu lên nhau
   - Ống đích phải có chỗ trống
   - Không vượt quá Move Limit (nếu có)
   - Hoàn thành trước Time Limit (nếu có)
5. **Thắng** khi tất cả ống đều chứa một màu hoặc rỗng

## 🔧 Level Configuration

### LevelData Asset Settings
```csharp
public int levelNumber;              // Level ID (1-50)
public int difficulty;               // 1-5 (Easy → Master)
public int numberOfBottles;          // 5-9 bottles
public int numberOfColors;           // 4-8 colors
public int bottleCapacity;           // Dung tích (thường 4)
public int maxMoves;                 // Số bước tối đa (0 = unlimited)
public int timeLimit;                // Giây (0 = no limit)
public int threeStarMoves;           // Điểm để được 3 sao
public int twoStarMoves;             // Điểm để được 2 sao
```

### Difficulty Progression
| Level | Difficulty | Bottles | Colors | Moves | Time Limit |
|-------|-----------|---------|--------|-------|-----------|
| 1-10  | Easy (1)  | 5-6     | 4-5    | ∞     | None      |
| 11-20 | Normal (2)| 6-7     | 5-6    | Auto  | Level 15, 20 |
| 21-30 | Hard (3)  | 7-8     | 6-7    | Auto  | Level 25, 30 |
| 31-40 | Expert (4)| 8-9     | 7-8    | Auto  | Level 35, 40 |
| 41+   | Master (5)| 9       | 8      | Auto  | Level 45, 50 |

### Move Calculation Algorithm
```
minMoves = numberOfColors + (numberOfBottles - numberOfColors) * 2

maxMoves based on difficulty:
- Difficulty 1 (Easy):   150% of minMoves
- Difficulty 2 (Normal): 120% of minMoves
- Difficulty 3 (Hard):   100% of minMoves
- Difficulty 4 (Expert): 80% of minMoves
- Difficulty 5 (Master): 60% of minMoves
```

## 🛠️ Editor Tools

### Level Generation
```
WaterSort/Generate Levels/Create Levels 11-13
WaterSort/Generate Levels/Create Levels 21-30
WaterSort/Generate Levels/Create Levels 31-50
```
Tạo level assets tự động với:
- Calculated difficulty và move limits
- Time limit cho levels 5, 10, 15...
- Star thresholds dựa trên minimum moves

### Level Setup
```
WaterSort/Setup Levels/Calculate & Setup Level (Single)
WaterSort/Setup Levels/Calculate & Setup Level Range (11-50)
WaterSort/Setup Levels/Show Level Calculation Stats
WaterSort/Setup Levels/Calculate Difficulty Table
```
Setup levels đã tạo với calculated stats

### UI Panel Creation
```
WaterSort/UI Tools/Create Move Limit Panel in Scene
WaterSort/UI Tools/Create Simple Move Counter (No Limit)
WaterSort/UI Tools/Auto-Assign Panels to UIManager
```
Tự động tạo Move Limit Panel và assign references

## 🔍 Bug Fixes & Improvements

### Timer System (Fixed v1.1)
- **Issue**: Timer bị freeze khi pause (Time.timeScale = 0)
- **Solution**: Dùng `Time.realtimeSinceStartup` thay vì `Time.deltaTime`
- **Impact**: Timer giờ chạy độc lập với game pause

### Lose Panel Display (Fixed v1.1)
- **Issue**: Child panel không hiển thị khi parent inactive
- **Solution**: Đảm bảo parent panel SetActive(true) trước khi show child
- **Impact**: Time limit lose panel giờ hiển thị đúng

### Move Limit Panel Integration (Fixed v1.2)
- **Issue**: Panel tạo nhưng không update khi move count thay đổi
- **Solution**: Tạo auto-assign tool để link panel vào UIManager
- **Impact**: Move limit feedback giờ hoạt động 100%

## 🚀 Quick Start Guide

### 1. Setup Project
```bash
# Clone repo
git clone https://github.com/nhim2004/Test_Intern.git
cd "Test_Intern/My project"

# Mở trong Unity 2022.3.6f1
```

### 2. Generate Levels
```
Editor: WaterSort/Generate Levels/Create Levels 11-13
Hoặc: WaterSort/Generate Levels/Create Levels 21-30
```

### 3. Setup Levels with Calculations
```
Editor: WaterSort/Setup Levels/Calculate & Setup Level Range (11-50)
```

### 4. Create Move Limit Panel
```
Editor: WaterSort/UI Tools/Create Move Limit Panel in Scene
Editor: WaterSort/UI Tools/Auto-Assign Panels to UIManager
```

### 5. Test Game
```
Play level từ menu hoặc Play scene Game.unity
```

## 📊 Performance Optimizations

- Object pooling cho particles
- Efficient collision detection
- Optimized sprite batching
- Memory-efficient data structures
- Real-time tracking không phụ thuộc Time.timeScale

## 🎓 Code Quality

- **Clean code**: Tên biến rõ ràng, comments đầy đủ
- **SOLID principles**: Single responsibility, Open/closed
- **Design patterns**: Singleton, Observer, Object Pool, Strategy
- **Modular design**: Dễ dàng mở rộng và bảo trì
- **XML Documentation**: Tất cả public methods có comments

## 🌟 Future Enhancements

Có thể thêm các tính năng sau:
- DOTween cho animations mượt mà hơn
- Addressables cho asset management
- Save/Load system
- Leaderboard
- Daily challenges
- Power-ups và boosters
- Theme system
- Multiplayer mode
- Particle effects cho move limit warnings

## 📝 Project Info

- **Engine**: Unity 2022.3.6f1
- **Language**: C# 10.0
- **Namespace**: WaterSort.* (Core, Managers, UI, Game, Effects, Utilities)
- **Git**: Chỉ track Assets/, Packages/, ProjectSettings/
- **Lines of Code**: 5000+ lines (production code)

## 📄 License

This project is created for evaluation purposes.

---

**Last Updated**: December 2025  
**Version**: 1.2 (Move Limit & Level Calculator System)
