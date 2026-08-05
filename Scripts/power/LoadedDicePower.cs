using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterPower]
public class LoadedDicePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/loaded_dice_64.png",
        BigIconPath: "res://Mod503/images/powers/loaded_dice_256.png"
    );

        public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Amount <= 0
            || cardPlay.Card.Owner != Owner.Player
            || cardPlay.Card.Type == CardType.Power
            || !cardPlay.Card.Keywords.Contains(DicerKeywords.Luckier)
            || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }
        await DiceOrb.setForceSix(new ThrowingPlayerChoiceContext(), cardPlay.Card.Owner, false);
        await PowerCmd.Decrement(this);
    }

        public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Amount <= 0
            || cardPlay.Card.Owner != Owner.Player
            || cardPlay.Card.Type == CardType.Power
            || !cardPlay.Card.Keywords.Contains(DicerKeywords.Luckier)
            || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }
        await DiceOrb.setForceSix(new ThrowingPlayerChoiceContext(), cardPlay.Card.Owner, true);
    }
}
