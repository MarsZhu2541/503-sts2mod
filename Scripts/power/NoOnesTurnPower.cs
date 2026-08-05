using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

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

 public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext, 
        CombatSide side, 
        IEnumerable<Creature> participants)
    {
        // 只在玩家回合结束时触发
        if (side != CombatSide.Player)
        {
            return;
        }

        // 检查是否是当前玩家的能力
        var player = Owner.Player;
        if (player == null)
        {
            return;
        }

        // 回合结束时取消排除1的效果
        await DiceOrb.setExcludeOne(choiceContext, player, false);
        await PowerCmd.Remove(this);
    }
}
