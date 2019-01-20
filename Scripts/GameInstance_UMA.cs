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
                        customizableSlots = new string[]
                        {
                            "Hair",
                            "Eyebrows",
                            "Eyes",
                            "Face",
                            "Ears",
                        }
                    },
                    new UmaRaceGender()
                    {
                        name = "Female",
                        customizableSlots = new string[]
                        {
                            "Hair",
                            "Eyebrows",
                            "Eyes",
                            "Face",
                            "Ears",
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
        public string[] customizableSlots;
    }
}
