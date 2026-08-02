using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame: De Creatieveling - Animatiemedewerker
/// Mechanic: Whac-a-Mole. Tap the guest/button that lights up.
/// </summary>
public class MG_EntertainGuests : MicrogameBase
{
    [Header("Moles / Guests")]
    [Tooltip("Daftar tombol tamu/anak-anak")]
    [SerializeField] private List<Button> guestButtons;
    
    [Header("Settings")]
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color inactiveColor = Color.gray;
    [SerializeField] private int requiredHits = 5;

    private int currentHits = 0;
    private int activeIndex = -1;

    protected override void OnSetup()
    {
        currentHits = 0;
        for (int i = 0; i < guestButtons.Count; i++)
        {
            int index = i;
            guestButtons[i].onClick.RemoveAllListeners();
            guestButtons[i].onClick.AddListener(() => OnGuestClicked(index));
            SetGuestActive(i, false);
        }

        ActivateRandomGuest();
    }

    private void ActivateRandomGuest()
    {
        if (guestButtons.Count == 0) return;

        // Deactivate current
        if (activeIndex >= 0 && activeIndex < guestButtons.Count)
        {
            SetGuestActive(activeIndex, false);
        }

        // Pick new random that is not the same as before if possible
        int newIndex = activeIndex;
        while (newIndex == activeIndex && guestButtons.Count > 1)
        {
            newIndex = Random.Range(0, guestButtons.Count);
        }

        activeIndex = newIndex;
        SetGuestActive(activeIndex, true);
    }

    private void SetGuestActive(int index, bool isActive)
    {
        if (index < 0 || index >= guestButtons.Count || guestButtons[index] == null) return;
        
        guestButtons[index].GetComponent<Image>().color = isActive ? activeColor : inactiveColor;
    }

    private void OnGuestClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        if (index == activeIndex)
        {
            currentHits++;
            if (currentHits >= requiredHits)
            {
                SetGuestActive(activeIndex, false);
                WinMicrogame();
            }
            else
            {
                ActivateRandomGuest();
            }
        }
    }
}
