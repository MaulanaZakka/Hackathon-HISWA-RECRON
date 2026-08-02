using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Microgame: De Helper - Zeil Instructeur
/// Mechanic: Balance. Tap the button to counter the wind pushing the slider to the edge.
/// </summary>
public class MG_TeachSailing : MicrogameBase
{
    [Header("UI Elements")]
    [Tooltip("Slider yang mewakili posisi perahu. Nilai 1 = Perahu seimbang/menang.")]
    [SerializeField] private Slider balanceSlider;
    
    [Tooltip("Tombol untuk menarik layar / menyeimbangkan perahu (bisa ditekan berulang-ulang)")]
    [SerializeField] private Button sailButton; 
    
    [Header("Settings")]
    [Tooltip("Kecepatan angin mendorong perahu (mengurangi nilai slider per detik)")]
    [SerializeField] private float windPushSpeed = 0.2f; 
    
    [Tooltip("Berapa banyak slider bertambah setiap kali tombol ditekan")]
    [SerializeField] private float tapPushAmount = 0.15f; 

    protected override void OnSetup()
    {
        if (balanceSlider != null)
        {
            balanceSlider.value = 0.2f; // Mulai dari hampir jatuh
            balanceSlider.interactable = false;
        }

        if (sailButton != null)
        {
            sailButton.onClick.RemoveAllListeners();
            sailButton.onClick.AddListener(OnSailClicked);
        }
    }

    protected override void Update()
    {
        base.Update(); // Memanggil Update di MicrogameBase agar timer berjalan

        if (!isPlaying || isCompleted || balanceSlider == null) return;

        // Angin selalu mendorong ke bawah (0)
        balanceSlider.value -= windPushSpeed * Time.deltaTime;

        if (balanceSlider.value <= 0f)
        {
            balanceSlider.value = 0f;
            // Biarkan saja di 0. Nanti kalau waktu habis, dia otomatis kalah.
        }
    }

    private void OnSailClicked()
    {
        if (!isPlaying || isCompleted || balanceSlider == null) return;

        // Pemain menyeimbangkan ke atas (1)
        balanceSlider.value += tapPushAmount;

        // Jika mencapai ujung atas, pemain menang!
        if (balanceSlider.value >= 1f)
        {
            balanceSlider.value = 1f;
            WinMicrogame();
        }
    }
}
