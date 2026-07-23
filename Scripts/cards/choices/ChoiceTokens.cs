using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

/// <summary>选牌界面用的选项卡，不进入图鉴与战斗生成。</summary>
public abstract class DiceChoiceToken : ModCardTemplate
{
    protected DiceChoiceToken()
        : base(0, CardType.Status, CardRarity.Token, TargetType.Self, shouldShowInCardLibrary: false)
    {
    }

    public override bool CanBeGeneratedInCombat => false;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "res://Mod503/images/cards/DiceEnergy.png"
    );

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) =>
        Task.CompletedTask;

    protected override void OnUpgrade()
    {
    }
}

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceHigh : DiceChoiceToken;

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceLow : DiceChoiceToken;

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceOdd : DiceChoiceToken;

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceEven : DiceChoiceToken;

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceReroll : DiceChoiceToken;

[RegisterCard(typeof(DicerCardPool))]
public sealed class ChoiceKeep : DiceChoiceToken;
