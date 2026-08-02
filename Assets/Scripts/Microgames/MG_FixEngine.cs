using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame untuk profil De Doener: Monteur Motoren.
/// Mekanik: Pemain harus menekan (tap/klik) beberapa baut mesin yang berasap
/// untuk memperbaikinya sebelum waktu habis.
/// </summary>
public class MG_FixEngine : MicrogameBase
{
    [Header("Engine Bolts")]
    [Tooltip("Daftar tombol baut yang harus ditekan")]
    [SerializeField] private List<Button> boltButtons;
    
    [Header("Visuals")]
    [SerializeField] private Color brokenColor = Color.red;
    [SerializeField] private Color fixedColor = Color.green;

    private int boltsFixedCount = 0;

    protected override void OnSetup()
    {
        // Reset state
        boltsFixedCount = 0;

        // Setup setiap baut
        foreach (var btn in boltButtons)
        {
            if (btn == null) continue;

            // Reset warna baut ke warna "rusak"
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = brokenColor;

            // Pastikan tombol bisa diklik
            btn.interactable = true;

            // Buat listener baru untuk menghindari duplikasi
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnBoltClicked(btn));
        }

        Debug.Log("[MG_FixEngine] Setup complete. Fix " + boltButtons.Count + " bolts!");
    }

    private void OnBoltClicked(Button clickedBtn)
    {
        // Jika belum main (masih muncul instruksi) atau sudah selesai, abaikan klik
        if (!isPlaying || isCompleted) return;

        // Baut ini sudah tidak bisa diklik lagi
        clickedBtn.interactable = false;

        // Ubah visual baut menjadi "diperbaiki"
        var img = clickedBtn.GetComponent<Image>();
        if (img != null) img.color = fixedColor;

        // (Opsional) Tambahkan efek partikel atau suara di sini

        boltsFixedCount++;

        // Cek apakah semua baut sudah diperbaiki
        if (boltsFixedCount >= boltButtons.Count)
        {
            // Semua beres! Pemain menang
            WinMicrogame(200); // Beri bonus skor 200
        }
    }
}
