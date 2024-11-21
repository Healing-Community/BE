namespace Infrastructure.Interface;

public interface IRedisContext
{
    // ========================== String Operations ==========================

    // Lưu trữ giá trị vào Redis (Set Key-Value)
    Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null);

    // Lấy giá trị từ Redis (Get Key-Value)
    Task<string?> GetValueAsync(string key);

    // Xóa một khóa trong Redis
    Task<bool> DeleteValueAsync(string key);

    // Kiểm tra xem khóa có tồn tại trong Redis không
    Task<bool> KeyExistsAsync(string key);

    // // ========================== List Operations ==========================

    // // Thêm giá trị vào List (vào cuối danh sách)
    // Task<long> AddToListAsync(string listKey, string value);

    // // Lấy tất cả các phần tử trong List
    // Task<string?[]> GetListAsync(string listKey);

    // // Xóa một phần tử trong List
    // Task<bool> RemoveFromListAsync(string listKey, string value);

    // // ========================== Set Operations ==========================

    // // Thêm giá trị vào Set
    // Task<bool> AddToSetAsync(string setKey, string value);

    // // Lấy tất cả các phần tử từ Set
    // Task<string?[]> GetSetAsync(string setKey);

    // // Kiểm tra xem một phần tử có tồn tại trong Set không
    // Task<bool> IsMemberOfSetAsync(string setKey, string value);

    // // Xóa một phần tử khỏi Set
    // Task<bool> RemoveFromSetAsync(string setKey, string value);

    // // ========================== Hash Operations ==========================

    // // Lưu trữ một đối tượng vào Hash
    // Task<bool> SaveEntityToHashAsync(string folderName, object entity, TimeSpan ttl);

    // // ========================== Sorted Set Operations ==========================

    // // Thêm giá trị vào Sorted Set với điểm số
    // Task<bool> AddToSortedSetAsync(string sortedSetKey, string value, double score);

    // // Lấy tất cả các phần tử từ Sorted Set theo thứ tự
    // Task<string?[]> GetSortedSetAsync(string sortedSetKey);

    // // Lấy các phần tử từ Sorted Set trong khoảng điểm số
    // Task<string?[]> GetSortedSetByScoreAsync(string sortedSetKey, double minScore, double maxScore);

    // // Xóa một phần tử khỏi Sorted Set
    // Task<bool> RemoveFromSortedSetAsync(string sortedSetKey, string value);

    // // Lấy thứ hạng của một phần tử trong Sorted Set
    // Task<long?> GetRankInSortedSetAsync(string sortedSetKey, string value);

    // // Lấy điểm số của một phần tử trong Sorted Set
    // Task<double?> GetScoreInSortedSetAsync(string sortedSetKey, string value);
}