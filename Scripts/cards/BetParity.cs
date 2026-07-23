using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class BetParity : ModCardTemplate
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public BetParity() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        var diceOrb = DiceOrb.Find(owner);
        if (diceOrb == null)
        {
            return;
        }

        var choice = await DiceChoiceHelper.ChooseTwo<ChoiceOdd, ChoiceEven>(choiceContext, owner);
        if (choice == null)
        {
            return;
        }

        diceOrb.PendingBet = choice is ChoiceOdd
            ? PendingDiceBet.Odd
            : PendingDiceBet.Even;

        DiceTextVfx.ShowInfo(
            owner.Creature,
            diceOrb.PendingBet == PendingDiceBet.Odd ? "赌奇数!" : "赌偶数!");
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
