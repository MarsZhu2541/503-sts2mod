using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Ui.Toast;

namespace Mod503.Scripts;

[RegisterPower]
public sealed class CheatStatusPower
    : ModPowerTemplate, IPowerExtraIconAmountLabelSpecsProvider
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int total { get; set; } = 2;
    public int left { get; set; } = 2;

    // 每次取描述、取角标都会读取这里，同步最新数值
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            yield return new CardsVar(total);
            yield return new DynamicVar("Left", left);
        }
    }

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Mod503/images/powers/CheatStatusPower_64.png",
        BigIconPath: "res://Mod503/images/powers/CheatStatusPower_256.png");

    // 仅使用旧版支持的 Plain，字符串从 DynamicVars 读取
    public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
    {
        return new List<ExtraIconAmountLabelSpec>
        {
            ExtraIconAmountLabelSpec.Plain(
                ExtraIconAmountLabelCorner.TopRight,
                $"{DynamicVars["Left"]}/{DynamicVars.Cards.BaseValue}"
            )
        };
    }

    protected override IEnumerable<string> RegisteredKeywordIds => ["MOD503_KEYWORD_LUCKIER"];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await DiceOrb.setForceSix(new ThrowingPlayerChoiceContext(), player, true);
        total *= Amount;
        left = total;
        
        // 关键：触发 Flash，UI 会重新读取 CanonicalVars + 重建角标文本
        Flash();
        await Task.CompletedTask;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Amount <= 0
            || cardPlay.Card.Owner != Owner.Player
            || cardPlay.Card.Type == CardType.Power
            || !cardPlay.Card.Keywords.Contains(DicerKeywords.Luckier)
            || CombatManager.Instance.IsOverOrEnding)
        {
            return;
        }
        if (left <= 0)
        {
            await DiceOrb.setForceSix(new ThrowingPlayerChoiceContext(), cardPlay.Card.Owner, false);
            return;
        }

        left--;
        // 修改数值后必须调用 Flash 刷新角标
        Flash();
    }
}