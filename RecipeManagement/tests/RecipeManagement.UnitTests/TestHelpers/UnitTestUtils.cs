namespace RecipeManagement.UnitTests.TestHelpers;

using System.Reflection;
using Mapster;
using MapsterMapper;
using Services;

public class UnitTestUtils
{
    public static Mapper GetApiMapper()
    {
        var apiAssembly = GetApiAssembly();
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings.Clone();
        typeAdapterConfig.Scan(apiAssembly);
        var mapper = new Mapper(typeAdapterConfig);
        return mapper;
    }

    public static Assembly GetApiAssembly()
    {
        // need to load something from the api for it to be in the loaded assemblies
        _ = new DateTimeProvider();
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "RecipeManagement");
    }
}
