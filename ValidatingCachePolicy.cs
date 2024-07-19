using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

public sealed class ValidatingCachePolicy : IOutputCachePolicy
{
    public ValidatingCachePolicy()
    {
    }

    ValueTask IOutputCachePolicy.CacheRequestAsync(
        OutputCacheContext context,
        CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeFromCacheAsync
        (OutputCacheContext context, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeResponseAsync
        (OutputCacheContext context, CancellationToken cancellationToken)
    {
        var response = context.HttpContext.Response;

        // Console.WriteLine("Deciding about caching: " + context.HttpContext.Request.GetDisplayUrl());
        // Console.WriteLine($"Content size: {response.ContentLength} StatusCode: {response.StatusCode}");
        // Console.WriteLine("Content: " + response.Body);

        // Check response code
        if (response.StatusCode != StatusCodes.Status200OK &&
            response.StatusCode != StatusCodes.Status301MovedPermanently)
        {
            context.AllowCacheStorage = false;
            Console.WriteLine("Not storing response in cache because it wasnt 200 or 301");
            return ValueTask.CompletedTask;
        }

        // if (response.ContentLength < 100)
        // {
        //     context.AllowCacheStorage = false;
        //     Console.WriteLine("Not storing response in cache because content lenght was under 100 bytes");
        // }

        return ValueTask.CompletedTask;
    }


}