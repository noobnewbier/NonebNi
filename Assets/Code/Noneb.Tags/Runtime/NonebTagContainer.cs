using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Noneb.Tags.Runtime
{
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class NonebTagContainer : IEnumerable<NonebTag>
    {
        [SerializeField] private List<NonebTag> tags = new ();

        public NonebTagContainer() { }

        public NonebTagContainer(NonebTagContainer other)
        {
            tags = new (other.tags);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private string DebuggerDisplay => $"Count = ({tags.Count})";

        public IEnumerator<NonebTag> GetEnumerator() => tags.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<NonebTag> GetTags() => tags;

        public void Clear()
        {
            tags.Clear();
        }

        public void Add(NonebTag tag)
        {
            tags.Add(tag);
        }

        public void Add(IEnumerable<NonebTag> newTags)
        {
            foreach (var tag in newTags)
                Add(tag);
        }

        public void Remove(NonebTag tag)
        {
            tags.Remove(tag);
        }

        public void Remove(in NonebTagContainer other)
        {
            foreach (var tag in other.GetTags()) tags.Remove(tag);
        }

        public bool Has(NonebTag tag)
        {
            foreach (var t in tags)
            {
                if (t == tag) return true;

                if (t.ParentTags.Contains(tag)) return true;
            }

            return false;
        }

        public bool HasAll(IEnumerable<NonebTag> otherTags) => otherTags.All(Has);

        public bool HasAny(IEnumerable<NonebTag> otherTags) => otherTags.Any(Has);
    }
}