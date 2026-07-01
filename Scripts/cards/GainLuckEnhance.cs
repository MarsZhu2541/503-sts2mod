using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
[RegisterCharacterStarterCard(typeof(Dicer), 1)]
public class GainLuckEnhance : ModCardTemplate
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Power;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    public GainLuckEnhance() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/gain_luck_enhance.png"
    );

    // public override IEnumerable<CardKeyword> CanonicalKeywords => [
    //    DicerKeywords.EnhanceLucky
    // ];

  
    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(DicerKeywords.EnhanceLucky),
        HoverTipFactory.FromKeyword(DicerKeywords.Luckier),
        HoverTipFactory.FromPower<LuckyPower>()
        
    ];

    
    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        await PowerCmd.Apply<EnhanceLucky>(choiceContext, cardPlay.Card.Owner.Creature, 1, cardPlay.Card.Owner.Creature, null);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        // DynamicVars.Cards.UpgradeValueBy(2);
    }
}