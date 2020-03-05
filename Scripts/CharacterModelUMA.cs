using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [RequireComponent(typeof(DynamicCharacterAvatar))]
    [System.Obsolete("`Character Model` is deprecate and stopped development, use context menu to convert to newer character model")]
    public sealed class CharacterModelUMA : CharacterModel, ICharacterModelUma
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
        public System.Action OnUmaCharacterCreated { get; set; }
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
            SetClipBasedOnWeapon(equipWeapons);

            if (!IsUmaCharacterCreated)
                return;

            ClearObjectsAndSlots(equipWeaponUsedSlots, equipWeaponObjects);

            if (CacheUmaAvatar.activeRace == null ||
                CacheUmaAvatar.activeRace.racedata == null ||
                equipWeapons == null)
                return;

            string raceName = CacheUmaAvatar.activeRace.racedata.raceName;
            IEquipmentItem tempEquipmentItem;
            UMATextRecipe[] receipes;
            // Setup right hand weapon
            if (equipWeapons.rightHand != null)
            {
                tempEquipmentItem = equipWeapons.rightHand.GetWeaponItem();
                if (tempEquipmentItem != null)
                {
                    SetEquipmentObject(equipWeaponObjects, tempEquipmentItem.EquipmentModels, equipWeapons.rightHand.level, out rightHandEquipmentEntity);
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(equipWeaponUsedSlots, receipes);
                }
            }
            // Setup left hand weapon
            if (equipWeapons.leftHand != null)
            {
                // Weapon
                tempEquipmentItem = equipWeapons.leftHand.GetWeaponItem();
                if (tempEquipmentItem != null)
                {
                    SetEquipmentObject(equipWeaponObjects, (tempEquipmentItem as IWeaponItem).OffHandEquipmentModels, equipWeapons.leftHand.level, out leftHandEquipmentEntity);
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(equipWeaponUsedSlots, receipes);
                }
                // Shield
                tempEquipmentItem = equipWeapons.leftHand.GetShieldItem();
                if (tempEquipmentItem != null)
                {
                    SetEquipmentObject(equipWeaponObjects, tempEquipmentItem.EquipmentModels, equipWeapons.leftHand.level, out leftHandEquipmentEntity);
                    if (tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                        SetSlot(equipWeaponUsedSlots, receipes);
                }
            }
            // Update avatar
            CacheUmaAvatar.BuildCharacter(true);
            CacheUmaAvatar.ForceUpdate(true, true, true);
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
            IEquipmentItem tempEquipmentItem;
            UMATextRecipe[] receipes;
            BaseEquipmentEntity tempEquipmentEntity;
            foreach (CharacterItem equipItem in equipItems)
            {
                tempEquipmentItem = equipItem.GetEquipmentItem();
                if (tempEquipmentItem == null)
                    continue;

                SetEquipmentObject(equipItemObjects, tempEquipmentItem.EquipmentModels, equipItem.level, out tempEquipmentEntity);

                if (!tempEquipmentItem.UmaRecipeSlot.TryGetValue(raceName, out receipes))
                    continue;

                SetSlot(equipItemUsedSlots, receipes);
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

        private void SetEquipmentObject(List<GameObject> objectsList, EquipmentModel[] equipmentModels, int level, out BaseEquipmentEntity equipmentEntity)
        {
            equipmentEntity = null;
            if (objectsList == null || equipmentModels == null || equipmentModels.Length == 0)
                return;

            GameObject boneObj;
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

                tempEquipmentObject = Instantiate(equipmentModel.model);
                tempEquipmentObject.transform.SetParent(boneObj.transform, false);
                tempEquipmentObject.transform.localPosition = Vector3.zero;
                tempEquipmentObject.transform.localEulerAngles = Vector3.zero;
                tempEquipmentObject.transform.localScale = Vector3.one;
                tempEquipmentObject.gameObject.SetActive(true);
                tempEquipmentObject.gameObject.SetLayerRecursively(CurrentGameInstance.characterLayer.LayerIndex, true);
                tempEquipmentObject.RemoveComponentsInChildren<Collider>(false);
                tempEquipmentEntity = tempEquipmentObject.GetComponent<BaseEquipmentEntity>();
                if (tempEquipmentEntity != null)
                {
                    tempEquipmentEntity.Level = level;
                    if (equipmentEntity == null)
                        equipmentEntity = tempEquipmentEntity;
                }

                objectsList.Add(tempEquipmentObject);
            }
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
            CacheUmaAvatar.raceAnimationControllers.defaultAnimationController = CacheAnimatorController;
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
            if (tempEquipWeapons != null)
                SetEquipWeapons(tempEquipWeapons);
            if (tempEquipItems != null)
                SetEquipItems(tempEquipItems);

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
            if (applyingAvatarData != null)
            {
                ApplyUmaAvatar(applyingAvatarData.Value);
                applyingAvatarData = null;
            }
        }

#if UNITY_EDITOR
        public override void ConvertToNewerCharacterModel()
        {
            if (animatorType != AnimatorType.Animator)
            {
                Debug.LogError("[Character Model UMA] only animator can be converted");
                return;
            }
            AnimatorCharacterModelUMA model = gameObject.GetComponent<AnimatorCharacterModelUMA>();
            if (!model)
                model = gameObject.AddComponent<AnimatorCharacterModelUMA>();
            model.skinnedMeshRenderer = skinnedMeshRenderer;
            model.weaponAnimations = weaponAnimations;
            model.skillAnimations = skillAnimations;
            model.defaultAnimations = new DefaultAnimations()
            {
                idleClip = defaultAnimatorData.idleClip,
                moveClip = defaultAnimatorData.moveClip,
                moveBackwardClip = defaultAnimatorData.moveBackwardClip,
                moveLeftClip = defaultAnimatorData.moveLeftClip,
                moveRightClip = defaultAnimatorData.moveRightClip,
                moveForwardLeftClip = defaultAnimatorData.moveForwardLeftClip,
                moveForwardRightClip = defaultAnimatorData.moveForwardRightClip,
                moveBackwardLeftClip = defaultAnimatorData.moveBackwardLeftClip,
                moveBackwardRightClip = defaultAnimatorData.moveBackwardRightClip,
                jumpClip = defaultAnimatorData.jumpClip,
                fallClip = defaultAnimatorData.fallClip,
                hurtClip = defaultAnimatorData.hurtClip,
                deadClip = defaultAnimatorData.deadClip,
                rightHandAttackAnimations = defaultAttackAnimations,
                leftHandAttackAnimations = defaultAttackAnimations,
                rightHandReloadAnimation = defaultReloadAnimation,
                leftHandReloadAnimation = defaultReloadAnimation,
                skillCastClip = defaultSkillCastClip,
                skillActivateAnimation = defaultSkillActivateAnimation,
            };
            model.hiddingObjects = hiddingObjects;
            model.hiddingRenderers = hiddingRenderers;
            model.fpsHiddingObjects = fpsHiddingObjects;
            model.fpsHiddingRenderers = fpsHiddingRenderers;
            model.effectContainers = effectContainers;
            model.equipmentContainers = equipmentContainers;
            EditorUtility.SetDirty(model);
            Debug.Log("[Character Model UMA] Converted, you can remove this component.");
        }
#endif
    }
}
