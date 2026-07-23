using MegaCrit.Sts2.Core.Entities.Powers;
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
}
