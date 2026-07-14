using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Mod503.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace Mod503.Scripts;

[RegisterCard(typeof(DicerCardPool))]
public class DiceMultiAttack : ModCardTemplate
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）ic
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://Mod503/images/cards/{GetType().Name}.png"
    );

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        DicerKeywords.Luckier
    ];

    public DiceMultiAttack() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        // 获取玩家当前拥有的所有充能球

        var orbs = Owner.PlayerCombatState.OrbQueue.Orbs;

        // 查找第一个 DiceOrb 类型的充能球
        var diceOrb = orbs.OfType<DiceOrb>().FirstOrDefault();

        // 掷骰子刷新
        await diceOrb.RollSingleDice(choiceContext);

        // 获取骰子点数，如果没有骰子球则默认为1
        int dicePoint = diceOrb?.CurrentDicePoint ?? 1;
        // 计算最终伤害 = 基础伤害 + 骰子点数
        var repeat = 2;
        if(dicePoint > 5)
        {
            repeat = 4;
        }

        // 执行攻击
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .WithHitCount(repeat)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    // 升级效果
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}