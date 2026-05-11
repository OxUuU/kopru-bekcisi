
<div align="center">

# 🛡️ Köprü Bekçisi

### *A medieval gatekeeper game inspired by Papers, Please*

[![Unity](https://img.shields.io/badge/Unity-6-000000?style=for-the-badge&logo=unity)](https://unity.com/)
[![URP](https://img.shields.io/badge/Render-URP_2D-blueviolet?style=for-the-badge)](https://unity.com/srp/universal-render-pipeline)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)
[![itch.io](https://img.shields.io/badge/Play_on-itch.io-FA5C5C?style=for-the-badge&logo=itchdotio&logoColor=white)](https://OxUuU.itch.io/kopru-bekcisi)

[Play Demo](https://OxUuU.itch.io/kopru-bekcisi) · [Devlog](#devlog) · [Report Bug](../../issues)

</div>

---

## 🎮 Hakkında

Köprü Bekçisi, ortaçağ/fantastik bir krallıkta köprü başında bekleyen bir muhafızın hikâyesini anlatan,
[Papers, Please](https://papersplea.se/)'den ilham alan bir 2D oyundur.
Geçmeye çalışanların belgelerini, eşyalarını ve şüpheli davranışlarını inceleyip karar veriyorsun:
**geç bırak**, **çevir**, veya **gözaltına al**.

Her karar krallığın güvenliği ile günlük yaşamın ekonomisi arasındaki ince çizgide.
Yanlış kararlar konsekansı olur — ailen aç kalır veya bir casus saraya sızar.

---

## 📸 Demo

<!-- BURAYA ÖNEMLİ: en az 1 GIF veya 2-3 screenshot koy. -->
<!-- Önerilen yöntem: GitHub'da issue açıp görsel sürükle → kopyala link → buraya yapıştır -->

![Gameplay](docs/gameplay.gif)

| Karakter Kontrolü | Belge İnceleme | Karar Anı |
|:-:|:-:|:-:|
| ![1](docs/screenshot1.png) | ![2](docs/screenshot2.png) | ![3](docs/screenshot3.png) |

---

## ✨ Özellikler

- 🎭 **Çok katmanlı NPC sistemi** — her ziyaretçinin arka planı, motivasyonu ve şüphe puanı var
- 📜 **Dinamik belge sistemi** — sahte mühürler, yanlış tarihler, eksik imzalar
- 💰 **Ekonomi simülasyonu** — ailene yiyecek almak için ücret almak zorundasın
- 🌗 **Gece/gündüz döngüsü** — farklı zamanlarda farklı tipler geçer
- 🇹🇷 **Türkçe & İngilizce dil desteği**

---

## 🏗️ Mimari

<!--
3-5 cümlelik teknik özet. Junior için ALTIN: hangi pattern'leri kullandığını
göstermek seni sıradan "tutorial bitiren" geliştiriciden ayırır.
-->

```
Assets/
├── Scripts/
│   ├── Core/              # Oyun durum makinesi, save/load
│   ├── Gameplay/          # Karar mekaniği, NPC sistemi
│   ├── UI/                # Belge inceleme, diyalog
│   └── Data/              # ScriptableObject tabanlı içerik
├── ScriptableObjects/     # NPC profilleri, belge tipleri, olay senaryoları
├── Sprites/               # Karakter, UI, çevre sanat varlıkları
└── Scenes/                # Ana menü, oyun, kredi
```

**Kullandığım yaklaşımlar:**
- **ScriptableObject** ile veri-driven NPC ve belge tanımları (kod değiştirmeden içerik eklenebilir)
- **State Machine** ile oyun akışı (Idle → Approaching → Reviewing → Decision → Outcome)
- **Event Bus** pattern ile sistemler arası gevşek bağlılık
- **Addressables** ile asset yükleme

---

## 🚀 Çalıştırma

### Gereksinimler
- Unity **6.0.0f1** veya üstü
- URP 2D template aktif

### Adımlar
```bash
git clone https://github.com/OxUuU/kopru-bekcisi.git
cd kopru-bekcisi
# Unity Hub → Add → bu klasörü seç → Unity 6 ile aç
```

Ana sahne: `Assets/Scenes/MainMenu.unity`

### Build alma
- **PC (Windows/Mac/Linux):** File → Build Settings → Standalone
- **Web:** File → Build Settings → WebGL → Build (deploy için itch.io)

---

## 📖 Devlog

<!--
Devlog en güçlü "tutkulu geliştirici" sinyallerinden biri. 
Kısa yazılar bile büyük etki yapar. Her büyük özellik için 1 yazı.
İlk başta sadece bir-iki yazıyla başla, zamanla doldur.
-->

- [ ] **#01 — Karakter render pipeline'ı: URP 2D'de neden ve nasıl** _(yakında)_
- [ ] **#02 — ScriptableObject ile veri mimarisi** _(yakında)_
- [ ] **#03 — Papers Please'in karar tasarımını uyarlamak** _(yakında)_

> Devlog'ları `docs/devlog/` altında markdown olarak yayınla.
> İstersen [dev.to](https://dev.to) veya [itch.io devlog](https://itch.io/post-game-devlog) üzerinden de paylaş.

---

## 🛠️ Yol Haritası

- [x] Temel karakter ve hareket
- [x] Bekçi karakolu environment'ı (Blender → 2D render)
- [ ] NPC diyalog sistemi
- [ ] Belge inceleme UI
- [ ] Karar konsekans sistemi
- [ ] Hikâye senaryoları (10+ gün)
- [ ] Türkçe lokalizasyon
- [ ] WebGL build + itch.io yayın

---

## 🎨 Sanat ve Müzik

<!-- Lisans/credits — açık kaynak asset kullanıyorsan zorunlu -->

- **Tile set:** [varsa kaynak]
- **Müzik:** [varsa kaynak]
- **Karakter sanatı:** Kendim (Blender + post-processing)
- **Yazılım:** Unity 6, Blender 5.1, Aseprite

Tüm dış kaynaklar [CREDITS.md](CREDITS.md) dosyasında listelenmiştir.

---

## 📄 Lisans

Bu proje [MIT License](LICENSE) altında lisanslanmıştır.

Sanat varlıkları ve müzik kendi lisansları altındadır — `CREDITS.md` dosyasına bak.

---

<div align="center">

**Yapımcı:** [Onur Ulaş Canpolat](https://github.com/OxUuU)

⭐ Beğendiysen yıldız bırak, motivasyon olur!

</div>
