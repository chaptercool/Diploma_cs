namespace Diploma_cs.Services;

public static class ServiceHelper
{
    public static T GetService<T>() where T : class
    {
        if (Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(T)) is T service)
        {
            return service;
        }

        throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
    }

    public static T? TryGetService<T>() where T : class
    {
        return Application.Current?.Handler?.MauiContext?.Services.GetService(typeof(T)) as T;
    }
}