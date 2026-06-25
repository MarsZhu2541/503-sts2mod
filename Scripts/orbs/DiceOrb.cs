using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Godot;

namespace Mod503.Scripts;

[RegisterOrb]
public class DiceOrb : ModOrbTemplate
{
    public int CurrentDicePoint { get; private set; } = 1;
    private static readonly Random _diceRng = new Random();
    // 被动效果数值，ModifyOrbValue表示是否吃集中等
     public override decimal PassiveVal => ModifyOrbValue(CurrentDicePoint);

    // 激发效果数值
    public override decimal EvokeVal => ModifyOrbValue(0);

    // 暗色，使用球的主体色的暗色调
    public override Color DarkenedColor => new(0.1f, 0.2f, 0.5f);

    // 对于图片，只要是godot支持的格式都可以，例如png,jpg,svg等等
    public override OrbAssetProfile AssetProfile => new(
        // 提示文本小图标路径
        IconPath: "res://Mod503/images/relics/Dice.png",
               // 充能球场景路径
        VisualsScenePath: "res://Mod503/scenes/dice_orb.tscn"
    );
     public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
    {
        await RollSingleDice(choiceContext);
    }
     public async Task<int> RollSingleDice(PlayerChoiceContext choiceContext)
    {
        CurrentDicePoint = _diceRng.Next(1, 7);
        return CurrentDicePoint;
    }


    // 让你不需要手动挂脚本。复制即可。
    protected override Node2D? TryCreateOrbSprite() => RitsuGodotNodeFactories.CreateFromScenePath<Node2D>(AssetProfile.VisualsScenePath!);


    // 触发被动
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {

    }

    // 触发激发，返回受影响的角色
    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
    {
        return [Owner.Creature];
    }
}