using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "UmaAvatarDatabase", menuName = "Create UMA Integration/UmaAvatarDatabase")]
    public class UmaAvatarDatabase : ScriptableObject
    {
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
                            new UmaCustomizableSlot() { name = "Hair", title = "Hair" },
                            new UmaCustomizableSlot() { name = "Eyebrows", title = "Eyebrows" },
                            new UmaCustomizableSlot() { name = "Eyes", title = "Eyes" },
                            new UmaCustomizableSlot() { name = "Face", title = "Face" },
                            new UmaCustomizableSlot() { name = "Ears", title = "Ears" },
                        }
                    },
                    new UmaRaceGender()
                    {
                        name = "Female",
                        customizableSlots = new UmaCustomizableSlot[]
                        {
                            new UmaCustomizableSlot() { name = "Hair", title = "Hair" },
                            new UmaCustomizableSlot() { name = "Eyebrows", title = "Eyebrows" },
                            new UmaCustomizableSlot() { name = "Eyes", title = "Eyes" },
                            new UmaCustomizableSlot() { name = "Face", title = "Face" },
                            new UmaCustomizableSlot() { name = "Ears", title = "Ears" },
                        }
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
        public SharedColorTable[] colorTables;
    }

    [System.Serializable]
    public struct UmaRaceGender
    {
        public string name;
        public RaceData raceData;
        public UmaCustomizableSlot[] customizableSlots;
    }

    [System.Serializable]
    public struct UmaCustomizableSlot
    {
        public string name;
        public string title;
    }
}

