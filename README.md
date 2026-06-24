# 503 杀戮尖塔2模组

本项目基于教程https://tutorials.sts2modding.com/ 开发503杀戮尖塔2游戏模组

## 如何添加卡牌
1. 创建卡牌文件 参考 \503-sts2mod\Scripts\cards\TestCard.cs

    修改卡牌图片路径：
    ```
    PortraitPath: $"res://Demo/images/cards/zwk.png"
    ```
2. 添加卡牌图片资源：Demo\images\cards\zwk.png
3. 添加卡牌标题和描述文本 Demo\localization\zhs\cards.json
    key值为{modid}_{类别}_{原id}。例如这里的modid/C#第一命名空间是Demo, 类别是CARD。原始卡牌id为TEST_CARD，是TestCard C#类名的大写snake-case。
    ```
    "DEMO_CARD_TEST_CARD.title": "笑里藏刀",
    "DEMO_CARD_TEST_CARD.description": "造成{Damage:diff()}点伤害。"
    ```
