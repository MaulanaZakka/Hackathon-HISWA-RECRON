using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Contains all static data about LOBX profiles and HISWA-RECRON jobs.
/// Used by UI to display profile cards and results info.
/// All text is in Dutch as required by the briefing.
/// </summary>
public static class ProfileDatabase
{
    // ═══════════════════════════════════════════
    //  PROFILE INFO
    // ═══════════════════════════════════════════

    [System.Serializable]
    public class ProfileInfo
    {
        public ProfileType type;
        public string nameDutch;           // e.g. "De Doener"
        public string nameEnglish;         // e.g. "The Doer" (internal reference)
        public string description;         // Short description in Dutch
        public string emoji;               // Visual identifier
        public Color themeColor;
        public List<JobInfo> jobs;
    }

    [System.Serializable]
    public class JobInfo
    {
        public string microgameID;
        public string jobTitleDutch;       // Official Dutch job title
        public string jobTitleIndonesian;  // Indonesian translation (dev reference)
        public string sector;             // "Watersport & Maritiem" or "Vakantieparken & Recreatie"
        public string shortDescription;   // What this job does, in simple Dutch
    }

    // ═══════════════════════════════════════════
    //  DATABASE
    // ═══════════════════════════════════════════

    private static Dictionary<ProfileType, ProfileInfo> profiles;

    public static ProfileInfo GetProfile(ProfileType type)
    {
        InitializeIfNeeded();
        return profiles.ContainsKey(type) ? profiles[type] : null;
    }

    public static List<ProfileInfo> GetAllProfiles()
    {
        InitializeIfNeeded();
        var list = new List<ProfileInfo>();
        foreach (var kvp in profiles)
            list.Add(kvp.Value);
        return list;
    }

    private static void InitializeIfNeeded()
    {
        if (profiles != null) return;

        profiles = new Dictionary<ProfileType, ProfileInfo>
        {
            // ── DE DOENER ──
            {
                ProfileType.DeDoener, new ProfileInfo
                {
                    type = ProfileType.DeDoener,
                    nameDutch = "De Doener",
                    nameEnglish = "The Doer",
                    description = "Jij houdt van aanpakken! Met je handen werken, dingen repareren en bouwen - dat is jouw ding.",
                    emoji = "🔧",
                    themeColor = new Color(0.90f, 0.30f, 0.20f), // Red-orange
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_FixEngine",
                            jobTitleDutch = "Monteur Motoren",
                            jobTitleIndonesian = "Teknisi Mesin Kapal",
                            sector = "Watersport & Jachthavens",
                            shortDescription = "Je repareert en onderhoudt bootmotoren, dieselmotoren en jetski's."
                        },
                        new JobInfo
                        {
                            microgameID = "MG_ConnectCables",
                            jobTitleDutch = "Elektromonteur",
                            jobTitleIndonesian = "Teknisi Listrik Maritim",
                            sector = "Watersport & Jachthavens",
                            shortDescription = "Je verzorgt de elektrische systemen en navigatie aan boord van schepen."
                        },
                        new JobInfo
                        {
                            microgameID = "MG_FixPipes",
                            jobTitleDutch = "Medewerker Technische Dienst",
                            jobTitleIndonesian = "Staf Perbaikan Resor",
                            sector = "Vakantieparken & Recreatie",
                            shortDescription = "Je repareert huisjes, leidingen en elektra op het vakantiepark."
                        }
                    }
                }
            },

            // ── DE ORGANISATOR ──
            {
                ProfileType.DeOrganisator, new ProfileInfo
                {
                    type = ProfileType.DeOrganisator,
                    nameDutch = "De Organisator",
                    nameEnglish = "The Organizer",
                    description = "Jij houdt van orde en structuur. Plannen maken en alles op zijn plek - daar word jij blij van.",
                    emoji = "📋",
                    themeColor = new Color(0.20f, 0.50f, 0.85f), // Blue
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_ManageHarbor",
                            jobTitleDutch = "Havenmeester",
                            jobTitleIndonesian = "Syahbandar / Pengelola Dermaga",
                            sector = "Watersport & Jachthavens",
                            shortDescription = "Je regelt het scheepvaartverkeer, verwelkomt gasten en onderhoudt de haven."
                        },
                        new JobInfo
                        {
                            microgameID = "MG_CheckInGuest",
                            jobTitleDutch = "Medewerker Front Office & Shop",
                            jobTitleIndonesian = "Resepsionis & Toko Suvenir",
                            sector = "Vakantieparken & Recreatie",
                            shortDescription = "Je verwelkomt gasten bij de receptie, regelt reserveringen en beheert de winkel."
                        }
                    }
                }
            },

            // ── DE CREATIEVELING ──
            {
                ProfileType.DeCreatieveling, new ProfileInfo
                {
                    type = ProfileType.DeCreatieveling,
                    nameDutch = "De Creatieveling",
                    nameEnglish = "The Creative",
                    description = "Jij zit vol ideeën! Shows bedenken, feesten organiseren en mensen laten lachen - dat is jouw talent.",
                    emoji = "🎨",
                    themeColor = new Color(0.85f, 0.30f, 0.75f), // Magenta/Pink
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_EntertainGuests",
                            jobTitleDutch = "Animatiemedewerker",
                            jobTitleIndonesian = "Staf Hiburan / Entertainer",
                            sector = "Vakantieparken & Recreatie",
                            shortDescription = "Je vermaakt gasten, leidt kinderspellen en organiseert avondshows."
                        }
                    }
                }
            },

            // ── DE HELPER ──
            {
                ProfileType.DeHelper, new ProfileInfo
                {
                    type = ProfileType.DeHelper,
                    nameDutch = "De Helper",
                    nameEnglish = "The Helper",
                    description = "Jij helpt graag anderen! Mensen begeleiden, uitleggen en ondersteunen - daar krijg jij energie van.",
                    emoji = "🤝",
                    themeColor = new Color(0.20f, 0.75f, 0.40f), // Green
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_TeachSailing",
                            jobTitleDutch = "Zeil / Windsurf Instructeur",
                            jobTitleIndonesian = "Instruktur Berlayar / Windsurf",
                            sector = "Watersport & Jachthavens",
                            shortDescription = "Je leert toeristen en kinderen zeilen, windsurfen of wingfoilen."
                        },
                        new JobInfo
                        {
                            microgameID = "MG_GuideOutbound",
                            jobTitleDutch = "Medewerker Sport & Activiteiten",
                            jobTitleIndonesian = "Pemandu Outbound / Olahraga",
                            sector = "Vakantieparken & Recreatie",
                            shortDescription = "Je begeleidt buitenactiviteiten zoals klimmen, fietsverhuur en watersporten."
                        }
                    }
                }
            },

            // ── DE ONDERNEMER ──
            {
                ProfileType.DeOndernemer, new ProfileInfo
                {
                    type = ProfileType.DeOndernemer,
                    nameDutch = "De Ondernemer",
                    nameEnglish = "The Entrepreneur",
                    description = "Jij wilt dingen neerzetten! Een mooie plek creëren, kansen zien en iets opbouwen - dat past bij jou.",
                    emoji = "💼",
                    themeColor = new Color(0.90f, 0.65f, 0.15f), // Orange/Gold
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_BeautifyResort",
                            jobTitleDutch = "Medewerker Groenvoorziening",
                            jobTitleIndonesian = "Penata Taman Resor",
                            sector = "Vakantieparken & Recreatie",
                            shortDescription = "Je verzorgt de groene omgeving van het park: grasmaaien, beplanting en natuur onderhouden."
                        }
                    }
                }
            },

            // ── DE ONDERZOEKER ──
            {
                ProfileType.DeOnderzoeker, new ProfileInfo
                {
                    type = ProfileType.DeOnderzoeker,
                    nameDutch = "De Onderzoeker",
                    nameEnglish = "The Investigator",
                    description = "Jij wilt weten hoe het werkt! Precies meten, slim ontwerpen en puzzelen - daar ben jij goed in.",
                    emoji = "🔬",
                    themeColor = new Color(0.45f, 0.35f, 0.80f), // Purple
                    jobs = new List<JobInfo>
                    {
                        new JobInfo
                        {
                            microgameID = "MG_SewSail",
                            jobTitleDutch = "Zeilmaker",
                            jobTitleIndonesian = "Pembuat / Perancang Layar Kapal",
                            sector = "Watersport & Jachthavens",
                            shortDescription = "Je ontwerpt en repareert zeilen en scheepszeildoek met hoge precisie."
                        }
                    }
                }
            }
        };
    }
}
