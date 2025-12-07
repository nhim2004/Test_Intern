# Magic Bloom: Sort Water - Unity Prototype

## 📝 Mô tả
Đây là prototype gameplay của game "Magic Bloom: Sort Water" - game giải đố sắp xếp nước màu trong ống nghiệm.

## 🎮 Gameplay
- Chọn một ống nghiệm có nước
- Chọn ống nghiệm khác để đổ nước vào
- Chỉ có thể đổ nước cùng màu lên nhau
- Mục tiêu: Sắp xếp sao cho mỗi ống chỉ chứa một màu

## 🛠️ Tính năng

### Core Features
- ✅ Logic game chính xác và mượt mà
- ✅ Ít nhất 8 ống nghiệm mỗi level
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

### Design Patterns Used
- **Singleton Pattern**: GameManager, AudioManager, ParticleEffectManager
- **Object Pooling**: Tối ưu hiệu suất cho particles và effects
- **State Pattern**: Game states (Playing, Won, Paused)
- **Observer Pattern**: Event-driven architecture cho UI updates

## 📂 Cấu trúc Project

```
Assets/
├── Scripts/
│   ├── Core/
│   │   ├── WaterColor.cs         - Dữ liệu màu nước
│   │   ├── Bottle.cs              - Logic ống nghiệm
│   │   ├── LevelData.cs           - Dữ liệu level
│   │   ├── LevelGenerator.cs      - Sinh level tự động
│   │   └── BottleAnimator.cs      - Animation cho ống
│   ├── Managers/
│   │   ├── GameManager.cs         - Quản lý game chính
│   │   └── AudioManager.cs        - Quản lý âm thanh
│   ├── UI/
│   │   ├── UIManager.cs           - Quản lý UI
│   │   └── HintSystem.cs          - Hệ thống gợi ý
│   ├── Effects/
│   │   ├── WaterEffect.cs         - Hiệu ứng nước
│   │   └── ParticleEffectManager.cs - Quản lý particles
│   └── Utilities/
│       ├── ObjectPool.cs          - Object pooling
│       └── SimpleTween.cs         - Animation helper
├── Prefabs/
├── Scenes/
├── Audio/
└── Animations/
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

1. Click vào ống nghiệm có nước để chọn
2. Click vào ống nghiệm khác để đổ nước
3. Chỉ có thể đổ nước cùng màu lên nhau
4. Ống đích phải có chỗ trống
5. Thắng khi tất cả ống đều chứa một màu hoặc rỗng

## 🔧 Configuration

### Level Settings
- `numberOfBottles`: Số lượng ống (mặc định: 8)
- `numberOfColors`: Số màu khác nhau (mặc định: 5)
- `bottleCapacity`: Sức chứa mỗi ống (mặc định: 4)

### Difficulty Settings
- `difficultyLevel`: 0-1 (0 = dễ, 1 = khó)
- `extraEmptyBottles`: Số ống trống thêm
- `ensureSolvable`: Đảm bảo level giải được

## 📊 Performance Optimizations

- Object pooling cho particles
- Efficient collision detection
- Optimized sprite batching
- Memory-efficient data structures

## 🎓 Code Quality

- **Clean code**: Tên biến rõ ràng, comments đầy đủ
- **SOLID principles**: Single responsibility, Open/closed
- **Design patterns**: Singleton, Observer, Object Pool
- **Modular design**: Dễ dàng mở rộng và bảo trì

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

## 📝 Notes

- Project chỉ push 3 folders: **Assets, Packages, ProjectSettings**
- File .gitignore đã được cấu hình sẵn
- Code được tổ chức theo namespace để dễ quản lý
- Tất cả classes đều có XML documentation

## 👤 Author

Intern Test Project

## 📄 License

This project is created for evaluation purposes.

---

**Chúc may mắn với bài test!** 🍀
