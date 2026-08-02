using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame: De Ondernemer - Medewerker Groenvoorziening
/// Mechanic: Clean Up. Tap all trash items to clean them up.
/// </summary>
public class MG_BeautifyResort : MicrogameBase
{
    [Header("Trash Items")]
    [Tooltip("Daftar tombol sampah/daun yang berceceran")]
    [SerializeField] private List<Button> trashButtons;

    private int cleanedCount = 0;

    protected override void OnSetup()
    {
        cleanedCount = 0;

        for (int i = 0; i < trashButtons.Count; i++)
        {
            if (trashButtons[i] == null) continue;

            trashButtons[i].gameObject.SetActive(true);
            
            int index = i;
            trashButtons[i].onClick.RemoveAllListeners();
            trashButtons[i].onClick.AddListener(() => OnTrashClicked(index));
        }
    }

    private void OnTrashClicked(int index)
    {
        if (!isPlaying || isCompleted) return;
        if (trashButtons[index] == null || !trashButtons[index].gameObject.activeSelf) return;

        // Hide the trash
        trashButtons[index].gameObject.SetActive(false);
        cleanedCount++;

        // Jika semua sudah dibersihkan, menang
        if (cleanedCount >= trashButtons.Count)
        {
            WinMicrogame();
        }
    }
}
