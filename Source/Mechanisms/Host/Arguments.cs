using System.Collections.Generic;
using Mechanisms.Contracts;

namespace Mechanisms.Host
{
    using ArgumentList = IList<string>;
    using ArgumentMap = IDictionary<int, IList<string>>;

    public class Arguments
    {
        public ArgumentMap Named { get; private set; }
        public ArgumentList Unnamed { get; private set; }

        public Arguments(ArgumentMap named, ArgumentList unnnamed)
        {
            Named = named;
            Unnamed = unnnamed;
        }

        public bool IsSet(int id)
        {
            return Named.ContainsKey(id);
        }

        public string Value(int id)
        {
            Expect.True(IsSet(id), "IsSet(id)");
            Expect.True(Named[id].Count == 1, "Named[id].Count == 1");

            return Named[id][0];
        }
    }
}
