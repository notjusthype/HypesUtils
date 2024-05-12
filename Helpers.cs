using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Reflection;

internal static class CCSPlayerControllerEx
{
    internal static bool IsValidPlayer(this CCSPlayerController? controller)
    {
        return controller != null && controller.IsValid /*&& !controller.IsBot*/;
    }
}

namespace HypesUtils
{
    internal class Helpers
    {
        public CCSGameRules GameRules()
        {
            return Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
        }

        public string ReplaceColorValue(string msg)
        {
            if (msg.Contains('{'))
            {
                string modifiedValue = msg;
                foreach (FieldInfo field in typeof(ChatColors).GetFields())
                {
                    string pattern = $"{{{field.Name}}}";
                    if (msg.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    {
                        modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)!.ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
                return modifiedValue;
            }

            return string.IsNullOrEmpty(msg) ? "[HypesUtils]" : msg;
        }
    }
}