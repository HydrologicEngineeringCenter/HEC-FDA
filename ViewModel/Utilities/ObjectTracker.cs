using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Utilities
{
    public static class ObjectTracker
    {
        private static readonly List<LabeledWeakRef> REFS = new List<LabeledWeakRef>();
        
        public static void TrackObject(object obj, string label)
        {
            bool spinUpRunner = REFS.Count == 0;
            
            REFS.Add(new LabeledWeakRef(obj, label));

            if (spinUpRunner)
            {
                Task.Run(TrackRefs);
            }
        }

        private static void TrackRefs()
        {
            while (REFS.Count > 0)
            {
                foreach (LabeledWeakRef weakRef in REFS)
                {
                    Thread.Sleep(2000);
                    if (!weakRef.Ref.TryGetTarget(out object _))
                    {
                        //Nothing came back, so it's dead.
                        Console.Out.WriteLine(weakRef.Label + " object garbage collected.");
                    }
                }
            }
        }

        private class LabeledWeakRef
        {
            public LabeledWeakRef(object obj, string label)
            {
                Label = label;
                Ref = new WeakReference<object>(obj);
            }

            public string Label { get; }
            public WeakReference<object> Ref { get; }
        }
    }
}