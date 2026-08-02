using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Microgame: De Onderzoeker - Zeilmaker
/// Mechanic: Connect the Dots. Tap the nodes in sequential order to sew the sail.
/// </summary>
public class MG_SewSail : MicrogameBase
{
    [Header("Sewing Nodes")]
    [Tooltip("Susun node berurutan dari pertama (0) sampai terakhir")]
    [SerializeField] private List<Button> nodes;
    
    [Tooltip("Daftar objek garis/tali yang menghubungkan titik. Element 0 = tali antara Node 1 & 2.")]
    [SerializeField] private List<GameObject> threads;
    
    [Header("Visuals")]
    [Tooltip("Centang jika jahitan terpisah-pisah (1-2, lalu 3-4, lalu 5-6). Hilangkan centang jika menyambung terus seperti rasi bintang (1-2-3-4-5).")]
    [SerializeField] private bool connectInPairs = true;
    
    [SerializeField] private Color pendingColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;

    // Untuk mode berurutan (Continuous)
    private int nextExpectedIndex = 0;
    
    // Untuk mode pasangan (Pairs)
    private bool[] nodeClicked;
    private int pairsCompleted = 0;

    private void Awake()
    {
        // Sembunyikan benang sejak detik pertama scene dimuat
        if (threads != null)
        {
            foreach (var thread in threads)
            {
                if (thread != null)
                    thread.SetActive(false);
            }
        }
    }

    protected override void OnSetup()
    {
        nextExpectedIndex = 0;
        pairsCompleted = 0;
        nodeClicked = new bool[nodes.Count];

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null) continue;

            nodes[i].GetComponent<Image>().color = pendingColor;
            
            int index = i;
            nodes[i].onClick.RemoveAllListeners();
            nodes[i].onClick.AddListener(() => OnNodeClicked(index));
        }

        // Sembunyikan semua benang di awal game (jika di-restart)
        foreach (var thread in threads)
        {
            if (thread != null)
                thread.SetActive(false);
        }
    }

    private void OnNodeClicked(int index)
    {
        if (!isPlaying || isCompleted) return;

        if (connectInPairs)
        {
            HandlePairMode(index);
        }
        else
        {
            HandleContinuousMode(index);
        }
    }

    private void HandlePairMode(int index)
    {
        if (nodeClicked[index]) return; // Sudah ditekan sebelumnya

        // Tandai titik ini sudah ditekan
        nodeClicked[index] = true;
        nodes[index].GetComponent<Image>().color = completedColor;

        // Cari tahu siapa pasangannya (jika index 0 pasangannya 1. Jika index 1 pasangannya 0).
        int partnerIndex = (index % 2 == 0) ? (index + 1) : (index - 1);

        // Jika pasangannya juga sudah ditekan, berarti 1 jahitan selesai!
        if (partnerIndex < nodes.Count && nodeClicked[partnerIndex])
        {
            int threadIndex = index / 2;
            if (threadIndex < threads.Count && threads[threadIndex] != null)
            {
                threads[threadIndex].SetActive(true);
            }

            pairsCompleted++;

            // Menang jika semua pasangan selesai (jumlah node / 2)
            if (pairsCompleted >= nodes.Count / 2)
            {
                WinMicrogame();
            }
        }
    }

    private void HandleContinuousMode(int index)
    {
        if (index == nextExpectedIndex)
        {
            // Benar! Titik diklik berurutan
            nodes[index].GetComponent<Image>().color = completedColor;
            
            // Mode Menyambung: Tampilkan benang yang terhubung ke titik sebelumnya
            if (index > 0 && (index - 1) < threads.Count)
            {
                if (threads[index - 1] != null)
                {
                    threads[index - 1].SetActive(true);
                }
            }

            nextExpectedIndex++;

            if (nextExpectedIndex >= nodes.Count)
            {
                WinMicrogame();
            }
        }
        else if (index > nextExpectedIndex)
        {
            // Salah ketuk (lompat)! Reset semua dari awal.
            ResetNodesContinuous();
        }
    }

    private void ResetNodesContinuous()
    {
        nextExpectedIndex = 0;
        foreach (var node in nodes)
        {
            if (node != null)
            {
                node.GetComponent<Image>().color = pendingColor;
            }
        }

        // Sembunyikan benang kembali
        foreach (var thread in threads)
        {
            if (thread != null)
                thread.SetActive(false);
        }
    }
}
