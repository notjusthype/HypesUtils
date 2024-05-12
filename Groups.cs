using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;

namespace HypesUtils
{
    internal class Groups
    {
        private static List<Group>? groups;

        public static List<Group>? Load(string moduleDirectory)
        {
            try
            {
                if (!File.Exists(moduleDirectory))
                {
                    Group player = new Group().setName("Player").setPower(0);
                    Group vip = new Group().setName("VIP").setPower(1).setGroupColor("{Magenta}").setMessageColor("{Magenta}").setNameColor("{White}");
                    Group moderator = new Group().setName("Moderator").setPower(2).setGroupColor("{Green}").setMessageColor("{Green}").setNameColor("{White}");
                    Group admin = new Group().setName("Admin").setPower(3).setGroupColor("{Orange}").setMessageColor("{Orange}").setNameColor("{White}");
                    Group owner = new Group().setName("Owner").setPower(4).setGroupColor("{DarkRed}").setMessageColor("{DarkRed}").setNameColor("{White}");

                    List<Group> groups = new List<Group>([player, vip, moderator, admin, owner]);

                    File.WriteAllText(moduleDirectory, JsonConvert.SerializeObject(groups, Newtonsoft.Json.Formatting.Indented));
                }

                List<Group> result = JsonConvert.DeserializeObject<List<Group>>(File.ReadAllText(moduleDirectory));
                groups = result;

                return groups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load groups: {ex.Message}");
                return null;
            }
        }

        public Group Get(int power)
        {
            return groups.FirstOrDefault(x => x.power == power);
        }

        public Group? Get(string groupName)
        {
            return groups.FirstOrDefault(x => x.name.ToLower() == groupName.ToLower());
        }

        public Group GetUserGroup(Player player)
        {
            return groups.FirstOrDefault(x => x.power == player.power);
        }
    }
}