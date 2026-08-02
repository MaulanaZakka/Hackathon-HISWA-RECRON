using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame untuk profil De Doener: Technische Dienst (Teknisi Pipa/Bangunan).
/// Mekanik: Memutar pipa agar tersambung.
/// Pemain mengetuk (tap) potongan pipa yang posisinya salah agar berputar 90 derajat.
/// </summary>
public class MG_FixPipes : MicrogameBase
{
    [Header("Pipe Segments (Pisahkan jenisnya)")]
    [Tooltip("Masukkan semua pipa LURUS ke sini")]
    [SerializeField] private List<Button> straightPipes;
    
    [Tooltip("Masukkan semua pipa BELOK (L-shape) ke sini")]
    [SerializeField] private List<Button> cornerPipes;
    
    [Header("Settings")]
    [SerializeField] private Color brokenColor = Color.white;
    [SerializeField] private Color fixedColor = Color.cyan;

    private class PipeState
    {
        public Button button;
        public int currentRot;
        public int targetRot;
        public bool isStraight;
    }

    private List<PipeState> allPipes = new List<PipeState>();

    protected override void OnSetup()
    {
        allPipes.Clear();

        // Setup Pipa Lurus
        foreach (var btn in straightPipes)
        {
            if (btn != null) SetupPipe(btn, true);
        }

        // Setup Pipa Belok
        foreach (var btn in cornerPipes)
        {
            if (btn != null) SetupPipe(btn, false);
        }

        Debug.Log("[MG_FixPipes] Setup complete. Menunggu pemain menyambung pipa.");
    }

    private void SetupPipe(Button btn, bool isStraight)
    {
        PipeState state = new PipeState();
        state.button = btn;
        state.isStraight = isStraight;

        // Kunci jawaban = Rotasi awal di Editor
        state.targetRot = Mathf.RoundToInt(btn.transform.localEulerAngles.z) % 360;
        if (state.targetRot < 0) state.targetRot += 360;

        // Acak awal: Tambah 90, 180, atau 270 derajat
        int randomAngle = Random.Range(1, 4) * 90;
        state.currentRot = (state.targetRot + randomAngle) % 360;

        btn.transform.localEulerAngles = new Vector3(0, 0, state.currentRot);
        btn.GetComponent<Image>().color = brokenColor;
        btn.interactable = true;

        int index = allPipes.Count;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnPipeClicked(index));

        allPipes.Add(state);
    }

    private void OnPipeClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        PipeState state = allPipes[index];

        // Putar 90 derajat searah jarum jam (minus)
        state.currentRot = (state.currentRot - 90) % 360;
        if (state.currentRot < 0) state.currentRot += 360;

        state.button.transform.localEulerAngles = new Vector3(0, 0, state.currentRot);

        // Update warna visual
        if (IsPipeCorrect(state))
        {
            state.button.GetComponent<Image>().color = fixedColor;
        }
        else
        {
            state.button.GetComponent<Image>().color = brokenColor;
        }

        CheckWinCondition();
    }

    private bool IsPipeCorrect(PipeState state)
    {
        if (state.isStraight)
        {
            // Pipa lurus simetris. Beda 180 derajat tetap dianggap benar.
            return (state.currentRot % 180) == (state.targetRot % 180);
        }
        else
        {
            // Pipa belok harus persis sama rotasinya
            return state.currentRot == state.targetRot;
        }
    }

    private void CheckWinCondition()
    {
        bool allFixed = true;
        foreach (var state in allPipes)
        {
            if (!IsPipeCorrect(state))
            {
                allFixed = false;
                break;
            }
        }

        if (allFixed)
        {
            // Menang!
            foreach (var state in allPipes)
            {
                state.button.interactable = false;
                state.button.GetComponent<Image>().color = fixedColor;
            }
            WinMicrogame(200);
        }
    }
}
