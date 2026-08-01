using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame untuk profil De Doener: Technische Dienst (Teknisi Pipa/Bangunan).
/// Mekanik: Memutar pipa agar tersambung.
/// Pemain mengetuk (tap) potongan pipa yang posisinya salah agar berputar 90 derajat.
/// Jika semua pipa menghadap ke arah yang benar (rotasi 0), pemain menang.
/// </summary>
public class MG_FixPipes : MicrogameBase
{
    [Header("Pipe Segments")]
    [Tooltip("Daftar tombol pipa yang bisa diputar")]
    [SerializeField] private List<Button> pipeButtons;
    
    [Header("Settings")]
    [Tooltip("Warna saat pipa belum tersambung benar")]
    [SerializeField] private Color brokenColor = Color.white;
    [Tooltip("Warna saat pipa sudah pada posisi yang benar")]
    [SerializeField] private Color fixedColor = Color.cyan;

    // Menyimpan rotasi saat ini (dalam kelipatan 90 derajat)
    private List<int> pipeRotations = new List<int>();

    protected override void OnSetup()
    {
        pipeRotations.Clear();

        for (int i = 0; i < pipeButtons.Count; i++)
        {
            if (pipeButtons[i] == null) continue;

            // Beri rotasi acak: 90, 180, atau 270 derajat. (Hindari 0 agar game tidak langsung menang)
            int randomAngleIndex = Random.Range(1, 4); // 1 = 90, 2 = 180, 3 = 270
            int currentRot = randomAngleIndex * 90;
            pipeRotations.Add(currentRot);

            // Terapkan rotasi secara visual
            pipeButtons[i].transform.localEulerAngles = new Vector3(0, 0, currentRot);
            
            // Set warna awal (rusak)
            pipeButtons[i].GetComponent<Image>().color = brokenColor;
            pipeButtons[i].interactable = true;

            // Setup listener
            int index = i; // Cache index
            pipeButtons[i].onClick.RemoveAllListeners();
            pipeButtons[i].onClick.AddListener(() => OnPipeClicked(index));
        }

        Debug.Log("[MG_FixPipes] Setup complete. Rotate the pipes to fix the flow!");
    }

    private void OnPipeClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        // Tambah rotasi 90 derajat
        pipeRotations[index] += 90;
        
        // Jaga agar nilai rotasi tetap dalam batas 0-359
        if (pipeRotations[index] >= 360)
        {
            pipeRotations[index] = 0;
        }

        // Terapkan rotasi visual
        pipeButtons[index].transform.localEulerAngles = new Vector3(0, 0, pipeRotations[index]);

        // Cek apakah pipa ini sudah di posisi yang benar (rotasi 0)
        if (pipeRotations[index] == 0)
        {
            pipeButtons[index].GetComponent<Image>().color = fixedColor;
        }
        else
        {
            pipeButtons[index].GetComponent<Image>().color = brokenColor;
        }

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        bool allFixed = true;
        foreach (int rot in pipeRotations)
        {
            // Jika ada satu saja pipa yang rotasinya bukan 0, belum menang
            if (rot != 0)
            {
                allFixed = false;
                break;
            }
        }

        if (allFixed)
        {
            // Matikan interaksi semua tombol
            foreach (var btn in pipeButtons)
            {
                btn.interactable = false;
            }

            WinMicrogame(200); // Bonus 200 point
        }
    }
}
