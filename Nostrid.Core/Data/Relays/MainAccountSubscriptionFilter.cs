using Nostrid.Misc;
using NNostr.Client;

namespace Nostrid.Data.Relays;

public class MainAccountSubscriptionFilter : SubscriptionFilter
{
    private readonly string[] ids;

	public MainAccountSubscriptionFilter(string id)
    {
        ids = new[] { id };
        ParamsId = Utils.HashWithSHA256($"masf:{nameof(MainAccountSubscriptionFilter)}:{id}");
    }

    public override NostrSubscriptionFilter[] GetFilters()
    {
        return new[] {
            new NostrSubscriptionFilter() { Authors = ids, Kinds = new[]{ NostrKind.Metadata, NostrKind.Contacts }, Limit = 1 },
            new NostrSubscriptionFilter() { Authors = ids, Kinds = new[]{ NostrKind.Deletion } }
        };
    }

    public override SubscriptionFilter Clone()
    {
        return new MainAccountSubscriptionFilter(ids[0]);
    }
}

