using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.DataIds;
using NonebNi.Core.Factions;
using NonebNi.Core.Maps;

namespace NonebNi.Core.AI
{
    public interface IInfluenceMap
    {
        /// <summary>
        /// Faction's influence on a point. Value is always normalized [0-1]
        /// </summary>
        float FindInfluence(DataId<Faction> faction, Coordinate coord);

        InfluenceArea FindInfluence(DataId<Faction> faction, Coordinate coord, int radius);
        void AddInfluence(DataId<Faction> faction, IInfluenceSource source);
    }

    /// <summary>
    /// Note:
    /// Might(when to dirty stuffs?) be tricky to make caching work. Could be more robust to make this functional - if it's a
    /// perf bottleneck we can do caching.
    /// Ref link9
    /// https://github.com/apoch/curvature
    /// https://github.com/prime31/Nez/blob/master/Nez.Portable/AI/UtilityAI/Considerations/Appraisals/ActionAppraisal.cs
    /// </summary>
    public class InfluenceMap : IInfluenceMap
    {
        /*
         * Note:
         * We aren't caching stuffs atm, mostly because it's performant enough outside of debug(which we can live with, by tweaking the update rate)
         * Cache dirtying is a bit annoying to implement so let's do that later.
         *
         * https://www.gdcvault.com/play/1018040/Architecture-Tricks-Managing-Behaviors-in
         * imap watch https://www.gdcvault.com/play/1025243/Spatial-Knowledge-Representation-through-Modular
         */
        private readonly Dictionary<DataId<Faction>, HashSet<IInfluenceSource>> _influenceSources = new ();
        private readonly IReadOnlyMap _map;

        public InfluenceMap(IReadOnlyMap map)
        {
            _map = map;
        }

        /*
         * Note:
         * We can implement more type of influence later, we probably need more like "weak/valuable-tagert" map.
         */

        /// <summary>
        /// Faction's influence on a point. Value is always normalized [0-1]
        /// </summary>
        public float FindInfluence(DataId<Faction> faction, Coordinate coord)
        {
            RefreshSources();

            var sources = _influenceSources.GetValueOrDefault(faction);
            if (sources?.Any() != true) return 0;

            var maxScore = sources.Count; // we assume everything is normalized - i.e. max score return by any source is 1.
            var score = sources.Sum(source => source.FindInfluence(coord));

            score /= maxScore;
            return score;
        }

        public InfluenceArea FindInfluence(DataId<Faction> faction, Coordinate coord, int radius)
        {
            RefreshSources();

            var area = new InfluenceArea();

            foreach (var c in coord.WithinDistance(radius))
            {
                var influence = FindInfluence(faction, c);
                area[c] = influence;
            }

            return area;
        }

        public void AddInfluence(DataId<Faction> faction, IInfluenceSource source)
        {
            if (!_influenceSources.TryGetValue(faction, out var sources)) _influenceSources[faction] = sources = new HashSet<IInfluenceSource>();

            sources.Add(source);
        }

        private void RefreshSources()
        {
            foreach (var sources in _influenceSources.Values) sources.RemoveWhere(s => !s.IsValid());

            foreach (var unit in _map.GetAllUnits())
            {
                var source = new BasicInfluenceSource(unit, _map);
                AddInfluence(unit.FactionId, source);
            }
        }
    }
}