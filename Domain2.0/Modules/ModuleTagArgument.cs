using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain.Modules
{
    public enum ArgumentTypeEnum
    {
        Format,
        Sort,
        Custom
    }

    public class ModuleTagArgument
    {
        public string Argument { get; set; }
        public ArgumentTypeEnum ArgumentType
        {
            get
            {
                string[] brokenArgument = this.Argument.Split('=');
                if (brokenArgument.Length > 0)
                {
                    if (brokenArgument[0].Contains("Format"))
                    {
                        return ArgumentTypeEnum.Format;
                    }
                    else if (brokenArgument[0].Contains("Sort"))
                    {
                        return ArgumentTypeEnum.Sort;
                    }
                    else
                    {
                        return ArgumentTypeEnum.Custom;
                    }
                }
                else
                {
                    return ArgumentTypeEnum.Custom;
                }
            }
        }
        public string ArgumentKey
        {
            get
            {
                string[] brokenArgument = this.Argument.Split('=');
                if (brokenArgument.Length > 0)
                {
                    return brokenArgument[0];
                }
                else
                {
                    return "";
                }
            }
        }

        public string ArgumentValue
        {
            get
            {
                string[] brokenArgument = this.Argument.Split('=');
                if (brokenArgument.Length > 0)
                {
                    return brokenArgument[1];
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
