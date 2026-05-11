# Köprü Bekçisi — Kurulum Rehberi (M0 → M1)

Bu klasör, Unity 6 projenin **template / staging** halidir. Aşağıdaki adımları sırayla yap.

---

## 0. Ön Koşullar

- **Unity Hub** kurulu (https://unity.com/download)
- **Unity 6 LTS** — Hub'dan indir (Editor sürümü "6000.0.x LTS" olmalı)
- **Git** kurulu (https://git-scm.com/)
- **Git LFS** kurulu (https://git-lfs.github.com/) → terminalde bir kez `git lfs install` çalıştır

---

## 1. Unity Projesini Oluştur

1. Unity Hub'ı aç → **Projects** → **New project**.
2. Sol panelden **2D (URP)** template'ini seç. (Bu, URP 2D Renderer'ı otomatik kurar.)
3. **Project name:** `KopruBekcisi`
4. **Location:** `C:\Users\onuru\Desktop\` (bu dosyanın bulunduğu klasörle aynı seviye, **bu template klasörünün İÇİ değil**)
5. **Editor version:** Unity 6 LTS
6. **Create project**'e bas.

Unity proje açılınca `C:\Users\onuru\Desktop\KopruBekcisi\` klasörü oluşur. Sonraki adıma geç.

---

## 2. Template İçeriğini Projeye Kopyala

Yeni Unity projesini **kapat** (Editor'ı kapat).

PowerShell aç ve şunu çalıştır:

```powershell
$src = "C:\Users\onuru\Desktop\KopruBekcisi-template"
$dst = "C:\Users\onuru\Desktop\KopruBekcisi"
Copy-Item -Path "$src\.gitignore" -Destination $dst -Force
Copy-Item -Path "$src\.gitattributes" -Destination $dst -Force
Copy-Item -Path "$src\Assets\_Project" -Destination "$dst\Assets\" -Recurse -Force
```

Bu, `_Project/` klasörünü ve git config dosyalarını projenin köküne taşır.

---

## 3. Unity'yi Yeniden Aç ve Doğrula

1. Unity Hub'tan `KopruBekcisi` projesini aç.
2. Editor açılınca **Console**'u kontrol et — kırmızı hata olmamalı.
3. **Project** penceresinde `Assets/_Project/Scripts/...` ağacının görünür olduğunu doğrula.
4. Unity scriptleri compile edecek. Bittiğinde Console'da yeşil ✓ olmalı.

**Beklenen log:** Henüz hiçbir scene içinde Bootstrapper olmadığı için Play'e basınca log akmaz. Bu normal.

---

## 4. M1: Sahneleri ve Bootstrapper'ı Kur

### 4.1. 3 Sahne oluştur

Unity'de:
1. `Assets/_Project/Scenes/` klasörüne sağ tıkla → **Create → Scene** → adı **`Boot`** ver.
2. Aynı yere bir tane daha → **`Bridge`**.
3. Bir tane daha → **`Home`**.

### 4.2. Build Settings'e ekle

**File → Build Settings** → 3 sahneyi de soldaki Project'ten sürükleyip listeye ekle. **Boot** en üstte (index 0) olmalı.

### 4.3. GameDirector prefab'ı oluştur

1. **Boot** sahnesini aç.
2. Hierarchy'de boş bir GameObject oluştur (`GameObject → Create Empty`), adını **`GameDirector`** koy.
3. **Add Component → Game Director** (otomatik tamamlama ile bul, namespace: `KopruBekcisi.Core`).
4. Inspector'da `Bridge Scene Name` = `Bridge`, `Home Scene Name` = `Home` yaz.
5. Hierarchy'deki `GameDirector` GameObject'ini `Assets/_Project/Prefabs/` klasörüne sürükle → prefab oluştur.
6. Sahnedeki orijinal GameObject'i sil (sadece prefab kalsın).

### 4.4. Bootstrapper'ı Boot sahnesine ekle

1. **Boot** sahnesinde boş bir GameObject oluştur, adı **`Bootstrapper`**.
2. **Add Component → Bootstrapper**.
3. Inspector'da `Game Director Prefab` alanına az önce oluşturduğun prefab'ı sürükle.
4. Sahneyi kaydet (Ctrl+S).

### 4.5. Doğrulama (M1 testi)

1. Play'e bas.
2. Console'da görmeyi beklediğin loglar:
   ```
   [GameDirector] Boot -> MainMenu
   ```
3. Şimdi **Play modundayken** Console'da test:
   - `GameDirector.Instance.RequestState(KopruBekcisi.Core.GameState.DayIntro)` çalıştır → log: `MainMenu -> DayIntro`
   - `GameDirector.Instance.RequestState(KopruBekcisi.Core.GameState.BridgeShift)` → Bridge sahnesi yüklenir, log: `DayIntro -> BridgeShift`
   - Bridge sahnesinin yüklendiğini Hierarchy'de doğrula.

> **Not:** Bridge ve Home sahnelerinin içi şu an boş — sadece sahne geçişini test ediyoruz. M2'de Pixel Perfect Camera ve parallax kuracağız.

---

## 5. Git Repo Kurulumu (Opsiyonel ama Önerilir)

```powershell
cd C:\Users\onuru\Desktop\KopruBekcisi
git init
git lfs install
git add .gitignore .gitattributes
git commit -m "M0: project bootstrap (Unity 6 + URP 2D + .gitignore + .gitattributes)"
git add .
git commit -m "M1: GameDirector + state machine + 3 scenes"
```

---

## 6. Sıradaki Milestone (M2)

M1'i doğruladıktan sonra bana **"M1 onaylı, M2'ye geç"** yaz. M2 şunları kuracak:
- Pixel Perfect Camera (640x360 → 1080p 3x scale)
- 2D Light prototipi
- Parallax test katmanı (3 katman: arka plan / orta / ön)

---

## Sorun Giderme

- **Console'da `KopruBekcisi.Core` namespace bulunamadı:** Asmdef düzgün import edilmedi. Editor'ı kapat → `Library/` klasörünü sil → Editor'ı tekrar aç (re-import).
- **Build Settings'te sahneler yok:** Sahneler diskte var ama Build Settings'e eklenmemiş — tekrar ekle.
- **Play'e basınca hiçbir log yok:** Bootstrapper sahnesinde değil veya `Game Director Prefab` referansı boş.
- **`SceneNotFoundException`:** Bridge/Home sahneleri Build Settings'te değil veya isim eşleşmiyor (büyük/küçük harf duyarlı).

---

## Klasör Yapısı (referans)

```
KopruBekcisi/
├── .gitignore
├── .gitattributes
├── Assets/
│   ├── _Project/
│   │   ├── Art/  Audio/  Data/  Prefabs/  Scenes/  Settings/
│   │   └── Scripts/
│   │       ├── KopruBekcisi.Runtime.asmdef
│   │       ├── Core/
│   │       │   ├── GameDirector.cs
│   │       │   ├── Boot/Bootstrapper.cs
│   │       │   ├── GameStateMachine/  (GameState.cs, GameStateMachine.cs)
│   │       │   ├── EventChannels/     (VoidEventChannelSO.cs, GameStateEventChannelSO.cs)
│   │       │   └── Save/SaveService.cs
│   │       ├── Gameplay/Day/DayManager.cs
│   │       ├── Camera/ParallaxController.cs
│   │       ├── Input/InputRouter.cs
│   │       ├── Audio/AudioDirector.cs
│   │       ├── Economy/EconomyService.cs
│   │       ├── Karma/KarmaService.cs
│   │       └── Faction/FactionService.cs
│   ├── Settings/  (Unity'nin kendi URP assets klasörü)
│   ├── Scenes/    (Unity'nin oluşturduğu varsayılan SampleScene — silebilirsin)
├── Packages/      (Unity yönetir)
├── ProjectSettings/  (Unity yönetir)
└── Library/       (.gitignored — Unity cache)
```
