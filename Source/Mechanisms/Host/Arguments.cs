using System.Collections.Generic;

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
    }
}
