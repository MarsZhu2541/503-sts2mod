using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Godot;

namespace Mod503.Scripts;

public enum PendingDiceBet
{
    None,
    High,
    Low,
    Odd,
    Even
}

[RegisterOrb]
public class DiceOrb : ModOrbTemplate
{
    public int CurrentDicePoint { get; private set; } = 0;
    public bool forceSix { get; set; } = false;
    /// <summary>本回合骰子不会掷出1点。</summary>
    public bool excludeOne { get; set; } = false;
    /// <summary>已预知/锁定的下一次基础点数。</summary>
    public int? QueuedBasePoint { get; private set; }
    private bool _queuedConsumesLoaded;
    /// <summary>赌下次骰子的结果。</summary>
    public PendingDiceBet PendingBet { get; set; } = PendingDiceBet.None;

    public override decimal PassiveVal => ModifyOrbValue(CurrentDicePoint);
    public override decimal EvokeVal => ModifyOrbValue(0);
    public override Color DarkenedColor => new(0.1f, 0.2f, 0.5f);
    public override OrbAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/relics/Dice.png",
        VisualsScenePath: "res://Mod503/scenes/dice_orb.tscn"
    );
    private const string DICE_MUSIC = "res://Mod503/audio/dice.mp3";


    public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
    {
        await Task.CompletedTask;
    }

    public int EnsureQueuedBasePoint()
    {
        if (QueuedBasePoint.HasValue)
        {
            return QueuedBasePoint.Value;
        }

        var loaded = Owner.Creature.Powers.OfType<LoadedDicePower>().FirstOrDefault();
        if (forceSix || (loaded?.Amount ?? 0) > 0)
        {
            QueuedBasePoint = 6;
            _queuedConsumesLoaded = !forceSix && (loaded?.Amount ?? 0) > 0;
        }
        else
        {
            Rng rng = base.CombatState.RunState.Rng.CombatTargets;
            int random = rng.NextInt(1, 7);
            if (excludeOne && random == 1)
            {
                random = rng.NextInt(2, 7);
            }
            QueuedBasePoint = random;
            _queuedConsumesLoaded = false;
        }

        return QueuedBasePoint.Value;
    }

    public async Task<int> RollSingleDice(PlayerChoiceContext choiceContext, Player player, bool triggerLuckier = true)
    {
        int random;
        var loaded = player.Creature.Powers.OfType<LoadedDicePower>().FirstOrDefault();
        AudioManager.Play(DICE_MUSIC, 1.5f);
        if (QueuedBasePoint.HasValue)
        {
            random = QueuedBasePoint.Value;
            if (_queuedConsumesLoaded && loaded != null && loaded.Amount > 0)
            {
                await PowerCmd.Decrement(loaded);
            }
            QueuedBasePoint = null;
            _queuedConsumesLoaded = false;
        }
        else if (forceSix)
        {
            random = 6;
        }
        else if (loaded != null && loaded.Amount > 0)
        {
            random = 6;
            await PowerCmd.Decrement(loaded);
        }
        else
        {
            Rng rng = base.CombatState.RunState.Rng.CombatTargets;
            random = rng.NextInt(1, 7);
            if (excludeOne && random == 1)
            {
                random = rng.NextInt(2, 7);
            }
        }

        CurrentDicePoint = random;
        var luckyPower = player.Creature.Powers.OfType<LuckyPower>();
        int luckyPowerAmount = luckyPower.FirstOrDefault()?.Amount ?? 0;
        if (luckyPowerAmount > 0 && triggerLuckier)
        {
            var enhanceLuckyPower = player.Creature.Powers.OfType<EnhanceLucky>();
            int enhancedAmount = enhanceLuckyPower.FirstOrDefault()?.Amount ?? 0;
            CurrentDicePoint += enhancedAmount + 1;
            DiceTextVfx.ShowInfo(player.Creature, $"{random} + {enhancedAmount + 1} = {CurrentDicePoint}");
        }
        else
        {
            DiceTextVfx.ShowInfo(player.Creature, $"{random}");
        }

        return CurrentDicePoint;
    }

    public async Task rollAndBet(PlayerChoiceContext choiceContext, Player player)
    {

        var point = await RollSingleDice(choiceContext, player, false);
        if (PendingBet == PendingDiceBet.None)
        {
            return;
        }

        var bet = PendingBet;
        PendingBet = PendingDiceBet.None;
        var won = bet switch
        {
            PendingDiceBet.High => point >= 4,
            PendingDiceBet.Low => point <= 3,
            PendingDiceBet.Odd => point % 2 == 1,
            PendingDiceBet.Even => point % 2 == 0,
            _ => false
        };

        if (!won)
        {
            DiceTextVfx.ShowInfo(player.Creature, "赌输了");
            return;
        }

        DiceTextVfx.ShowInfo(player.Creature, "赌中了!");
        if (bet is PendingDiceBet.High or PendingDiceBet.Low)
        {
            await PlayerCmd.GainEnergy(1, player);
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, 1, player);
        }
    }

    protected override Node2D? TryCreateOrbSprite() =>
        RitsuGodotNodeFactories.CreateFromScenePath<Node2D>(AssetProfile.VisualsScenePath!);

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        await Task.CompletedTask;
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)
    {
        await Task.CompletedTask;
        return [Owner.Creature];
    }

    public static DiceOrb? Find(Player player) =>
        player.PlayerCombatState?.OrbQueue.Orbs.OfType<DiceOrb>().FirstOrDefault();

    public static async Task<int> getRollPoint(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var diceOrb = Find(player);
        if (diceOrb == null)
        {
            return 1;
        }

        return await diceOrb.RollSingleDice(choiceContext, player);
    }

    public static int getCurrentPoint(PlayerChoiceContext choiceContext, Player player)
    {
        var diceOrb = Find(player);
        if (diceOrb == null)
        {
            return 1;
        }

        return diceOrb.CurrentDicePoint;
    }

    public static Task<bool> setForceSix(PlayerChoiceContext choiceContext, Player player, bool isForce)
    {
        var diceOrb = Find(player);
        if (diceOrb != null)
        {
            diceOrb.forceSix = isForce;
        }
        return Task.FromResult(isForce);
    }

    public static Task<bool> setExcludeOne(PlayerChoiceContext choiceContext, Player player, bool exclude)
    {
        var diceOrb = Find(player);
        if (diceOrb != null)
        {
            diceOrb.excludeOne = exclude;
        }
        return Task.FromResult(exclude);
    }
}
