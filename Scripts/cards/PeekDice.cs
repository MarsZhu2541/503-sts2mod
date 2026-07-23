using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class PeekDice : ModCardTemplate
{
    private const int energyCost = 0;
    private const int rerollCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];

    public PeekDice() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        var diceOrb = DiceOrb.Find(owner);
        if (diceOrb == null)
        {
            return;
        }

        var preview = diceOrb.EnsureQueuedBasePoint();
        DiceTextVfx.ShowInfo(owner.Creature, $"预知: {preview}");

        var choice = await DiceChoiceHelper.ChooseTwo<ChoiceReroll, ChoiceKeep>(choiceContext, owner);
        if (choice is ChoiceReroll)
        {
            var energy = owner.PlayerCombatState?.Energy ?? 0;
            if (energy >= rerollCost)
            {
                await PlayerCmd.LoseEnergy(rerollCost, owner);
                var rerolled = diceOrb.RerollQueuedBasePoint();
                DiceTextVfx.ShowInfo(owner.Creature, $"重摇: {rerolled}");
            }
            else
            {
                DiceTextVfx.ShowInfo(owner.Creature, "能量不足");
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级后保留
        AddKeyword(CardKeyword.Retain);
    }
}
