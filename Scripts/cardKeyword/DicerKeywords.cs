using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Mod503.Scripts;

[RegisterOwnedCardKeyword(nameof(Luckier), CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(LuckyPower), IconPath = "res:///Mod503/images/powers/lucky_power_64.png", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(EnhanceLucky), IconPath = "res:///Mod503/images/powers/enhance_luck_64.png", CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
public class DicerKeywords
{
    public static readonly CardKeyword Luckier = ModContentRegistry.GetQualifiedKeywordId("Mod503", nameof(Luckier)).GetModCardKeyword();
    public static readonly CardKeyword LuckyPower = ModContentRegistry.GetQualifiedKeywordId("Mod503", nameof(LuckyPower)).GetModCardKeyword();
    public static readonly CardKeyword EnhanceLucky = ModContentRegistry.GetQualifiedKeywordId("Mod503", nameof(EnhanceLucky)).GetModCardKeyword();

}