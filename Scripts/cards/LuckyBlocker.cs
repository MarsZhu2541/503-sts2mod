using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class LuckyBlocker : ModCardTemplate
{
    // 基础耗能
    private const int energyCost = 1;
    private int blocker = 10;
    private int luckier = 1;
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
    );
    

   protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Defend };

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(blocker, ValueProp.Move), new CardsVar(luckier)
    ];

    public LuckyBlocker() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LuckyPower>(choiceContext, cardPlay.Card.Owner.Creature, DynamicVars.Cards.BaseValue, cardPlay.Card.Owner.Creature, null);
        await CreatureCmd.GainBlock(Owner.Creature, new BlockVar(DynamicVars.Block.BaseValue, ValueProp.Move), cardPlay);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(2);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(DicerKeywords.LuckyPower),
        HoverTipFactory.FromKeyword(DicerKeywords.Luckier),
    ];
}