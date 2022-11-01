using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

// ReSharper disable InconsistentNaming

namespace GankM;

public class PreviewPanel : ViewModel
{
    internal readonly GauntletLayer layer = new(int.MaxValue);
    private ItemCollectionElementViewModel itemVisualModel = new();
    private string ItemModifier;
    private string StringId;
    private string weaponName;

    public PreviewPanel(EquipmentElement weapon = default)
    {
        weaponName = weapon.Item?.Name.ToString()
                     ?? Hero.MainHero.BattleEquipment[0].Item?.Name.ToString()
                     ?? "Weapon";
        layer.LoadMovie("PreviewPanel", this);
        ItemVisualModel.FillFrom(weapon.Item is null ? Hero.MainHero.BattleEquipment[0] : weapon);
        ItemModifier = ItemVisualModel.ItemModifierId;
        StringId = ItemVisualModel.StringId;
    }

    [DataSourceProperty]
    public ItemCollectionElementViewModel ItemVisualModel
    {
        get => itemVisualModel;
        set
        {
            itemVisualModel = value;
            OnPropertyChangedWithValue(value);
        }
    }

    public string WeaponName
    {
        get => weaponName;
        set => weaponName = value;
    }

    public void DismissPreview()
    {
        ScreenManager.TopScreen.RemoveLayer(layer);
        layer.InputRestrictions.ResetInputRestrictions();
    }
}
