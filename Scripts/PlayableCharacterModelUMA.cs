using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using Cysharp.Threading.Tasks;

namespace MultiplayerARPG.GameData.Model.Playables
{
    [RequireComponent(typeof(DynamicCharacterAvatar))]
    public class PlayableCharacterModelUMA : PlayableCharacterModel, ICharacterModelUma
    {
        private DynamicCharacterAvatar _cacheUmaAvatar;
        public DynamicCharacterAvatar CacheUmaAvatar
        {
            get
            {
                if (_cacheUmaAvatar == null)
                    _cacheUmaAvatar = GetComponent<DynamicCharacterAvatar>();
                return _cacheUmaAvatar;
            }
        }

        private UMAData _cacheUmaData;
        public UMAData CacheUmaData
        {
            get
            {
                if (_cacheUmaData == null)
                    _cacheUmaData = GetComponent<UMAData>();
                return _cacheUmaData;
            }
        }

        public bool IsUmaCharacterCreated { get; private set; }
        public bool IsInitializedUMA { get; private set; }
        public System.Action OnUmaCharacterCreated { get; set; }

        private UmaAvatarData? _applyingAvatarData;
        private Coroutine _applyCoroutine;

        private readonly HashSet<string> _equipWeaponUsedSlots = new HashSet<string>();
        private readonly HashSet<string> _equipItemUsedSlots = new HashSet<string>();
        private readonly List<GameObject> _equipRightWeaponObjects = new List<GameObject>();
        private readonly List<GameObject> _equipLeftWeaponObjects = new List<GameObject>();
        private readonly List<GameObject> _equipItemObjects = new List<GameObject>();
        private EquipWeapons _prevEquipWeapons;

        protected override void Awake()
        {
            base.Awake();
            if (animator != null && animator.runtimeAnimatorController == null)
                animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("__EmptyAnimatorControllerForUMAIntegration");
        }

        protected override void Start()
        {
            base.Start();
            InitializeUMA();
        }

        public override async void SetEquipItems(IList<CharacterItem> equipItems, IList<EquipWeapons> selectableWeaponSets, byte equipWeaponSet, bool isWeaponsSheathed)
        {
            if (!IsUmaCharacterCreated)
            {
                // Store data to re-setup later
                EquipItems = equipItems;
                SelectableWeaponSets = selectableWeaponSets;
                EquipWeaponSet = equipWeaponSet;
                IsWeaponsSheathed = isWeaponsSheathed;
                return;
            }

            ClearObjectsAndSlots(_equipItemUsedSlots, _equipItemObjects);

            if (CacheUmaAvatar.activeRace == null ||
                CacheUmaAvatar.activeRace.racedata == null)
            {
                // Store data to re-setup later
                EquipItems = equipItems;
                SelectableWeaponSets = selectableWeaponSets;
                EquipWeaponSet = equipWeaponSet;
                IsWeaponsSheathed = isWeaponsSheathed;
                return;
            }

            string raceName = CacheUmaAvatar.activeRace.racedata.raceName;
            IEquipmentItem tempEquipmentItem;
            UMATextRecipe[] receipes;
            BaseEquipmentEntity tempEquipmentEntity;
            foreach (CharacterItem equipItem in equipItems)
            {
                tempEquipmentItem = equipItem.GetEquipmentItem();
                if (tempEquipmentItem == null)
                    continue;

                tempEquipmentEntity = await SetEquipmentObject(_equipItemObjects, tempEquipmentItem.EquipmentModels, equipItem.level);

                if (!tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                    continue;

                SetSlot(_equipItemUsedSlots, receipes);
            }

            // Prepare equip weapons
            EquipWeapons newEquipWeapons;
            if (isWeaponsSheathed || selectableWeaponSets == null || selectableWeaponSets.Count == 0)
            {
                newEquipWeapons = new EquipWeapons();
            }
            else
            {
                if (equipWeaponSet >= selectableWeaponSets.Count)
                {
                    // Issues occuring, so try to simulate data
                    // Create a new list to make sure that changes won't be applied to the source list (the source list must be readonly)
                    selectableWeaponSets = new List<EquipWeapons>(selectableWeaponSets);
                    while (equipWeaponSet >= selectableWeaponSets.Count)
                    {
                        selectableWeaponSets.Add(new EquipWeapons());
                    }
                }
                newEquipWeapons = selectableWeaponSets[equipWeaponSet].Clone();
            }

            bool rightIsDiffer;
            bool leftIsDiffer;
            _prevEquipWeapons.IsDiffer(newEquipWeapons, out rightIsDiffer, out leftIsDiffer);
            _prevEquipWeapons = newEquipWeapons;

            // Update weapon
            BaseEquipmentEntity baseEquipmentEntity;
            // Setup right hand weapon
            if (rightIsDiffer)
            {
                for (int i = 0; i < _equipRightWeaponObjects.Count; ++i)
                {
                    if (_equipRightWeaponObjects[i] == null) continue;
                    Destroy(_equipRightWeaponObjects[i].gameObject);
                }
                _equipRightWeaponObjects.Clear();
                tempEquipmentItem = newEquipWeapons.rightHand.GetWeaponItem();
                if (tempEquipmentItem != null)
                {
                    baseEquipmentEntity = await SetEquipmentObject(_equipRightWeaponObjects, tempEquipmentItem.EquipmentModels, newEquipWeapons.rightHand.level);
                    CacheRightHandEquipmentEntity = baseEquipmentEntity;
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(_equipWeaponUsedSlots, receipes);
                }
            }

            // Setup left hand weapon
            if (leftIsDiffer)
            {
                for (int i = 0; i < _equipLeftWeaponObjects.Count; ++i)
                {
                    if (_equipLeftWeaponObjects[i] == null) continue;
                    Destroy(_equipLeftWeaponObjects[i].gameObject);
                }
                _equipLeftWeaponObjects.Clear();
                // Weapon
                tempEquipmentItem = newEquipWeapons.leftHand.GetWeaponItem();
                if (tempEquipmentItem != null)
                {
                    baseEquipmentEntity = await SetEquipmentObject(_equipLeftWeaponObjects, (tempEquipmentItem as IWeaponItem).OffHandEquipmentModels, newEquipWeapons.leftHand.level);
                    CacheLeftHandEquipmentEntity = baseEquipmentEntity;
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(_equipWeaponUsedSlots, receipes);
                }
                // Shield
                tempEquipmentItem = newEquipWeapons.leftHand.GetShieldItem();
                if (tempEquipmentItem != null)
                {
                    baseEquipmentEntity = await SetEquipmentObject(_equipLeftWeaponObjects, tempEquipmentItem.EquipmentModels, newEquipWeapons.leftHand.level);
                    CacheLeftHandEquipmentEntity = baseEquipmentEntity;
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(_equipWeaponUsedSlots, receipes);
                }
            }

            // Get one equipped weapon from right-hand or left-hand
            IWeaponItem rightWeaponItem = newEquipWeapons.GetRightHandWeaponItem();
            IWeaponItem leftWeaponItem = newEquipWeapons.GetLeftHandWeaponItem();
            if (rightWeaponItem == null)
                rightWeaponItem = leftWeaponItem;
            // Set equipped weapon type, it will be used to get animations by id
            _equippedWeaponType = null;
            if (rightWeaponItem != null)
                _equippedWeaponType = rightWeaponItem.WeaponType;
            if (Behaviour != null)
                Behaviour.SetEquipWeapons(rightWeaponItem, leftWeaponItem, newEquipWeapons.GetLeftHandShieldItem());

            // Update avatar
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);

            EquipItems = equipItems;
            SelectableWeaponSets = selectableWeaponSets;
            EquipWeaponSet = equipWeaponSet;
            IsWeaponsSheathed = isWeaponsSheathed;
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

        private async UniTask<BaseEquipmentEntity> SetEquipmentObject(List<GameObject> objectsList, EquipmentModel[] equipmentModels, int level)
        {
            BaseEquipmentEntity equipmentEntity = null;
            if (objectsList == null || equipmentModels == null || equipmentModels.Length == 0)
                return equipmentEntity;

            GameObject boneObj;
            GameObject tempEquipmentObject;
            BaseEquipmentEntity tempEquipmentEntity;
            foreach (EquipmentModel equipmentModel in equipmentModels)
            {
                GameObject meshPrefab = await equipmentModel.GetMeshPrefab();
                if (string.IsNullOrEmpty(equipmentModel.equipSocket) ||
                    meshPrefab == null)
                {
                    // If data is empty, skip it
                    continue;
                }

                boneObj = CacheUmaData.GetBoneGameObject(equipmentModel.equipSocket);
                if (boneObj == null)
                    continue;

                tempEquipmentObject = Instantiate(meshPrefab);
                tempEquipmentObject.transform.SetParent(boneObj.transform, false);
                tempEquipmentObject.transform.localPosition = equipmentModel.localPosition;
                tempEquipmentObject.transform.localEulerAngles = equipmentModel.localEulerAngles;
                tempEquipmentObject.transform.localScale = equipmentModel.localScale;
                tempEquipmentObject.gameObject.SetActive(true);
                tempEquipmentObject.gameObject.SetLayerRecursively(EquipmentLayer, true);
                tempEquipmentObject.RemoveComponentsInChildren<Collider>(false);
                tempEquipmentEntity = tempEquipmentObject.GetComponent<BaseEquipmentEntity>();
                if (tempEquipmentEntity != null)
                {
                    tempEquipmentEntity.Setup(this, equipmentModel.equipPosition, equipmentModel.item);
                    if (equipmentEntity == null)
                        equipmentEntity = tempEquipmentEntity;
                }

                objectsList.Add(tempEquipmentObject);
            }

            return equipmentEntity;
        }

        private void SetSlot(HashSet<string> usedSlotsSet, UMATextRecipe[] receipes)
        {
            if (usedSlotsSet == null || receipes == null || receipes.Length == 0)
                return;

            foreach (UMATextRecipe receipeSlot in receipes)
            {
                if (receipeSlot == null ||
                    string.IsNullOrEmpty(receipeSlot.wardrobeSlot) ||
                    usedSlotsSet.Contains(receipeSlot.wardrobeSlot))
                {
                    // If data is empty or slot already used, skip it
                    continue;
                }
                usedSlotsSet.Add(receipeSlot.wardrobeSlot);
                CacheUmaAvatar.SetSlot(receipeSlot);
            }
        }

        public void InitializeUMA()
        {
            if (IsInitializedUMA)
                return;
            IsInitializedUMA = true;
            CacheUmaAvatar.CharacterCreated.RemoveListener(OnUmaCharacterCreatedCallback);
            CacheUmaAvatar.CharacterCreated.AddListener(OnUmaCharacterCreatedCallback);
        }

        private void OnUmaCharacterCreatedCallback(UMAData data)
        {
            IsUmaCharacterCreated = true;
            ApplyPendingAvatarData();
            if (OnUmaCharacterCreated != null)
                OnUmaCharacterCreated.Invoke();
        }

        public void ApplyUmaAvatar(UmaAvatarData avatarData)
        {
            if (CacheUmaAvatar == null)
            {
                Debug.LogWarning("[CharacterModelUMA] Uma avatar or applier is empty, cannot change avatar appearances");
                return;
            }
            InitializeUMA();
            if (!IsUmaCharacterCreated)
            {
                _applyingAvatarData = avatarData;
                return;
            }
            if (_applyCoroutine != null)
                StopCoroutine(_applyCoroutine);
            _applyCoroutine = StartCoroutine(ApplyUmaAvatarRoutine(avatarData));
        }

        IEnumerator ApplyUmaAvatarRoutine(UmaAvatarData avatarData)
        {
            int i;
            UmaRace race = CurrentGameInstance.UmaRaces[avatarData.raceIndex];
            UmaRaceGender gender = race.genders[avatarData.genderIndex];
            CacheUmaAvatar.ChangeRace(gender.raceData.raceName);
            yield return null;
            // Set character hair, beard, eyebrows (or other things up to your settings)
            if (avatarData.slots != null)
            {
                Dictionary<string, List<UMATextRecipe>> recipes = CacheUmaAvatar.AvailableRecipes;
                string slotName;
                for (i = 0; i < gender.customizableSlots.Length; ++i)
                {
                    if (i >= avatarData.slots.Length)
                        break;
                    slotName = gender.customizableSlots[i].name;
                    CacheUmaAvatar.SetSlot(recipes[slotName][avatarData.slots[i]]);
                }
            }
            // Set skin color, eyes color, hair color (or other things up to your settings)
            if (avatarData.colors != null)
            {
                SharedColorTable colorTable;
                for (i = 0; i < race.colorTables.Length; ++i)
                {
                    if (i >= avatarData.colors.Length)
                        break;
                    colorTable = race.colorTables[i];
                    CacheUmaAvatar.SetColor(colorTable.sharedColorName, colorTable.colors[avatarData.colors[i]]);
                }
            }
            // Set equip items if it is already set
            SetEquipItems(EquipItems, SelectableWeaponSets, EquipWeaponSet, IsWeaponsSheathed);

            // Update avatar
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
            yield return null;
            // Set character dna
            if (avatarData.dnas != null)
            {
                Dictionary<string, DnaSetter> dnas = null;
                while (dnas == null || dnas.Count == 0)
                {
                    dnas = CacheUmaAvatar.GetDNA();
                    yield return null;
                }
                List<string> dnaNames = new List<string>(dnas.Keys);
                dnaNames.Sort();
                string dnaName;
                for (i = 0; i < dnaNames.Count; ++i)
                {
                    if (i >= avatarData.dnas.Length)
                        break;
                    dnaName = dnaNames[i];
                    dnas[dnaName].Set(avatarData.dnas[i] * 0.01f);
                }
            }
            // Update avatar after set dna 
            CacheUmaAvatar.ForceUpdate(true, false, false);
        }

        public void ApplyPendingAvatarData()
        {
            if (_applyingAvatarData.HasValue)
            {
                ApplyUmaAvatar(_applyingAvatarData.Value);
                _applyingAvatarData = null;
            }
        }
    }
}