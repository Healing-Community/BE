using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Service;
using Application.Interfaces.Services;
using Application.Services;
using CleanArchitecture.Persistence.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;


namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IBookmarkPostRepository, BookmarkPostRepository>();
        services.AddScoped<IBookMarkRepository, BookMarkRepository>();
        services.AddScoped<IUserReferenceRepository, UserReferenceRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<IReactionTypeRepository, ReactionTypeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IReportTypeRepository, ReportTypeRepository>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IFirebaseStorageService, FirebaseService>();
        services.AddScoped<ICommentTreeService, CommentTreeService>();

        //#region MongoDB

        //services.AddSingleton<IMongoClient>(sp =>
        //{
        //    var config = sp.GetRequiredService<IConfiguration>();
        //    var connectionString = config.GetConnectionString("MongoDb");
        //    return new MongoClient(connectionString);
        //});

        //services.AddScoped(typeof(IMongoRepository<>), typeof(MongoGenericMongoRepository<>));
        //#endregion

    }
}