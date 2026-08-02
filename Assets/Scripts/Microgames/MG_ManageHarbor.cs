using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Microgame: De Organisator - Havenmeester
/// Mechanic: Timing. Tap the button when the moving slider is in the green zone.
/// </summary>
public class MG_ManageHarbor : MicrogameBase
{
    [Header("UI Elements")]
    [Tooltip("Slider yang mewakili pergerakan kapal")]
    [SerializeField] private Slider boatSlider;
    
    [Tooltip("Tombol untuk membuka gerbang/menerima kapal")]
    [SerializeField] private Button actionButton;
    
    [Header("Settings")]
    [Tooltip("Kecepatan gerak kapal")]
    [SerializeField] private float speed = 1.5f;
    [Tooltip("Batas bawah area aman (0.0 - 1.0)")]
    [SerializeField] private float safeZoneMin = 0.35f;
    [Tooltip("Batas atas area aman (0.0 - 1.0)")]
    [SerializeField] private float safeZoneMax = 0.65f;
    [Tooltip("Jumlah kapal yang harus diterima")]
    [SerializeField] private int requiredSuccesses = 3;

    private int currentSuccesses = 0;
    private float sliderDirection = 1f;

    protected override void OnSetup()
    {
        currentSuccesses = 0;
        sliderDirection = 1f;

        if (boatSlider != null)
        {
            boatSlider.value = 0f;
            boatSlider.interactable = false; // Player can't drag it manually
        }

        if (actionButton != null)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnActionClicked);
        }
    }

    protected override void Update()
    {
        base.Update(); // Memanggil Update di MicrogameBase agar timer berjalan

        if (!isPlaying || isCompleted || boatSlider == null) return;

        // Move the slider
        boatSlider.value += sliderDirection * speed * Time.deltaTime;

        // Ping-pong effect
        if (boatSlider.value >= 1f)
        {
            boatSlider.value = 1f;
            sliderDirection = -1f;
        }
        else if (boatSlider.value <= 0f)
        {
            boatSlider.value = 0f;
            sliderDirection = 1f;
        }
    }

    private void OnActionClicked()
    {
        if (!isPlaying || isCompleted || boatSlider == null) return;

        // Check if inside the green zone
        if (boatSlider.value >= safeZoneMin && boatSlider.value <= safeZoneMax)
        {
            currentSuccesses++;
            if (currentSuccesses >= requiredSuccesses)
            {
                WinMicrogame();
            }
            else
            {
                // Increase speed slightly for the next ship
                speed += 0.5f;
                // Reset position to edge
                boatSlider.value = 0f;
                sliderDirection = 1f;
            }
        }
        else
        {
            // Missed! Speed resets, player must try again.
            speed = 1.5f; 
        }
    }
}
