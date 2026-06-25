using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Mod503.Characters;

namespace Mod503.Scripts;

[RegisterRelic(typeof(DicerRelicPool))]
[RegisterCharacterStarterRelic(typeof(Dicer))]
public class Dice : ModRelicTemplate
{
    private int _lastCombatId = -1;
    
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"res://Mod503/images/relics/{GetType().Name}.png",
        IconOutlinePath: $"res://Mod503/images/relics/{GetType().Name}.png",
        BigIconPath: $"res://Mod503/images/relics/{GetType().Name}.png"
    );

    // 每次战斗开始时重置
    public override async Task BeforeCombatStart()
    {
        _lastCombatId = -1;
        await Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 获取当前战斗的 ID
        var currentCombatId = player.Creature.CombatState?.GetHashCode() ?? 0;
        
        // 如果这场战斗已经触发过，跳过
        if (_lastCombatId == currentCombatId)
        {
            return;
        }
        
        // 记录这场战斗已触发
        _lastCombatId = currentCombatId;
        
        // 添加 DiceOrb 充能球
        await OrbCmd.Channel<DiceOrb>(choiceContext, Owner);
    }
}