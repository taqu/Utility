﻿using UnityEngine;
using System.Collections;

public class SceneFlow : MonoBehaviour
{
    private Flow.Process process_;
    private Flow.SequenceCache sequenceCache_ = new Flow.SequenceCache();

    IEnumerator print(string str)
    {
        Debug.LogFormat("{0} - {1}", str, Time.time);
        IEnumerator wait = Flow.WaitFor.seconds(3.0f);
        while(wait.MoveNext()) {
            yield return null;
        }
    }

    void printFunc(string str)
    {
        Debug.LogFormat("{0} - {1}", str, Time.time);
    }

	void Start()
    {
        Flow.Sequence sequence = sequenceCache_.sequence();
        sequence.Add(print("Sequence A"));
        //sequence.Add(Flow.WaitFor.seconds(1.0f));
        sequence.Add(print("Sequence B"));

        for(int i = 0; i < 5; ++i) {
            Flow.Concurrent concurrent = sequenceCache_.concurrent();
            concurrent.Add(print("Concurrent A" + i));
            concurrent.Add(print("Concurrent B" + i));
            sequence.Add(concurrent);
        }

        Flow.DelayedConcurrent delayedConcurrent = sequenceCache_.delayedConcurrent();
        delayedConcurrent.Add(print("Delayed A"));
        delayedConcurrent.Add(print("Delayed B"), 1.0f);
        delayedConcurrent.Add(new Flow.Functor(Flow.ToDelegate1<string>.Do(this.printFunc), "Delayed Func A"), 1.0f);
        sequence.Add(delayedConcurrent);

        process_ = sequenceCache_.build(sequence);
	}

	void Update()
    {
        process_.run();
	}
}
