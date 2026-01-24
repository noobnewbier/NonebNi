using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace NonebNi.Ui.Entities
{
    public interface IEntityRepository
    {
        Entity? GetEntity(Guid guid);
    }

    public class EntityRepository : IEntityRepository
    {
        public Entity? GetEntity(Guid guid)
        {
            // todo: for the love of god, how did this get in.
            var allEntities = Object.FindObjectsOfType<Entity>();

            return allEntities.FirstOrDefault(e => e.guid == guid);
        }
    }
}