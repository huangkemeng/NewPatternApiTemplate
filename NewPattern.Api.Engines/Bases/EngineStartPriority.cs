namespace RenameMe.Api.Engines.Bases
{
    /// <summary>
    /// 值越小 优先级越高
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EngineStartPriority : Attribute
    {
        public EngineStartPriority(int priority)
        {
            Priority = priority;
        }
        public int Priority { get; set; } = DefaultPriority;

        public const int DefaultPriority = 0;
    }
}
