using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterPower]
public class PricePool : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>本场战斗中奖池达到过的最大值（爆运重置层数后仍保留）。</summary>
    public int MaxReached { get; private set; }

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/price_pool_64.png",
        BigIconPath: "res://Mod503/images/powers/price_pool_256.png"
    );

    // 悬浮提示展示Luckier关键词（仅UI）
    protected override IEnumerable<string> RegisteredKeywordIds => ["MOD503_KEYWORD_PRICE_POOL"];

    public override Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (ReferenceEquals(power, this))
        {
            MaxReached = Math.Max(MaxReached, Amount);
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // 爆运：奖池回到1，但保留本场最大奖池记录
        if (cardPlay.Card is Explosive)
        {
            MaxReached = Math.Max(MaxReached, Amount);
            if (Amount != 1)
            {
                await PowerCmd.ModifyAmount(ctx, this, 1 - Amount, null, null);
            }
            return;
        }

        if (Amount <= 0
          || cardPlay.Card.Owner != Owner.Player
          || cardPlay.Card.Type == CardType.Power
          || !cardPlay.Card.Keywords.Contains(DicerKeywords.Luckier)
          || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }

        // 不触发护甲/下注结算，避免与卡牌自身掷骰重复结算
        var rollPoint = await DiceOrb.getRollPoint(ctx, cardPlay.Card.Owner, resolveExtras: false);
        if (rollPoint > 5)
        {
            await PowerCmd.Apply<PricePool>(ctx, cardPlay.Card.Owner.Creature, 1, cardPlay.Card.Owner.Creature, null);
            MaxReached = Math.Max(MaxReached, Amount);
        }
    }
}