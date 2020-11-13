using StackExchange.Redis;

namespace Basket.API.Data.Interface
{
    public interface IBasketContext
    {
        IDatabase Redis { get; }
    }
}
