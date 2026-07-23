using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Mod503.Scripts;

public static class DiceChoiceHelper
{
    public static async Task<CardModel?> ChooseTwo<TLeft, TRight>(
        PlayerChoiceContext choiceContext,
        Player player)
        where TLeft : CardModel
        where TRight : CardModel
    {
        var combat = player.Creature.CombatState;
        if (combat == null)
        {
            return null;
        }

        var left = combat.CreateCard<TLeft>(player);
        var right = combat.CreateCard<TRight>(player);
        return await CardSelectCmd.FromChooseACardScreen(
            choiceContext,
            [left, right],
            player,
            canSkip: false);
    }
}
