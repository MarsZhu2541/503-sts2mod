using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterPower]
public class DiceArmorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    // Amount = 每次获得的护甲值
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/dice_armor_64.png",
        BigIconPath: "res://Mod503/images/powers/dice_armor_256.png"
    );

    private const int block = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(block, ValueProp.Move)
    ];

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
        if (DiceOrb.getCurrentPoint(ctx, cardPlay.Card.Owner) > 5)
        {
              await CreatureCmd.GainBlock(
                cardPlay.Card.Owner.Creature,
                new BlockVar(block, ValueProp.Move),
                null);
        
        }   
    }

}
