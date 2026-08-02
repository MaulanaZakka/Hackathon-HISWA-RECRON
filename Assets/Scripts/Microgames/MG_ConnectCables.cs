using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame untuk profil De Doener: Elektromonteur (Teknisi Listrik).
/// Mekanik: Menghubungkan kabel dengan warna yang sama. 
/// Pemain klik kabel di sisi kiri, lalu klik colokan yang warnanya sama di sisi kanan.
/// </summary>
public class MG_ConnectCables : MicrogameBase
{
    [Header("Cable Nodes")]
    [Tooltip("Tombol kabel di sisi kiri")]
    [SerializeField] private List<Button> leftNodes;
    
    [Tooltip("Tombol colokan di sisi kanan")]
    [SerializeField] private List<Button> rightNodes;

    [Header("Colors (Must match count)")]
    [SerializeField] private List<Color> cableColors = new List<Color> { Color.red, Color.green, Color.blue };
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow; // Highlight saat kiri dipilih

    private int currentlySelectedLeftIndex = -1;
    private int cablesConnected = 0;

    // Untuk melacak warna apa yang dimiliki setiap node
    private List<int> leftColorIDs = new List<int>();
    private List<int> rightColorIDs = new List<int>();

    protected override void OnSetup()
    {
        currentlySelectedLeftIndex = -1;
        cablesConnected = 0;

        // Siapkan ID warna (0, 1, 2)
        leftColorIDs.Clear();
        rightColorIDs.Clear();
        for (int i = 0; i < cableColors.Count; i++)
        {
            leftColorIDs.Add(i);
            rightColorIDs.Add(i);
        }

        // Acak posisi warna di sisi kanan agar permainannya menantang
        ShuffleList(rightColorIDs);

        // Setup Listener dan Warna Visual
        for (int i = 0; i < leftNodes.Count; i++)
        {
            int index = i; // Cache index for lambda
            if (leftNodes[index] != null)
            {
                leftNodes[index].interactable = true;
                leftNodes[index].GetComponent<Image>().color = cableColors[leftColorIDs[index]];
                leftNodes[index].onClick.RemoveAllListeners();
                leftNodes[index].onClick.AddListener(() => OnLeftNodeClicked(index));
            }
        }

        for (int i = 0; i < rightNodes.Count; i++)
        {
            int index = i;
            if (rightNodes[index] != null)
            {
                rightNodes[index].interactable = true;
                rightNodes[index].GetComponent<Image>().color = cableColors[rightColorIDs[index]];
                rightNodes[index].onClick.RemoveAllListeners();
                rightNodes[index].onClick.AddListener(() => OnRightNodeClicked(index));
            }
        }

        Debug.Log("[MG_ConnectCables] Setup complete. Hubungkan kabel sesuai warnanya!");
    }

    private void OnLeftNodeClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        // Reset warna visual node kiri yang sebelumnya dipilih (jika ada)
        if (currentlySelectedLeftIndex != -1 && leftNodes[currentlySelectedLeftIndex].interactable)
        {
            leftNodes[currentlySelectedLeftIndex].GetComponent<Image>().color = cableColors[leftColorIDs[currentlySelectedLeftIndex]];
        }

        // Pilih node kiri ini
        currentlySelectedLeftIndex = index;
        
        // Beri efek highlight (misal jadi sedikit terang atau beda warna)
        // Di sini kita pakai border/warna lain, tapi untuk simpelnya kita redupkan warnanya
        Color c = cableColors[leftColorIDs[index]];
        c.a = 0.5f; // Transparan 50% tanda sedang dipilih
        leftNodes[index].GetComponent<Image>().color = c;
    }

    private void OnRightNodeClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        // Jika pemain belum memilih kabel kiri, abaikan
        if (currentlySelectedLeftIndex == -1) return;

        // Cek apakah warna kabel kiri yang dipilih SAMA dengan colokan kanan ini
        int leftColorID = leftColorIDs[currentlySelectedLeftIndex];
        int rightColorID = rightColorIDs[index];

        if (leftColorID == rightColorID)
        {
            // BENAR! Kabel terhubung
            leftNodes[currentlySelectedLeftIndex].interactable = false;
            rightNodes[index].interactable = false;

            // Kembalikan warnanya jadi solid + matikan agar tidak bisa diklik lagi
            leftNodes[currentlySelectedLeftIndex].GetComponent<Image>().color = cableColors[leftColorID];
            rightNodes[index].GetComponent<Image>().color = cableColors[rightColorID];

            // (Opsional) Disini bisa memunculkan garis penghubung UI

            currentlySelectedLeftIndex = -1; // Reset pilihan
            cablesConnected++;

            // Jika semua kabel terhubung, menang!
            if (cablesConnected >= cableColors.Count)
            {
                WinMicrogame(200); // Bonus 200 point
            }
        }
        else
        {
            // SALAH PASANGAN! 
            // Reset pilihan kabel kiri
            leftNodes[currentlySelectedLeftIndex].GetComponent<Image>().color = cableColors[leftColorID];
            currentlySelectedLeftIndex = -1;
            
            // (Opsional) Kurangi waktu sebagai penalti jika salah pasang
            // timeRemaining -= 1f; 
        }
    }

    // Helper untuk mengacak list (Fisher-Yates shuffle)
    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
