namespace RenameMe.Api.Engines.Bases
{
    public interface IEngine<T>
    {
        void Run(T context);
    }
}
