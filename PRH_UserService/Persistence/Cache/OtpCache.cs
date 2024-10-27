using System;
using Application.Interfaces.Redis;
using Infrastructure.Interface;

namespace Persistence.Cache;

public class OtpCache(IRedisContext redisContext) : IOtpCache
{
    readonly string OtpColection = "otp:Email";
    public async Task<bool> DeleteOtpAsync(string key)
    {
        key = $"{OtpColection}:{key}";
        var result = await redisContext.DeleteValueAsync(key);
        return result;
    }

    public async Task<string?> GetOtpAsync(string key)
    {
        key = $"{OtpColection}:{key}";
        var otp = await redisContext.GetValueAsync(key);
        return otp;
    }

    public async Task<bool> OtpExistsAsync(string key)
    {
        key = $"{OtpColection}:{key}";
        var exists = await redisContext.KeyExistsAsync(key);
        return exists;
    }

    public async Task<bool> SaveOtpAsync(string key, string otp, TimeSpan expiry)
    {
        key = $"{OtpColection}:{key}";
        var result = await redisContext.SetValueAsync(key, otp, expiry);
        return result;
    }
}
