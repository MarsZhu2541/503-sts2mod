using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Characters;

public sealed class DicerRelicPool : TypeListRelicPoolModel
{
    public override string EnergyColorName => "RitsuLibModTemplate";
    public override Color LabOutlineColor => Dicer.ThemeColor;

    // 遗物实验室和文本也会读取池子的能量图标路径。
    // 资源路径以 res:// 开头，并且要能在 PCK 内找到对应文件。
    public override string? BigEnergyIconPath => $"res://Mod503/images/characters/energy_big.png";
    public override string? TextEnergyIconPath => $"res://Mod503/images/characters/energy_text.png";
}