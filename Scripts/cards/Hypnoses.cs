using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class Hypnoses : ModCardTemplate
{
    // 基础耗能
    private const int energyCost = 1;
    private const int count = 2;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
         PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        DicerKeywords.Luckier
    ];

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(count)
    ];

    public Hypnoses() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var rollPoint = await DiceOrb.getRollPoint(choiceContext, cardPlay.Card.Owner);
        if (rollPoint > 5)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Card.Owner.Creature, DynamicVars.Cards.IntValue*2, cardPlay.Card.Owner.Creature, null);
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Card.Owner.Creature, DynamicVars.Cards.IntValue*2, cardPlay.Card.Owner.Creature, null);
        }
        else
        {
           await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Card.Owner.Creature, DynamicVars.Cards.IntValue, cardPlay.Card.Owner.Creature, null);
           await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Card.Owner.Creature, DynamicVars.Cards.IntValue, cardPlay.Card.Owner.Creature, null);
        }
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
         HoverTipFactory.FromKeyword(DicerKeywords.LuckyPower),
    ];
}