# noCTRL Game Events

**Performant, extensible in-game event system for Unity.**  
Create events as assets, reference them anywhere in your project, and decouple your game logic without relying on hard-coded dependencies.

---

## ✨ Features
- **ScriptableObject-based events** for easy reusability and scene independence.
- **Strong decoupling** between event senders and listeners.
- **Lightweight & performant** runtime.
- **Custom Editor tools** for easy debugging and event management.
- **Sample scenes** to get started immediately.
- Fully compatible with **Unity 2021.3+**.

---

##  Crediting noCTRL Studios  
If you use this package, you must credit **noCTRL Studios** and include a link to its GitHub (https://github.com/noCTRL-Studios) in your project docs, README, or credits. See the full license in `LICENSE.md`.

---

## 📦 Installation

### Unity Package Manager (Git URL)
1. Open Unity → **Window** → **Package Manager**.
2. Click the **+** button → **Add package from git URL...**
3. Paste:
    ```text
    https://github.com/noCTRL-Studios/Unity-Game-Events.git
    ```

### Using a Specific Version
You can target a specific release by appending `#vX.Y.Z` to the URL:
```text
https://github.com/noCTRL-Studios/Unity-Game-Events.git#v1.0.0
```

---

## 🚀 Quick Start

### 1. Create an Event Asset
1. Right-click in the **Project** window.
2. Select **Create → Game Events → New Game Event**.
3. Name it (e.g., `OnCoinCollected`).

### 2. Raise an Event in Code
```csharp
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameEvent OnCoinCollected;

    public void Collect()
    {
        OnCoinCollected.Raise();
        Destroy(gameObject);
    }
}
```

### 3. Listen for the Event
```csharp
using UnityEngine;

public class ScoreListener : MonoBehaviour
{
    public GameEvent OnCoinCollected;

    private void OnEnable()
    {
        OnCoinCollected.Register(OnCoinCollectedHandler);
    }

    private void OnDisable()
    {
        OnCoinCollected.Unregister(OnCoinCollectedHandler);
    }

    private void OnCoinCollectedHandler()
    {
        Debug.Log("Coin collected! Updating score...");
    }
}
```

---

## 🧪 Sample Scene
A ready-to-use example scene is included in:
```
Samples~/Basic Usage
```
This demo shows how to:
- Create and raise events
- Listen and respond to them
- Debug events in the Inspector

---

## 📂 Project Structure
```
Runtime/       # Core runtime scripts (events, listeners)
Editor/        # Custom editors for easier workflow
Samples~/      # Example scenes and usage scripts
Tests/         # Optional automated tests
```

---

## 📝 Changelog
See [CHANGELOG.md](CHANGELOG.md) for version history.

---

## 📄 License
This project is licensed under the [MIT License](LICENSE.md).

---

## 👤 Author
**noCTRL Studios** – Tools and SDKs for game developers.  
📧 silashafeli@noctrlstudios.com  
🌐 [https://noctrlstudios.com](https://noctrlstudios.com)
