using Infrastructure.Interface;
using StackExchange.Redis;

namespace Infrastructure.Context;

public class RedisContext(IConnectionMultiplexer connectionMultiplexer) : IRedisContext
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

    // Lưu trữ giá trị vào Redis (Set Key-Value)
    public async Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null)
    {
        return await _database.StringSetAsync(key, value, expiry);
    }

    // Lấy giá trị từ Redis (Get Key-Value)
    public async Task<string?> GetValueAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return value.HasValue ? value.ToString() : null;
    }

    // Xóa một khóa trong Redis
    public async Task<bool> DeleteValueAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    // Kiểm tra xem khóa có tồn tại trong Redis không
    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    // // ========================== List Operations ==========================

    // // Thêm giá trị vào List (vào cuối danh sách)
    // public async Task<long> AddToListAsync(string listKey, string value)
    // {
    //     return await _database.ListRightPushAsync(listKey, value);
    // }

    // // Lấy tất cả các phần tử trong List
    // public async Task<string?[]> GetListAsync(string listKey)
    // {
    //     var values = await _database.ListRangeAsync(listKey);
    //     return values.ToStringArray();
    // }

    // // Xóa một phần tử trong List
    // public async Task<bool> RemoveFromListAsync(string listKey, string value)
    // {
    //     return await _database.ListRemoveAsync(listKey, value) > 0;
    // }

    // // ========================== Set Operations ==========================

    // // Thêm giá trị vào Set
    // public async Task<bool> AddToSetAsync(string setKey, string value)
    // {
    //     return await _database.SetAddAsync(setKey, value);
    // }

    // // Lấy tất cả các phần tử từ Set
    // public async Task<string?[]> GetSetAsync(string setKey)
    // {
    //     var values = await _database.SetMembersAsync(setKey);
    //     return values.ToStringArray();
    // }

    // // Kiểm tra xem một phần tử có tồn tại trong Set không
    // public async Task<bool> IsMemberOfSetAsync(string setKey, string value)
    // {
    //     return await _database.SetContainsAsync(setKey, value);
    // }

    // // Xóa một phần tử khỏi Set
    // public async Task<bool> RemoveFromSetAsync(string setKey, string value)
    // {
    //     return await _database.SetRemoveAsync(setKey, value);
    // }

    // // ========================== Hash Operations ==========================

    // // Lưu trữ một đối tượng vào Hash
    // public async Task<bool> SaveEntityToHashAsync(string folderName, object entity, TimeSpan ttl)
    // {
    //     // Tạo một key duy nhất cho hash với tiền tố thư mục
    //     string hashKey = $"{folderName}:{Ulid.NewUlid().ToString()}";

    //     // Lấy các thuộc tính của đối tượng
    //     var properties = entity.GetType().GetProperties();
    //     var hashEntries = new HashEntry[properties.Length];

    //     // Tạo các cặp key-value cho Redis Hash
    //     for (int i = 0; i < properties.Length; i++)
    //     {
    //         var propertyValue = properties[i].GetValue(entity);
    //         hashEntries[i] = new HashEntry(properties[i].Name, propertyValue?.ToString() ?? string.Empty);
    //     }

    //     // Lưu vào Redis Hash
    //     await _database.HashSetAsync(hashKey, hashEntries);

    //     // Thiết lập TTL cho key
    //     bool isExpiredSet = await _database.KeyExpireAsync(hashKey, ttl);

    //     // Thêm hashKey vào một danh sách quản lý (Collection) với tên thư mục
    //     await _database.ListRightPushAsync(folderName, hashKey);

    //     // Kiểm tra xem số lượng field đã được thêm có bằng số lượng ban đầu không
    //     var fieldsCount = await _database.HashLengthAsync(hashKey);

    //     // Trả về true nếu tất cả field được lưu và TTL được đặt thành công
    //     return fieldsCount == properties.Length && isExpiredSet;
    // }


    // // ========================== Sorted Set Operations ==========================

    // // Thêm giá trị vào Sorted Set với điểm số
    // public async Task<bool> AddToSortedSetAsync(string sortedSetKey, string value, double score)
    // {
    //     return await _database.SortedSetAddAsync(sortedSetKey, value, score);
    // }

    // // Lấy tất cả các phần tử từ Sorted Set theo thứ tự
    // public async Task<string?[]> GetSortedSetAsync(string sortedSetKey)
    // {
    //     var values = await _database.SortedSetRangeByRankAsync(sortedSetKey);
    //     return values.ToStringArray();
    // }

    // // Lấy các phần tử từ Sorted Set trong khoảng điểm số
    // public async Task<string?[]> GetSortedSetByScoreAsync(string sortedSetKey, double minScore, double maxScore)
    // {
    //     var values = await _database.SortedSetRangeByScoreAsync(sortedSetKey, minScore, maxScore);
    //     return values.ToStringArray();
    // }

    // // Xóa một phần tử khỏi Sorted Set
    // public async Task<bool> RemoveFromSortedSetAsync(string sortedSetKey, string value)
    // {
    //     return await _database.SortedSetRemoveAsync(sortedSetKey, value);
    // }

    // // Lấy thứ hạng của một phần tử trong Sorted Set
    // public async Task<long?> GetRankInSortedSetAsync(string sortedSetKey, string value)
    // {
    //     return await _database.SortedSetRankAsync(sortedSetKey, value);
    // }

    // // Lấy điểm số của một phần tử trong Sorted Set
    // public async Task<double?> GetScoreInSortedSetAsync(string sortedSetKey, string value)
    // {
    //     return await _database.SortedSetScoreAsync(sortedSetKey, value);
    // }
}