using System.IO;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ScreenSystem;

namespace GankM
{
    public class GankBehavior : CampaignBehaviorBase
    {
        private static string[] specialWeapons;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, Setup);
        }

        private static void Setup(CampaignGameStarter gameStarter)
        {
            specialWeapons = File.ReadAllLines(Path.Combine(ModuleHelper.GetModuleFullPath("GankM"), "weapons.txt"));
            bool tookWeapon = default;
            gameStarter.AddPlayerLine("takeWeapon", "hero_main_options", "takeWeapon", "{=GankMSteal}Your weapon belongs to me now.", CanTakeWeapon, () =>
            {
                TakeWeapon();
                tookWeapon = true;
            });
            gameStarter.AddDialogLine("youTookMyWeapon", "takeWeapon", "", "{=GankMReply}You'll regret that!", () => tookWeapon, null);
        }

        private static void TakeWeapon()
        {
            var weapon = Hero.OneToOneConversationHero.BattleEquipment[0];
            var panel = new PreviewPanel(weapon);
            ScreenManager.TopScreen.AddLayer(panel.layer);
            Hero.OneToOneConversationHero.BattleEquipment[0] = new EquipmentElement();
            MobileParty.MainParty.ItemRoster.Add(new ItemRosterElement(weapon.Item, 1));
            MBInformationManager.AddQuickInformation(new TextObject($"You took {weapon.Item.Name} from {Hero.OneToOneConversationHero.Name}"));
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, Hero.OneToOneConversationHero, -100);
        }

        private static bool CanTakeWeapon()
        {
            return MobileParty.MainParty?.PrisonRoster?.Count > 0
                   && MobileParty.MainParty.PrisonRoster.GetTroopRoster().AnyQ(e => e.Character == CharacterObject.OneToOneConversationCharacter)
                   && Hero.OneToOneConversationHero.BattleEquipment[0].Item is not null
                   && specialWeapons.AnyQ(weapon => Hero.OneToOneConversationHero.BattleEquipment.Contains(weapon));
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
