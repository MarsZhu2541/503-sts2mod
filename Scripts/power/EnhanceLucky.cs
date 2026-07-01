using MegaCrit.Sts2.Core.Combat;
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
using MegaCrit.Sts2.Core.Commands;

namespace Mod503.Scripts;

[RegisterPower]
public class EnhanceLucky : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/enhance_lucky_64.png",
        BigIconPath: "res://Mod503/images/powers/enhance_lucky_256.png"
    );

    // 悬浮提示展示Luckier关键词（仅UI）
     protected override IEnumerable<string> RegisteredKeywordIds => ["MOD503_KEYWORD_LUCKY_POWER","MOD503_KEYWORD_LUCKIER"];

        
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (Amount <= 0
           || cardPlay.Card.Owner != Owner.Player
           || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }
        Player player = Owner.Player;
        var luckyPower = player.Creature.Powers.OfType<LuckyPower>();
        luckyPower.FirstOrDefault()?.setLuckyBonus(luckyPower.FirstOrDefault().LuckyBonus + 1);
    }

}