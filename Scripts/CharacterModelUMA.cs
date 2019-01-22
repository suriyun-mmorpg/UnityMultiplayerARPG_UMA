using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(DynamicCharacterAvatar))]
    public class CharacterModelUMA : CharacterModel
    {
        private DynamicCharacterAvatar cacheUmaAvatar;
        public DynamicCharacterAvatar CacheUmaAvatar
        {
            get
            {
                if (cacheUmaAvatar == null)
                    cacheUmaAvatar = GetComponent<DynamicCharacterAvatar>();
                return cacheUmaAvatar;
            }
        }

        private UMAData cacheUmaData;
        public UMAData CacheUmaData
        {
            get
            {
                if (cacheUmaData == null)
                    cacheUmaData = GetComponent<UMAData>();
                return cacheUmaData;
            }
        }

        public bool IsUmaCharacterCreated { get; private set; }
        public bool IsInitializedUMA { get; private set; }
        public System.Action onUmaCharacterCreated;
        private UmaAvatarData? applyingAvatarData;
        private Coroutine applyCoroutine;
        private EquipWeapons tempEquipWeapons;
        private IList<CharacterItem> tempEquipItems;

        private readonly HashSet<string> equipWeaponUsedSlots = new HashSet<string>();
        private readonly HashSet<string> equipItemUsedSlots = new HashSet<string>();
        private readonly List<GameObject> equipWeaponObjects = new List<GameObject>();
        private readonly List<GameObject> equipItemObjects = new List<GameObject>();

        private void Start()
        {
            InitializeUMA();
        }

        public override void SetEquipWeapons(EquipWeapons equipWeapons)
        {
            tempEquipWeapons = equipWeapons;

            if (!IsUmaCharacterCreated)
                return;

            ClearObjectsAndSlots(equipWeaponUsedSlots, equipWeaponObjects);

            if (CacheUmaAvatar.activeRace == null ||
                CacheUmaAvatar.activeRace.racedata == null ||
                equipWeapons == null)
                return;

            string raceName = CacheUmaAvatar.activeRace.racedata.raceName;
            Item tempEquipmentItem;
            UmaReceiptSlot[] receiptSlots;

            if (equipWeapons.rightHand != null)
            {
                tempEquipmentItem = equipWeapons.rightHand.GetEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    SetEquipmentObject(equipItemObjects, tempEquipmentItem.equipmentModels);
                    if (tempEquipmentItem.CacheUmaReceiptSlot.TryGetValue(raceName, out receiptSlots))
                        SetSlot(equipItemUsedSlots, receiptSlots);
                }
            }
            if (equipWeapons.leftHand != null)
            {
                tempEquipmentItem = equipWeapons.leftHand.GetEquipmentItem();
                if (tempEquipmentItem != null)
                {
                    SetEquipmentObject(equipItemObjects, tempEquipmentItem.equipmentModels);
                    if (tempEquipmentItem.CacheUmaReceiptSlot.TryGetValue(raceName, out receiptSlots))
                        SetSlot(equipItemUsedSlots, receiptSlots);
                }
            }
        }

        public override void SetEquipItems(IList<CharacterItem> equipItems)
        {
            tempEquipItems = equipItems;

            if (!IsUmaCharacterCreated)
                return;

            ClearObjectsAndSlots(equipItemUsedSlots, equipItemObjects);

            if (CacheUmaAvatar.activeRace == null ||
                CacheUmaAvatar.activeRace.racedata == null ||
                equipItems == null)
                return;

            string raceName = CacheUmaAvatar.activeRace.racedata.raceName;
            Item tempEquipmentItem;
            UmaReceiptSlot[] receiptSlots;

            foreach (CharacterItem equipItem in equipItems)
            {
                tempEquipmentItem = equipItem.GetEquipmentItem();
                if (tempEquipmentItem == null)
                    continue;

                SetEquipmentObject(equipItemObjects, tempEquipmentItem.equipmentModels);

                if (!tempEquipmentItem.CacheUmaReceiptSlot.TryGetValue(raceName, out receiptSlots))
                    continue;

                SetSlot(equipItemUsedSlots, receiptSlots);
            }
            // Update avatar
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
        }

        private void ClearObjectsAndSlots(HashSet<string> usedSlotsSet, List<GameObject> objectsList)
        {
            foreach (string usedSlotSet in usedSlotsSet)
            {
                CacheUmaAvatar.ClearSlot(usedSlotSet);
            }
            usedSlotsSet.Clear();

            foreach (GameObject objectList in objectsList)
            {
                Destroy(objectList);
            }
            objectsList.Clear();
        }

        private void SetEquipmentObject(List<GameObject> objectsList, EquipmentModel[] equipmentModels)
        {
            if (objectsList == null || equipmentModels == null || equipmentModels.Length == 0)
                return;

            GameObject boneObj;
            GameObject newObj;
            foreach (EquipmentModel equipmentModel in equipmentModels)
            {
                if (string.IsNullOrEmpty(equipmentModel.equipSocket) ||
                    equipmentModel.model == null)
                {
                    // If data is empty, skip it
                    continue;
                }

                boneObj = CacheUmaData.GetBoneGameObject(equipmentModel.equipSocket);
                if (boneObj == null)
                    continue;
                
                newObj = Instantiate(equipmentModel.model);
                newObj.transform.SetParent(boneObj.transform, false);
                newObj.transform.localPosition = Vector3.zero;
                newObj.transform.localEulerAngles = Vector3.zero;
                newObj.transform.localScale = Vector3.one;
                newObj.gameObject.SetActive(true);
                newObj.gameObject.SetLayerRecursively(gameInstance.characterLayer.LayerIndex, true);
                newObj.RemoveComponentsInChildren<Collider>(false);

                objectsList.Add(newObj);
            }
        }

        private void SetSlot(HashSet<string> usedSlotsSet, UmaReceiptSlot[] receiptSlots)
        {
            if (usedSlotsSet == null || receiptSlots == null || receiptSlots.Length == 0)
                return;

            foreach (UmaReceiptSlot receiptSlot in receiptSlots)
            {
                if (receiptSlot.recipe == null ||
                    string.IsNullOrEmpty(receiptSlot.recipe.wardrobeSlot) ||
                    usedSlotsSet.Contains(receiptSlot.recipe.wardrobeSlot))
                {
                    // If data is empty or slot already used, skip it
                    continue;
                }
                usedSlotsSet.Add(receiptSlot.recipe.wardrobeSlot);
                CacheUmaAvatar.SetSlot(receiptSlot.recipe);
            }
        }

        public void InitializeUMA()
        {
            if (IsInitializedUMA)
                return;
            IsInitializedUMA = true;
            CacheUmaAvatar.raceAnimationControllers.defaultAnimationController = CacheAnimatorController;
            CacheUmaAvatar.CharacterCreated.RemoveListener(OnUmaCharacterCreated);
            CacheUmaAvatar.CharacterCreated.AddListener(OnUmaCharacterCreated);
        }

        private void OnUmaCharacterCreated(UMAData data)
        {
            IsUmaCharacterCreated = true;
            ApplyPendingAvatarData();
            if (onUmaCharacterCreated != null)
                onUmaCharacterCreated.Invoke();
        }
        
        public void ApplyUmaAvatar(UmaAvatarData avatarData)
        {
            if (CacheUmaAvatar == null)
            {
                Debug.LogWarning("[CharacterModelUMA] Uma avatar or applier is empty, cannot change avatar appearances");
                return;
            }
            InitializeUMA();
            UmaRace race = gameInstance.umaRaces[avatarData.raceIndex];
            UmaRaceGender gender = race.genders[avatarData.genderIndex];
            CacheUmaAvatar.ChangeRace(gender.raceData.raceName);
            if (!IsUmaCharacterCreated)
            {
                applyingAvatarData = avatarData;
                return;
            }
            if (applyCoroutine != null)
                StopCoroutine(applyCoroutine);
            applyCoroutine = StartCoroutine(ApplyUmaAvatarRoutine(avatarData));
        }

        IEnumerator ApplyUmaAvatarRoutine(UmaAvatarData avatarData)
        {
            int i;
            UmaRace race = gameInstance.umaRaces[avatarData.raceIndex];
            UmaRaceGender gender = race.genders[avatarData.genderIndex];
            // Set character hair, beard, eyebrows (or other things up to your settings)
            if (avatarData.slots != null)
            {
                Dictionary<string, List<UMATextRecipe>> recipes = CacheUmaAvatar.AvailableRecipes;
                string slotName;
                for (i = 0; i < avatarData.slots.Length; ++i)
                {
                    slotName = gender.customizableSlots[i].name;
                    CacheUmaAvatar.SetSlot(recipes[slotName][avatarData.slots[i]]);
                }
            }
            // Set character dna
            if (avatarData.dnas != null)
            {
                Dictionary<string, DnaSetter> dnas = CacheUmaAvatar.GetDNA();
                List<string> dnaNames = new List<string>(dnas.Keys);
                dnaNames.Sort();
                string dnaName;
                for (i = 0; i < avatarData.dnas.Length; ++i)
                {
                    dnaName = dnaNames[i];
                    dnas[dnaName].Set(avatarData.dnas[i] * 0.01f);
                }
            }
            // Set skin color, eyes color, hair color (or other things up to your settings)
            if (avatarData.colors != null)
            {
                SharedColorTable colorTable;
                for (i = 0; i < avatarData.colors.Length; ++i)
                {
                    colorTable = race.colorTables[i];
                    CacheUmaAvatar.SetColor(colorTable.sharedColorName, colorTable.colors[avatarData.colors[i]]);
                }
            }
            yield return null;
            // Set equip items if it is already set
            if (tempEquipItems != null)
                SetEquipItems(tempEquipItems);
            // Update avatar
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
        }

        public void ApplyPendingAvatarData()
        {
            if (applyingAvatarData != null)
            {
                ApplyUmaAvatar(applyingAvatarData.Value);
                applyingAvatarData = null;
            }
        }
    }
}
