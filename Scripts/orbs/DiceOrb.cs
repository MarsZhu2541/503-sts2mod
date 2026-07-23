using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Godot;
using STS2RitsuLib.Ui.Toast;

namespace Mod503.Scripts;

[RegisterOrb]
public class DiceOrb : ModOrbTemplate
{

    public int CurrentDicePoint { get; private set; } = 0;
    public bool forceSix { get; set; } = false;
    /// <summary>本回合骰子不会掷出1点。</summary>
    public bool excludeOne { get; set; } = false;
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
    }
    public async Task<int> RollSingleDice(PlayerChoiceContext choiceContext)
    {
        int random = _diceRng.Next(1, 7);
        // RitsuToastService.ShowInfo("forceSix" + forceSix.ToString());

        if (forceSix)
        {
            random = 6;
        }
        else if (excludeOne && random == 1)
        {
            random = _diceRng.Next(2, 7);
        }
        CurrentDicePoint = random;
        var luckyPower = Owner.Creature.Powers.OfType<LuckyPower>();
        int luckyPowerAmount = luckyPower.FirstOrDefault()?.Amount ?? 0;
        if (luckyPowerAmount > 0)
        {
            var enhanceLuckyPower = Owner.Creature.Powers.OfType<EnhanceLucky>();
            int enhancedAmount = enhanceLuckyPower.FirstOrDefault()?.Amount ?? 0;
            CurrentDicePoint += (enhancedAmount + 1);
            DiceTextVfx.ShowInfo(Owner.Creature, $"{random} + {enhancedAmount + 1} = {CurrentDicePoint}");
        }
        else
        {
            DiceTextVfx.ShowInfo(Owner.Creature, $"{random}");
        }
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

    public static async Task<int> getRollPoint(PlayerChoiceContext choiceContext, Player player)
    {
        var orbs = player.PlayerCombatState.OrbQueue.Orbs;

        // 查找第一个 DiceOrb 类型的充能球
        var diceOrb = orbs.OfType<DiceOrb>().FirstOrDefault();

        // 掷骰子刷新
        await diceOrb.RollSingleDice(choiceContext);

        // 获取骰子点数，如果没有骰子球则默认为0
        int dicePoint = diceOrb?.CurrentDicePoint ?? 1;
        return dicePoint;
    }

    public static Task<bool> setForceSix(PlayerChoiceContext choiceContext, Player player, bool isForce)
    {
        // RitsuToastService.ShowInfo("setForceSix +" + isForce.ToString());

        var orbs = player.PlayerCombatState.OrbQueue.Orbs;

        // 查找第一个 DiceOrb 类型的充能球
        var diceOrb = orbs.OfType<DiceOrb>().FirstOrDefault();

        // 设置强制掷出6点
        if (diceOrb != null)
        {
            diceOrb.forceSix = isForce;
        }

        return Task.FromResult(isForce);
    }

    public static Task<bool> setExcludeOne(PlayerChoiceContext choiceContext, Player player, bool exclude)
    {
        var orbs = player.PlayerCombatState.OrbQueue.Orbs;
        var diceOrb = orbs.OfType<DiceOrb>().FirstOrDefault();
        if (diceOrb != null)
        {
            diceOrb.excludeOne = exclude;
        }
        return Task.FromResult(exclude);
    }
}