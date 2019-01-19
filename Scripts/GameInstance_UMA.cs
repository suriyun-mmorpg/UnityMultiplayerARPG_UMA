using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    public partial class GameInstance
    {
        [Header("UMA Setting")]
        // Race choice
        public UmaRace[] umaRaces = new UmaRace[]
        {
            new UmaRace()
            {
                name = "Human",
                // Available gender
                genders = new UmaRaceGender[]
                {
                    new UmaRaceGender()
                    {
                        name = "Male",
                        customizableSlots = new UmaCustomizableSlot[]
                        {
                            new UmaCustomizableSlot() { name = "Hair" },
                            new UmaCustomizableSlot() { name = "Beard" },
                            new UmaCustomizableSlot() { name = "Eyebrows" },
                            new UmaCustomizableSlot() { name = "Eyes" },
                            new UmaCustomizableSlot() { name = "Face" },
                            new UmaCustomizableSlot() { name = "Ears" },
                        }
                    },
                    new UmaRaceGender()
                    {
                        name = "Female",
                        customizableSlots = new UmaCustomizableSlot[]
                        {
                            new UmaCustomizableSlot() { name = "Hair" },
                            new UmaCustomizableSlot() { name = "Eyebrows" },
                            new UmaCustomizableSlot() { name = "Eyes" },
                            new UmaCustomizableSlot() { name = "Face" },
                            new UmaCustomizableSlot() { name = "Ears" },
                        }
                    },
                },
                // Color settings
                colors = new UmaColorList[]
                {
                    new UmaColorList()
                    {
                        name = "Hair",
                    },
                    new UmaColorList()
                    {
                        name = "Eyes",
                    },
                    new UmaColorList()
                    {
                        name = "Skin",
                    },
                },
            }
        };
    }

    [System.Serializable]
    public struct UmaRace
    {
        public string name;
        public UmaRaceGender[] genders;
        public UmaColorList[] colors;
    }

    [System.Serializable]
    public struct UmaRaceGender
    {
        public string name;
        public RaceData race;
        public UmaCustomizableSlot[] customizableSlots;
    }

    [System.Serializable]
    public struct UmaColorList
    {
        public string name;
        public SharedColorTable colorTable;
    }

    [System.Serializable]
    public struct UmaCustomizableSlot
    {
        public string name;
        public UMATextRecipe[] recipe;
    }
}
