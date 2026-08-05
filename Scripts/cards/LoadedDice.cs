using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class LoadedDice : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];


    public LoadedDice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DiceOrb.setForceSix(choiceContext, cardPlay.Card.Owner, true);
        await PowerCmd.Apply<LoadedDicePower>(
            choiceContext,
            cardPlay.Card.Owner.Creature,
            1,
            cardPlay.Card.Owner.Creature,
            null);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}
