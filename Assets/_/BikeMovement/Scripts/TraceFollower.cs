using PT.Bike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class TraceFollower : MonoBehaviour
    {
        public Transform[] trace;
        public bool _isFollowingTrace;

        public void FollowTrace(Transform[] trace)
        {
            SetIndex();
            this.trace = trace;
            _isFollowingTrace = true;
        }

        public void DontFollowTrace()
        {
            _isFollowingTrace = false;
        }

        [SerializeField] private BikeController _bc;
        [SerializeField] private int _index = 0;
        [SerializeField] private float _tolerance = 3;

        private void Update()
        {
            if (_index >= trace.Length)
            {

            }
            else
            {
                float mag = (trace[_index].position - transform.position).magnitude;
                if (mag < _tolerance)
                {
                    _index++;
                }
                else
                {
                    Quaternion fromTo = Quaternion.FromToRotation(transform.forward, (trace[_index].position - transform.position).normalized);
                    _bc.Steer(new Vector3(fromTo.y, 0f), true);
                }
            }
        }

        private void SetIndex()
        {
            int index = 0;
            float minMag = 0;

            int i = 0;
            foreach (Transform t in trace)
            {
                float mag = (t.position - transform.position).magnitude;
                if (minMag > mag)
                {
                    index = i;
                    minMag = mag;
                }

                i++;
            }

            _index = index;
        }
    }
}