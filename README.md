# HISWA RECRON - Hackathon Game Project

Sebuah game berbasis Progressive Web App (PWA) yang dibangun menggunakan Unity WebGL untuk Hackathon HISWA RECRON. Game ini didesain secara spesifik agar dapat diakses melalui browser dengan fitur unggulan **Offline-First**, memungkinkan pemain untuk terus bermain meskipun tanpa koneksi internet setelah pemuatan pertama.

## 🌟 Fitur Utama

- **Offline-First (PWA):** Terintegrasi dengan Service Worker untuk melakukan *caching* terhadap semua aset game (.wasm, .data, dsb). Setelah dimuat sekali, game bisa dimainkan sepenuhnya tanpa koneksi internet.
- **Installable:** Pemain dapat menginstal game ini ke Home Screen (Android/iOS) atau Desktop mereka, sehingga terasa seperti aplikasi *native*.
- **Responsive Portrait Mode:** Dirancang khusus untuk dimainkan dalam orientasi potrait (1080x1920) baik di perangkat mobile maupun desktop (dengan *auto-scaling* fit-to-screen).
- **QR Code Ready:** Dilengkapi dengan halaman generator QR Code mandiri untuk mempermudah distribusi game di lokasi event/booth.

## 🎮 Cara Bermain

Game ini sudah di-hosting menggunakan GitHub Pages. Anda bisa mengaksesnya langsung melalui link berikut:

👉 **[Mainkan HISWA RECRON](https://maulanazakka.github.io/Hackathon-HISWA-RECRON/)**

### Untuk Event Organizer (QR Code)
Untuk menampilkan QR Code agar pengunjung event dapat melakukan scan dan bermain, buka halaman berikut di layar booth Anda:

👉 **[Halaman QR Code Game](https://maulanazakka.github.io/Hackathon-HISWA-RECRON/qrcode-page.html)**

## 🛠️ Tech Stack

- **Game Engine:** Unity 2022.3
- **Build Target:** WebGL
- **Web Technologies:** HTML5, CSS3, JavaScript (Service Worker, Manifest)
- **Deployment:** GitHub Pages

## 🚀 Cara Build & Deploy (Untuk Developer)

1. Buka project di **Unity**.
2. Masuk ke **File → Build Settings**.
3. Pastikan platform di-set ke **WebGL**.
4. Klik **Build** dan simpan output ke sebuah folder.
5. Pindahkan seluruh isi folder output tersebut ke *root folder* repository GitHub Anda (timpa/overwrite file lama).
6. Lakukan Commit dan Push ke branch `main`.
7. GitHub Pages akan otomatis memperbarui versi game.

*(Jangan lupa untuk menaikkan angka versi di variabel `CACHE_NAME` pada file `sw.js` setiap kali melakukan deploy baru agar browser melakukan update cache!)*
