using CommandLine;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlobBatchUpdate
{
    sealed class Options
    {
        [Option('s', "Storage", Required = true, HelpText = "Storage account name")]
        public string StorageName { get; set; }

        [Option('k', "Key", Required = true, HelpText = "Storage account key")]
        public string StorageKey { get; set; }

        [Option('c', "UseChinaEndpoint", Required = false, DefaultValue = false, HelpText = "[Optional]Use China Endpoint, default not use")]
        public bool UseChinaEndpoint { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            StringBuilder usage = new StringBuilder();
            usage.AppendLine("BlobBatchUpdate 1.0");

            usage.AppendLine("Usage: BlobBatchUpdate -s StorageName -k StorageKey -c 0|1");

            MemberInfo[] members = GetType().GetMembers();
            foreach (MemberInfo memberInfo in members)
            {
                var data = memberInfo.GetCustomAttributes(typeof(OptionAttribute), false);
                if (data.Any())
                {
                    OptionAttribute optionAttr = (OptionAttribute)data[0];
                    usage.AppendLine("\t" + "-" + optionAttr.ShortName + "\t" + optionAttr.HelpText);
                }
            }

            return usage.ToString();
        }
    }
}
