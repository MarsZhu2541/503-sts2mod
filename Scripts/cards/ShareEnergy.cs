using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class ShareEnergy : ModCardTemplate
{
    private const int energyCost = 0;
    private int energyCount = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    // AnyPlayer：单人可点自己，多人可选队友
    private const TargetType targetType = TargetType.AnyPlayer;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        DicerKeywords.Luckier
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(energyCount)
    ];

    public ShareEnergy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var rollPoint = await DiceOrb.getRollPoint(choiceContext, cardPlay.Card.Owner);
        var targetPlayer = cardPlay.Target?.Player;
        if (targetPlayer == null)
        {
            return;
        }

        if (rollPoint > 5)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue + 1, targetPlayer);
        }
        else if (rollPoint == 1)
        {
            await PlayerCmd.LoseEnergy(1, targetPlayer);
        }
        else
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, targetPlayer);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1);
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(DicerKeywords.LuckyPower),
    ];
}
