using System;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Develop
{
    public class ValueTrackerTest : MonoBehaviour
    {
        [SerializeField] private string trackName = "a";
        [SerializeField] private float value;


        private void Update()
        {
            ValueTracker.Track(trackName, value);
        }
    }
}