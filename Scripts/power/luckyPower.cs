using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using Mod503.Scripts;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Mod503.Scripts;

[RegisterPower]
public class LuckyPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public int LuckyBonus { get; set; } = 1;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(LuckyBonus)];
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/lucky_power_64.png",
        BigIconPath: "res://Mod503/images/powers/lucky_power_256.png"
    );
    protected override IEnumerable<string> RegisteredKeywordIds => ["MOD503_KEYWORD_LUCKIER"];
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
        await PowerCmd.Decrement(this);
    }


    internal async Task setLuckyBonusAsync(int v)
    {
        LuckyBonus = v;
        await Cmd.CustomScaledWait(0.2f, 0.4f);
        Flash();
    }
}