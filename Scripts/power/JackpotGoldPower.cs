using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterPower]
public class JackpotGoldPower : ModPowerTemplate
{
    private const int Threshold = 10;
    private const int GoldReward = 10;
    private int _maxPricePoolSeen;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(Threshold),
        new GoldVar(GoldReward)
    ];

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/jackpot_gold_64.png",
        BigIconPath: "res://Mod503/images/powers/jackpot_gold_256.png"
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

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        var player = Owner.Player!;
        var maxReached = Math.Max(
            _maxPricePoolSeen,
            player.Creature.GetPower<PricePool>()?.MaxReached ?? 0);
        if (maxReached < Threshold)
        {
            return;
        }

        Flash();
        await PlayerCmd.GainGold(GoldReward, Owner.Player!);
    }
}
