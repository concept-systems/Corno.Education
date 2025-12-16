namespace Corno.Services.Corno;

public interface IHasChildCollections<T>
{
    void ConfigureGraphMapping(RefactorThis.GraphDiff.MappingBuilder<T> map);
}

