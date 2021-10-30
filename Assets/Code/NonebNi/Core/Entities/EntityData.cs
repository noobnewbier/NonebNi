namespace NonebNi.Core.Entities
{
    /// <summary>
    /// Todo: add in SAUCE so the change of one data reflects on all level(or just all units in the same scene)
    /// </summary>
    public abstract class EntityData
    {
        public string Name { get; }

        protected EntityData(string name)
        {
            Name = name;
        }
    }
}