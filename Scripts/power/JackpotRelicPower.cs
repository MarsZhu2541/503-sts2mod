using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterPower]
public class JackpotRelicPower : ModPowerTemplate
{
    private const int Threshold = 30;
    private int _maxPricePoolSeen;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(Threshold)
    ];

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/jackpot_relic_64.png",
        BigIconPath: "res://Mod503/images/powers/jackpot_relic_256.png"
    );

    protected override IEnumerable<string> RegisteredKeywordIds => ["MOD503_KEYWORD_PRICE_POOL"];

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is PricePool pricePool && power.Owner == Owner)
        {
            _maxPricePoolSeen = Math.Max(_maxPricePoolSeen, Math.Max(pricePool.Amount, pricePool.MaxReached));
        }
        return Task.CompletedTask;
    }

    public override Task AfterCombatVictory(CombatRoom room)
    {
        var player = Owner.Player!;
        var maxReached = Math.Max(
            _maxPricePoolSeen,
            player.Creature.GetPower<PricePool>()?.MaxReached ?? 0);
        if (maxReached < Threshold)
        {
            return Task.CompletedTask;
        }

        Flash();
        room.AddExtraReward(player, new RelicReward(player));
        return Task.CompletedTask;
    }
}
