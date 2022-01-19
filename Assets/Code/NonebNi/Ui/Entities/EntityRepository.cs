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
            var allEntities = Object.FindObjectsOfType<Entity>();

            return allEntities.FirstOrDefault(e => e.guid == guid);
        }
    }
}