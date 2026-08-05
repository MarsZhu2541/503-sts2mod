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
public class PeekDice : ModCardTemplate
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];


    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    public PeekDice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        var owner = cardPlay.Card.Owner;
        var diceOrb = DiceOrb.Find(owner);
        var peek = diceOrb.PeekPoint();
        DiceTextVfx.ShowInfoWithDuration(owner.Creature, "预知：" + peek, 3f);
    
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
