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
public class PartyEnergy : ModCardTemplate
{
    private const int energyCost = 0;
    private int energyCount = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
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

    public PartyEnergy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        var rollPoint = await DiceOrb.getRollPoint(choiceContext, owner);

        if (rollPoint > 5)
        {
            var combatState = owner.Creature.CombatState;
            if (combatState == null)
            {
                await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, owner);
                return;
            }

            foreach (var player in combatState.Players)
            {
                await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, player);
            }
        }
        else if (rollPoint == 1)
        {
            await PlayerCmd.LoseEnergy(1, owner);
        }
        else
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, owner);
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
