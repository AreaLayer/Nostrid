using Nostrid.Misc;
using NNostr.Client;
using System;

namespace Nostrid.Data.Relays;

public class AccountDetailsSubscriptionFilter : SubscriptionFilter
{
    private readonly string[] ids;

	public AccountDetailsSubscriptionFilter(string id) : this(new[] { id })
    {
	}

    public AccountDetailsSubscriptionFilter(string[] ids)
    {
        this.ids = ids;
        ParamsId = Utils.HashWithSHA256("adsf:" + ids.OrderBy(x => x).Aggregate((a, b) => $"{a}:{b}"));
    }

    public override NostrSubscriptionFilter[] GetFilters()
    {
        return new[] {
            new NostrSubscriptionFilter() { Authors = ids, Kinds = new[]{ 0 }, Limit = 1 } // Get the latest details
        };
    }

    public static List<AccountDetailsSubscriptionFilter> CreateInBatch(string[] ids, int batchSize = 150)
    {
        int index = 0;
        var segment = new ArraySegment<string>(ids);
        var ret = new List<AccountDetailsSubscriptionFilter>();
        while (index < ids.Length)
        {
            ret.Add(new AccountDetailsSubscriptionFilter(segment.Slice(index, Math.Min(batchSize, ids.Length - index)).ToArray()));
            index += batchSize;
        }
        return ret;
    }

    public override SubscriptionFilter Clone()
    {
        return new AccountDetailsSubscriptionFilter(ids);
    }
}

