// Copyright (C) 2019-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

using Random = UnityEngine.Random;

namespace CCGKit
{
    /// <summary>
    /// This component is responsible for bootstrapping the game/battle scene. This process
    /// mainly consists on:
    ///     - Creating the player character and the associated UI widgets.
    ///     - Creating the enemy character/s and the associated UI widgets.
    ///     - Starting the turn sequence.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private PlayableCharacterConfiguration playerConfig;
        [SerializeField]
        private List<NonPlayableCharacterConfiguration> enemyConfig;

        [SerializeField]
        private DeckDrawingSystem deckDrawingSystem;
        [SerializeField]
        private HandPresentationSystem handPresentationSystem;
        [SerializeField]
        private TurnManagementSystem turnManagementSystem;
        [SerializeField]
        private CardWithArrowSelectionSystem cardWithArrowSelectionSystem;
        [SerializeField]
        private EnemyBrainSystem enemyBrainSystem;
        [SerializeField]
        private EffectResolutionSystem effectResolutionSystem;
        [SerializeField]
        private PoisonResolutionSystem poisonResolutionSystem;

        [SerializeField]
        private AssetReference characterTemplate;
        [SerializeField]
        private AssetReference enemyTemplate;

        [SerializeField]
        public Transform playerPivot;
        [SerializeField]
        public Transform enemyPivot;

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private ManaWidget manaWidget;
        [SerializeField]
        private DeckWidget deckWidget;
        [SerializeField]
        private DiscardPileWidget discardPileWidget;
        [SerializeField]
        private EndTurnButton endTurnButton;

        [SerializeField]
        private ObjectPool cardPool;
#pragma warning restore 649

        private Camera mainCamera;

        private CardLibrary playerDeck;

        private GameObject player;
        private GameObject enemy;

        //同伴系统新增
        
        [SerializeField]
        private Transform[] allyPivots;
        [SerializeField]
        private AllyCharacterConfiguration allyConfig;
        [SerializeField]
        private List<AllyTemplate> allyTemplates;
        private GameObject[] allys=null;
        private bool allysAllCreated=false;
        private CharacterObject[] allyCharacters=null;
        //草药系统新增
        [SerializeField]
        private Transform HerbPivot;
        private const float herbPosOffset = 1.4f;

        //数据交接新增
        public DataExchangeSystem dataExchangeSystem;
        private void Start()
        {
            mainCamera = Camera.main;

            cardPool.Initialize();
            //数据交接新增
            dataExchangeSystem.downloadData();

            Addressables.InitializeAsync().Completed += op =>
            {
                CreatePlayer(characterTemplate);

                // Use the existing map info (if any) to populate the enemy template. Otherwise, use
                // the one set in the game bootstrap object.
                var template = enemyTemplate;
                /*var info = FindObjectOfType<MapInfo>();
                /if (info != null)
                {
                    template = info.EnemyTemplate;
                    Destroy(info.gameObject);
                }*/
                CreateEnemy(template);
            };
            createAllys();
        }

        //初始化创建玩家角色
        private void CreatePlayer(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<PlayerTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;
                player = Instantiate(template.Prefab, playerPivot);
                Assert.IsNotNull(player);

                playerDeck = template.StartingDeck;

                //玩家基本属性赋值
                var hp = playerConfig.Hp;
                var mana = playerConfig.Mana;
                var power = playerConfig.Power;
                var magic = playerConfig.Magic;
                var armor = playerConfig.Armor;
                var magicResistance = playerConfig.MagicResistance;
                var level = playerConfig.Level;
                hp.Value = template.Hp;
                mana.Value = template.Mana;
                power.Value = template.Power;
                magic.Value = template.Magic;
                armor.Value = template.Armor;
                magicResistance.Value = template.MagicResistance;
                level.Value = template.Level;

                //同伴系统重构
                //显示血量、状态与属性栏
                var widget=CreateHpWidget(playerConfig.HpWidget, player, hp);
                widget.SetActive(true);
                widget=CreateStatusWidget(playerConfig.StatusWidget, player);
                widget.SetActive(true);
                widget = CreateAttributeWidget(playerConfig.AttributeWidget, player, power, magic, armor, magicResistance);
                widget.SetActive(true);

                manaWidget.Initialize(mana);

                //创建动态玩家角色
                var obj = player.GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter
                {
                    Hp = hp,
                    Power = power,
                    Magic = magic,
                    Armor = armor,
                    MagicResistance = magicResistance,
                    Mana = mana,
                    Level = level,
                    Status = playerConfig.Status 
                };
                obj.Character.Status.Value.Clear();
                CreateHerbBag(template.herbLibrary, template.herbPrefab);

                //同伴系统重构
                if (player && enemy && allysAllCreated)
                    InitializeGame();
            };
        }

        //初始化创建敌人角色
        private void CreateEnemy(AssetReference templateRef)
        {
            var handle = Addressables.LoadAssetAsync<EnemyTemplate>(templateRef);
            handle.Completed += op =>
            {
                var template = op.Result;
                enemy = Instantiate(template.Prefab, enemyPivot);
                Assert.IsNotNull(enemy);

                //怪物基本属性复制
                var initialHp = Random.Range(template.HpLow, template.HpHigh + 1);
                var initialPower = Random.Range(template.PowerLow, template.PowerHigh + 1);
                var initialMagic = Random.Range(template.MagicLow, template.MagicHigh + 1);
                var initialArmor = Random.Range(template.ArmorLow, template.ArmorHigh + 1);
                var initialMagicResistance = Random.Range(template.MagicResistanceLow, template.MagicResistanceHigh + 1);
                var hp = enemyConfig[0].Hp;
                var power = enemyConfig[0].Power;
                var magic = enemyConfig[0].Magic;
                var armor = enemyConfig[0].Armor;
                var magicResistance = enemyConfig[0].MagicResistance;
                hp.Value = initialHp;
                power.Value = initialPower;
                magic.Value = initialMagic;
                armor.Value = initialArmor;
                magicResistance.Value = initialMagicResistance;

                //同伴系统重构
                //创建血量,状态栏,意图栏，属性栏
                var widget=CreateHpWidget(enemyConfig[0].HpWidget, enemy, hp);
                widget.SetActive(true);
                widget = CreateStatusWidget(enemyConfig[0].StatusWidget, enemy);
                widget.SetActive(true);
                widget = CreateIntentWidget(enemyConfig[0].IntentWidget, enemy);
                widget.SetActive(true);
                CreateAttributeWidget(enemyConfig[0].AttributeWidget, enemy, power, magic, armor, magicResistance);

                //创建动态敌人角色
                var obj = enemy.GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter 
                {
                    Hp = hp,
                    Power = power,
                    Magic = magic,
                    Armor = armor,
                    MagicResistance = magicResistance,
                    Status = enemyConfig[0].Status
                };
                obj.Character.Status.Value.Clear();

                //同伴系统重构
                if (player && enemy && allysAllCreated)
                    InitializeGame();
            };
        }

        private void createAllys()
        {
            allys = new GameObject[allyTemplates.Count];
            allyCharacters = new CharacterObject[allyTemplates.Count];
            for (int i=0;i<allyTemplates.Count;i++)
            {
                var template = allyTemplates[i];
                allys[i]= Instantiate(allyTemplates[i].Prefab, allyPivots[i]);
                Assert.IsNotNull(allys[i]);

                //同伴基本属性复制 
                //给每个同伴创建事件变量 在变量值修改时触发 并各自引发自己的变化
                IntVariable hp = ScriptableObject.CreateInstance<IntVariable>(); hp.Value = template.hp;hp.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventInt>();
                IntVariable power = ScriptableObject.CreateInstance<IntVariable>(); power.Value = template.power; power.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventInt>();
                IntVariable magic = ScriptableObject.CreateInstance<IntVariable>(); magic.Value = template.magic; magic.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventInt>();
                IntVariable armor = ScriptableObject.CreateInstance<IntVariable>(); armor.Value = template.armor; armor.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventInt>();
                IntVariable magicResistance = ScriptableObject.CreateInstance<IntVariable>(); magicResistance.Value = template.magicResistance; magicResistance.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventInt>();
                StatusVariable status = ScriptableObject.CreateInstance<StatusVariable>();status.ValueChangedEvent = ScriptableObject.CreateInstance<GameEventStatus>();

                //创建allybrain
                var allyBrainSystem=allys[i].AddComponent<AllyBrainSystem>();
                allyBrainSystem.intentChangeEvent = ScriptableObject.CreateInstance<IntentChangeEvent>();
                allyBrainSystem.effectResolutionSystem = effectResolutionSystem;

                //创建血量,状态栏,意图栏，属性栏

                var widget=CreateHpWidget(allyConfig.HpWidget, allys[i], hp);
                GameEventIntListener gameEventIntListener = widget.GetComponent<GameEventIntListener>();
                gameEventIntListener.Event = hp.ValueChangedEvent;
                widget.SetActive(true);

                widget = CreateStatusWidget(allyConfig.StatusWidget, allys[i]);
                GameEventStatusListener gameEventStatusListener = widget.GetComponent<GameEventStatusListener>();
                gameEventStatusListener.Event = status.ValueChangedEvent;
                widget.SetActive(true);

                widget =CreateIntentWidget(allyConfig.IntentWidget, allys[i]);
                IntentChangeEventListener intentChangeEventListener = widget.GetComponent<IntentChangeEventListener>();
                intentChangeEventListener.Event = allyBrainSystem.intentChangeEvent;
                widget.SetActive(true);

                //创建动态同伴角色
                var obj = allys[i].GetComponent<CharacterObject>();
                obj.Template = template;
                obj.Character = new RuntimeCharacter
                {
                    Hp = hp,
                    Power = power,
                    Magic = magic,
                    Armor = armor,
                    MagicResistance = magicResistance,
                    //TODO:同伴系统修改
                    Status = status
                };
                obj.Character.Status.Value.Clear();

                var playerTurnBeginListener = allys[i].GetComponent<GameEventListener>();
                playerTurnBeginListener.Response.AddListener(allyBrainSystem.OnPlayerTurnBegan);

                allyCharacters[i] = obj;

            }
            allysAllCreated = true;

            if (player && enemy && allysAllCreated)
                InitializeGame();

        }
        private void InitializeGame()
        {
            
            deckDrawingSystem.Initialize(deckWidget, discardPileWidget);
            var deckSize = deckDrawingSystem.LoadDeck(playerDeck);
            deckDrawingSystem.ShuffleDeck();

            handPresentationSystem.Initialize(cardPool, deckWidget, discardPileWidget);

            var playerCharacter = player.GetComponent<CharacterObject>();
            var enemyCharacter = enemy.GetComponent<CharacterObject>();
            cardWithArrowSelectionSystem.Initialize(playerCharacter, enemyCharacter);
            enemyBrainSystem.Initialize(playerCharacter, enemyCharacter);
            effectResolutionSystem.Initialize(playerCharacter, enemyCharacter);
            poisonResolutionSystem.Initialize(playerCharacter, enemyCharacter);
            //同伴系统新增
            turnManagementSystem.allyInitialize(allyCharacters);

            turnManagementSystem.BeginGame();
        }

        //三个create方法都进行重构
        private GameObject CreateHpWidget(GameObject prefab, GameObject character, IntVariable hp)
        {
            var hpWidget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(0.0f, -0.5f, 0.0f));
            hpWidget.GetComponent<RectTransform>().anchorMin = canvasPos;
            hpWidget.GetComponent<RectTransform>().anchorMax = canvasPos;
            hpWidget.GetComponent<HpWidget>().Initialize(hp);
            return hpWidget;
        }

        private GameObject CreateAttributeWidget(GameObject prefab, GameObject character, IntVariable power, IntVariable magic, IntVariable armor, IntVariable magicResistance)
        {
            int characterType = 1;
            if (character == enemy) characterType = -1;
            Transform transform = canvas.transform;
            transform.position +=new Vector3(0, 0, -5);
            var widget = Instantiate(prefab, transform, false);
            var pivot = character.transform;
            var size = character.GetComponent<BoxCollider2D>().bounds.size;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(characterType * -2.5f, size.y - 1.5f, 0.0f));
            widget.GetComponent<RectTransform>().anchorMin = canvasPos;
            widget.GetComponent<RectTransform>().anchorMax = canvasPos;
            widget.GetComponent<AttributeWidget>().Initialize(power, magic, armor, magicResistance);
            return widget;
        }

        private GameObject CreateStatusWidget(GameObject prefab, GameObject character)
        {
            var widget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(0.0f, -1.1f, 0.0f));
            widget.GetComponent<RectTransform>().anchorMin = canvasPos;
            widget.GetComponent<RectTransform>().anchorMax = canvasPos;
            return widget;
        }

        private GameObject CreateIntentWidget(GameObject prefab, GameObject character)
        {
            var widget = Instantiate(prefab, canvas.transform, false);
            var pivot = character.transform;
            var size = character.GetComponent<BoxCollider2D>().bounds.size;
            var canvasPos = mainCamera.WorldToViewportPoint(pivot.position + new Vector3(-0.5f, size.y + 0.7f, 0.0f));
            widget.GetComponent<RectTransform>().anchorMin = canvasPos;
            widget.GetComponent<RectTransform>().anchorMax = canvasPos;
            return widget;
        }

        //草药系统新增
        private void CreateHerbBag(HerbLibrary herbLibrary, GameObject herbPrefab)
        {
            foreach (var entry in herbLibrary.Entries)
            {
                // Skip over invalid entries.
                if (entry.herb == null)
                    continue;

                var Herb = Instantiate(herbPrefab);
                Transform transform = Herb.GetComponent<Transform>();
                transform.position = HerbPivot.position;

                var herbObject = Herb.GetComponent<HerbObject>();
                herbObject.SetInfo(entry);
                Vector3 offset = new Vector3(herbPosOffset, 0, 0);
                HerbPivot.position += offset;
            }
        }

    }
}
