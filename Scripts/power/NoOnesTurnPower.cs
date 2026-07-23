using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

/// <summary>
/// 本回合骰子不会出现1。下一回合开始时自动移除。
/// </summary>
[RegisterPower]
public class NoOnesTurnPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/no_ones_turn_64.png",
        BigIconPath: "res://Mod503/images/powers/no_ones_turn_256.png"
    );

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        // 能力在打出当回合生效，于下一回合开始时清除
        await DiceOrb.setExcludeOne(choiceContext, player, false);
        await PowerCmd.Remove(this);
    }
}
