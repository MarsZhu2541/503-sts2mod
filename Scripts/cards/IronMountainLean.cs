using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
[RegisterCharacterStarterCard(typeof(Dicer), 4)]
public class IronMountainLean : ModCardTemplate
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Basic;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    public override bool GainsBlock => true;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/IronMountainLean.png"
    // 卡框等，有需求自己添加。需要自行判断卡牌类型（攻击、技能、能力等）设置，建议写在基类里。
    // 如果使用自定义卡池，需要改下material，看添加人物章节的添加卡池部分
    // FramePath: "", // 卡牌背景
    // PortraitBorderPath: "", // 边框（状态牌感染使用的）
    // BannerTexturePath: "" // 横幅（不同类型）
    );


    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(3m, ValueProp.Move)
    ];

    public IronMountainLean() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 获取玩家当前拥有的所有充能球
        var orbs = Owner.PlayerCombatState.OrbQueue.Orbs;

        // 查找第一个 DiceOrb 类型的充能球
        var diceOrb = orbs.OfType<DiceOrb>().FirstOrDefault();

        // 掷骰子刷新
        await diceOrb.RollSingleDice(choiceContext);
        // 获取骰子点数，如果没有骰子球则默认为0
        int dicePoint = diceOrb?.CurrentDicePoint ?? 0;

        // 计算最终护甲 = 基础护甲 + 骰子点数
        decimal finalBlock = DynamicVars.Block.BaseValue + (decimal)dicePoint;
        await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(finalBlock, ValueProp.Move), cardPlay);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
       DicerKeywords.Luckier
   ];
}